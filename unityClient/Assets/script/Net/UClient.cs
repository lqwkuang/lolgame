using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Net
{
    //客户端代理
    public class UClient 
    {
        public IPEndPoint endPoint;
        Usocket usocket1;
        public int sessionID;//会话ID
        public int sendSn = 0;//发送序号
        public int handleSn = 0;//处理序号
        Action<BufferEntity> handleAction;//处理报文的函数 实际就是分发报文给各个游戏模块

        public UClient(Usocket usocket,IPEndPoint endPoint,int sendsn,int handleSN,int sessionID,Action<BufferEntity> dispatchNetEvent)
        {
            this.usocket1 = usocket;
            this.endPoint = endPoint;
            this.sendSn = sendsn;
            this.handleSn = handleSN;
            this.sessionID = sessionID;
            handleAction = dispatchNetEvent;

            CheckOuttime();
        }
        //处理消息:按照报文的序号 进行顺序处理
        public void Handle(BufferEntity buffer)
        {
            if(this.sessionID==0&&buffer.session!=0)
            {
                Debug.Log($"服务器发送给我们的会话ID是：{buffer.session}");
                this.sessionID = buffer.session;
            }
            /*else
            {
                if(buffer.session==this.sessionID)
                {

                }
            }*/
            switch(buffer.messageType)
            {
                case 0:
                    BufferEntity bufferEntity;
                    if(sendPackage.TryRemove(buffer.sn,out bufferEntity))
                    {
                        Debug.Log($"收到ACK确认报文,序号{buffer.sn}");
                    }
                    break;
                case 1:
                    BufferEntity ackPacka = new BufferEntity(buffer);
                    usocket1.SendAck(ackPacka);//先告诉服务器 我已经收到这个报文

                    //再来处理业务报文 
                    HandleLogincPackage(buffer);
                    break;
                default:
                    break;
            }
        }

        ConcurrentDictionary<int, BufferEntity> waitHandle = new ConcurrentDictionary<int, BufferEntity>();
        //处理业务报文
        private void HandleLogincPackage(BufferEntity buffer)
        {
            if(buffer.sn<=handleSn)
            {
                return;
            }
            if(buffer.sn-handleSn>1)
            {
                if(waitHandle.TryAdd(buffer.sn,buffer))
                {
                    Debug.Log($"收到错序的报文:{buffer.sn}");

                }
                return;
            }
            //更新序号
            handleSn = buffer.sn;
            if (handleAction != null)
            {
                //分发到游戏模块去处理
                handleAction(buffer);
            }
            BufferEntity nextBuffer;
            if(waitHandle.TryRemove(handleSn+1,out nextBuffer))
            {
                //判断缓冲有没有存在下一条数据 
                HandleLogincPackage(nextBuffer);
            }

        }
        //缓存已经发送的报文
        ConcurrentDictionary<int, BufferEntity> sendPackage = new ConcurrentDictionary<int, BufferEntity>();
        //发送接口
        public void Send(BufferEntity package)
        {
            package.time = TimeHelp.Now();
            sendSn++;
            package.sn = sendSn;

            package.Encoder(false);
            if(sessionID!=0)
            {
                //缓存起来 可能需要重新发送
                sendPackage.TryAdd(sendSn, package);
            }
            else
            {
                //还没进行连接

            }
            usocket1.Send(package.buffer, endPoint);
        }
        int overtime = 150;

        //超时重传接口
        public async void CheckOuttime()
        {
            await Task.Delay(overtime);
            foreach(var item in sendPackage.Values)
            {
                //确定是不是超过最大次数 关闭Socket
                if(item.recurCount>=10)
                {
                    Debug.Log("重发次数超过10次，关闭socket");
                    OnDisconnet();
                    return;
                }
                if(TimeHelp.Now() - item.time >= (item.recurCount+1)*overtime)
                {
                    item.recurCount++;
                    Debug.Log($"超时重发:次数:{item.recurCount}");
                    usocket1.Send(item.buffer, endPoint);
                }

            }
            CheckOuttime();
        }
        public void OnDisconnet()
        {
            handleAction = null;
            usocket1.Close();

        }
    }

}

