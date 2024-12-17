using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
namespace Game.Net
{
    //�ṩSocket���͵Ľӿ� �Լ� Socket���յ�ҵ��
    public class Usocket 
    {
        UdpClient udpClient;
        String ip= "10.159.63.251";//������IP
        int port=8899;//�̶��˿�
        public static IPEndPoint server;
        public static UClient local;//�ͻ��˴���:��ɷ��͵��߼����ʹ�����߼� ��֤���ĵ�˳��
        public Usocket(Action<BufferEntity> dispatchNetEvent)
        {
            udpClient = new UdpClient(0);
            server = new IPEndPoint(IPAddress.Parse(ip), port);
            local = new UClient(this, server, 0, 0, 0, dispatchNetEvent);
            ReceiveTask();//������Ϣ
        }
        ConcurrentQueue<UdpReceiveResult> awaitHandle = new ConcurrentQueue<UdpReceiveResult>();
        //���ձ��� �첽
        public async void ReceiveTask()
        {
            while(udpClient!=null)
            {
                try
                {
                    UdpReceiveResult result= await udpClient.ReceiveAsync();//�ȴ�������Ϣ
                    Debug.Log("���ܵ�����Ϣ");
                    awaitHandle.Enqueue(result);
                }
                catch(Exception e)
                {
                    Debug.LogError(e);
                }
            }
        }
        //���ͱ��Ľӿ�
        public async void Send(byte[] data,IPEndPoint endPoint)
        {
            if(udpClient!=null)
            {
                try
                {
                  int length= await udpClient.SendAsync(data,data.Length,ip,port);
                }
                catch(Exception e)
                {
                    Debug.LogError(e);
                }
            }
        }
        //����Ack���� ����� ���ϵ���
        public void SendAck(BufferEntity buffer)
        {
            Send(buffer.buffer, server);
        }

        //��Update��ȥ���е���
        public void Handle()
        {
            if(awaitHandle.Count>0)
            {
                UdpReceiveResult data;
                if(awaitHandle.TryDequeue(out data))
                {
                    //�����л�
                    BufferEntity bufferEntity = new BufferEntity(data.RemoteEndPoint, data.Buffer);
                    if(bufferEntity.isFull)
                    {
                        Debug.Log($"������ϢID��{bufferEntity.messageID},���:{bufferEntity.sn}");
                        //����ҵ���߼�
                        local.Handle(bufferEntity);
                    }
                   
                }
            }
        }

       //�ر�udpClient
       public void Close()
        { 
            if(local!=null)
            {
                local = null;
            }
            
            if(udpClient!=null)
            {
                udpClient.Close();
                udpClient = null;
            }
        }
    }
}


