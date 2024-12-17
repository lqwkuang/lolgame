using Mobaserver.Player;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Mobaserver.Net
{
    public class UClient
    {
        private USocket uSocket;
        public IPEndPoint endPoint;
        private int sendSN;
        private int handleSN;
        public int session;
        private Action<BufferEntity> dispatchNetEvent;
        public UClient(USocket uSocket,IPEndPoint endpoint, int sendSN, int handleSN, int session, Action<BufferEntity> dispatchNetEvent)
        {
            this.uSocket = uSocket;
            this.endPoint = endpoint;
            this.sendSN = sendSN;
            this.handleSN = handleSN;
            this.session = session;
            this.dispatchNetEvent = dispatchNetEvent;

            //超时重传检测
            CheckOutTime();
        }
        public bool isConnect = true;

        int overtime = 150;//超时的时间
        //超时重传检测
        private async void CheckOutTime()
        {
            await Task.Delay(overtime);
            foreach(var package in sendPackage.Values)
            {
                if(package.recurCount>=10)
                {
                    Debug.LogError($"重发十次还是失败!,协议ID:{package.messageID}");
                    uSocket.RemoveClient(session);
                    return;
                }

                if(TimeHelp.Now()-package.time>=(package.recurCount+1)*overtime)
                {
                    //重发的次数加一
                    package.recurCount++;
                    Debug.Log($"超时重发，序号是:{package.sn}");
                    uSocket.Send(package.buffer, endPoint);

                }
            }
            CheckOutTime();
        }

        internal void Close()
        {
            isConnect = false;
            //客户端断开 清理缓存 避免游戏模块在获取的时候 获取到错误的数据
            if(PlayerManager.GetPlayerEntityFromSession(session)!=null)
            {
                int roleID = PlayerManager.GetPlayerEntityFromSession(session).rolesInfo.RolesID;
                PlayerManager.RemoveFromRolesID(roleID);
            }
            PlayerManager.RemoveFromSession(session);
        }

        internal void Handle(BufferEntity bufferEntity)
        {
            //要移除掉已经发送的BufferEntity
            int sn = bufferEntity.sn;
            switch(bufferEntity.messageType)
            {
                case 0://ACK确认报文
                    BufferEntity buffer;
                    if(sendPackage.TryRemove(bufferEntity.sn,out buffer))
                    {
                        Debug.Log($"报文已确认，序号:{bufferEntity.sn}");
                    }
                    else
                    {
                        Debug.Log($"要确认的报文不存在，序号:{bufferEntity.sn}");
                    }
                    break;
                case 1://业务报文
                    /*if(bufferEntity .sn!=1)
                    {
                        return;//测试代码
                    }*/
                    BufferEntity ackPackage = new BufferEntity(bufferEntity);
                    uSocket.SendACK(ackPackage, endPoint);
                    Debug.Log("收到的是业务报文");
                    //再进行处理业务报文
                    HandleLogincPackage(bufferEntity);
                    break;
                default:
                    break;
            }
        }


        //存储错序的报文
        ConcurrentDictionary<int, BufferEntity> waitHandle = new ConcurrentDictionary<int, BufferEntity>();
        //处理业务逻辑接口
        private void HandleLogincPackage(BufferEntity bufferEntity)
        {
            if(bufferEntity.sn<=handleSN)
            {
                Debug.Log($"已经处理过的消息，序号：{bufferEntity.sn}");
                return;
            }
            if(bufferEntity.sn-handleSN>1)
            {
                if(waitHandle.TryAdd(bufferEntity.sn,bufferEntity))
                {
                    Debug.Log($"错序的报文，进行缓存，序号是：{bufferEntity.sn}");
                }
                return;
            }
            handleSN = bufferEntity.sn;
            if(dispatchNetEvent!=null)
            {
                Debug.Log("分发消息给游戏模块");
                dispatchNetEvent(bufferEntity);
            }

            BufferEntity nextbuffer;
            if(waitHandle.TryRemove(handleSN+1,out nextbuffer))
            {
                HandleLogincPackage(nextbuffer);
            }
        }

        ConcurrentDictionary<int, BufferEntity> sendPackage=new ConcurrentDictionary<int, BufferEntity>(); 
        //发送接口
       public void Send(BufferEntity package)
        {
            if(isConnect==false)
            {
                return;
            }
            package.time = TimeHelp.Now();
            sendSN++;
            package.sn = sendSN;

            //序列化
            package.Encoder(false);
            uSocket.Send(package.buffer, endPoint);
            if(session!=0)
            {
                //已经发送的数据
                sendPackage.TryAdd(package.sn, package);
            }
        }
    }
}
