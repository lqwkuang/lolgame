using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Net
{
    //�ͻ��˴���
    public class UClient 
    {
        public IPEndPoint endPoint;
        Usocket usocket1;
        public int sessionID;//�ỰID
        public int sendSn = 0;//�������
        public int handleSn = 0;//�������
        Action<BufferEntity> handleAction;//�����ĵĺ��� ʵ�ʾ��Ƿַ����ĸ�������Ϸģ��

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
        //������Ϣ:���ձ��ĵ���� ����˳����
        public void Handle(BufferEntity buffer)
        {
            if(this.sessionID==0&&buffer.session!=0)
            {
                Debug.Log($"���������͸����ǵĻỰID�ǣ�{buffer.session}");
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
                        Debug.Log($"�յ�ACKȷ�ϱ���,���{buffer.sn}");
                    }
                    break;
                case 1:
                    BufferEntity ackPacka = new BufferEntity(buffer);
                    usocket1.SendAck(ackPacka);//�ȸ��߷����� ���Ѿ��յ��������

                    //��������ҵ���� 
                    HandleLogincPackage(buffer);
                    break;
                default:
                    break;
            }
        }

        ConcurrentDictionary<int, BufferEntity> waitHandle = new ConcurrentDictionary<int, BufferEntity>();
        //����ҵ����
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
                    Debug.Log($"�յ�����ı���:{buffer.sn}");

                }
                return;
            }
            //�������
            handleSn = buffer.sn;
            if (handleAction != null)
            {
                //�ַ�����Ϸģ��ȥ����
                handleAction(buffer);
            }
            BufferEntity nextBuffer;
            if(waitHandle.TryRemove(handleSn+1,out nextBuffer))
            {
                //�жϻ�����û�д�����һ������ 
                HandleLogincPackage(nextBuffer);
            }

        }
        //�����Ѿ����͵ı���
        ConcurrentDictionary<int, BufferEntity> sendPackage = new ConcurrentDictionary<int, BufferEntity>();
        //���ͽӿ�
        public void Send(BufferEntity package)
        {
            package.time = TimeHelp.Now();
            sendSn++;
            package.sn = sendSn;

            package.Encoder(false);
            if(sessionID!=0)
            {
                //�������� ������Ҫ���·���
                sendPackage.TryAdd(sendSn, package);
            }
            else
            {
                //��û��������

            }
            usocket1.Send(package.buffer, endPoint);
        }
        int overtime = 150;

        //��ʱ�ش��ӿ�
        public async void CheckOuttime()
        {
            await Task.Delay(overtime);
            foreach(var item in sendPackage.Values)
            {
                //ȷ���ǲ��ǳ��������� �ر�Socket
                if(item.recurCount>=10)
                {
                    Debug.Log("�ط���������10�Σ��ر�socket");
                    OnDisconnet();
                    return;
                }
                if(TimeHelp.Now() - item.time >= (item.recurCount+1)*overtime)
                {
                    item.recurCount++;
                    Debug.Log($"��ʱ�ط�:����:{item.recurCount}");
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

