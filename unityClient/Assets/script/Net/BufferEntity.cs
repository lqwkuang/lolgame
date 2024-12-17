using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

namespace Game.Net
{
    public class BufferEntity 
    {
        public int recurCount = 0;//重发次数 工程内部使用 不是要发送到客户端
        public IPEndPoint endpoint;//发送的目标终端


        public int protoSize;//包体大小
        public int session;//会话ID
        public int sn;//序号
        public int moduleID;//模块Id
        public long time;//发送时间
        public int messageType;//协议类型
        public int messageID;//协议ID
        public byte[] proto;//业务报文

        public byte[] buffer;//最终要发送的数据 或者是收到的数据

        //请求报文
        public BufferEntity(IPEndPoint endPoint,int session,int sn,int moduleID,int messageType,int messageID, byte[] proto)
        {
            protoSize = proto.Length;
            this.endpoint = endPoint;
            this.session = session;
            this.sn = sn;
            this.moduleID = moduleID;
            this.messageType = messageType;
            this.messageID = messageID;
            this.proto = proto;
        } 
        //创建ACk报文
        public BufferEntity(BufferEntity package)
        {
            protoSize = 0;
            this.endpoint = package.endpoint;
            this.session = package.session;
            this.sn = package.sn;
            this.moduleID = package.moduleID;
            this.time = 0;
            this.messageType = 0;
            this.messageID = package.messageID;

            //会话ID和序号

            buffer = Encoder(true);
        }
        public BufferEntity(IPEndPoint endPoint, byte[] buffer)
        {
            this.endpoint = endPoint;
            this.buffer = buffer;
            DeCode();
        }
        public bool isFull = false;
        //将报文反序列化
        private void DeCode()
        {
            if(buffer.Length>=4)
            {
                protoSize = BitConverter.ToInt32(buffer, 0);//转化为int类型
                if(buffer.Length==protoSize+32)
                {
                    isFull = true;
                }
            }
            else
            {
                isFull = false;
                return;
            }
             
            session = BitConverter.ToInt32(buffer, 4);
            sn = BitConverter.ToInt32(buffer, 8);
            moduleID = BitConverter.ToInt32(buffer, 12);
            time = BitConverter.ToInt32(buffer, 16);
            messageType= BitConverter.ToInt32(buffer, 24);
            messageID = BitConverter.ToInt32(buffer, 28);

            if(messageType==0)
            {

            }
            else
            {
                proto = new byte[protoSize];
                //将buffer剩下的数据 复制到proto
                Array.Copy(buffer, 32, proto, 0, protoSize);
            }
        }
        //编码的接口
        //需要确认是否是Ack确认报文
        public byte[] Encoder(bool isAck)
        {
            byte[] data = new byte[32 + protoSize];
            if(isAck==true)
            {
                protoSize = 0;
            }
            byte[] _length= BitConverter.GetBytes(protoSize);
            byte[] _session = BitConverter.GetBytes(session);
            byte[] _sn = BitConverter.GetBytes(sn);
            byte[] _moduleID = BitConverter.GetBytes(moduleID);
            byte[] _time = BitConverter.GetBytes(time);
            byte[] _messagetype = BitConverter.GetBytes(messageType);
            byte[] _messageID = BitConverter.GetBytes(messageID);

            Array.Copy(_length, 0, data, 0, 4);
            Array.Copy(_session, 0, data, 4, 4);
            Array.Copy(_sn, 0, data, 8, 4);
            Array.Copy(_moduleID, 0, data, 12, 4);
            Array.Copy(_time, 0, data, 16, 8);
            Array.Copy(_messagetype, 0, data,24, 4);
            Array.Copy(_messageID, 0, data, 28, 4);
            if(isAck)
            {
                //不处理
            }
            else
            {
                //业务数据追加
                Array.Copy(proto, 0, data, 32, proto.Length);
            }
            buffer = data;
            return data;
        }


       
    }
}

