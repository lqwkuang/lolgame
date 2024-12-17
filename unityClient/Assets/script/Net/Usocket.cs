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
    //提供Socket发送的接口 以及 Socket接收的业务
    public class Usocket 
    {
        UdpClient udpClient;
        String ip= "10.159.63.251";//服务器IP
        int port=8899;//固定端口
        public static IPEndPoint server;
        public static UClient local;//客户端代理:完成发送的逻辑，和处理的逻辑 保证报文的顺序
        public Usocket(Action<BufferEntity> dispatchNetEvent)
        {
            udpClient = new UdpClient(0);
            server = new IPEndPoint(IPAddress.Parse(ip), port);
            local = new UClient(this, server, 0, 0, 0, dispatchNetEvent);
            ReceiveTask();//接受消息
        }
        ConcurrentQueue<UdpReceiveResult> awaitHandle = new ConcurrentQueue<UdpReceiveResult>();
        //接收报文 异步
        public async void ReceiveTask()
        {
            while(udpClient!=null)
            {
                try
                {
                    UdpReceiveResult result= await udpClient.ReceiveAsync();//等待返回消息
                    Debug.Log("接受到了消息");
                    awaitHandle.Enqueue(result);
                }
                catch(Exception e)
                {
                    Debug.LogError(e);
                }
            }
        }
        //发送报文接口
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
        //发送Ack报文 解包后 马上调用
        public void SendAck(BufferEntity buffer)
        {
            Send(buffer.buffer, server);
        }

        //在Update中去进行调用
        public void Handle()
        {
            if(awaitHandle.Count>0)
            {
                UdpReceiveResult data;
                if(awaitHandle.TryDequeue(out data))
                {
                    //反序列化
                    BufferEntity bufferEntity = new BufferEntity(data.RemoteEndPoint, data.Buffer);
                    if(bufferEntity.isFull)
                    {
                        Debug.Log($"处理消息ID：{bufferEntity.messageID},序号:{bufferEntity.sn}");
                        //处理业务逻辑
                        local.Handle(bufferEntity);
                    }
                   
                }
            }
        }

       //关闭udpClient
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


