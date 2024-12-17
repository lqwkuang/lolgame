using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

namespace Game.Net
{
    public class BufferEntity 
    {
        public int recurCount = 0;//�ط����� �����ڲ�ʹ�� ����Ҫ���͵��ͻ���
        public IPEndPoint endpoint;//���͵�Ŀ���ն�


        public int protoSize;//�����С
        public int session;//�ỰID
        public int sn;//���
        public int moduleID;//ģ��Id
        public long time;//����ʱ��
        public int messageType;//Э������
        public int messageID;//Э��ID
        public byte[] proto;//ҵ����

        public byte[] buffer;//����Ҫ���͵����� �������յ�������

        //������
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
        //����ACk����
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

            //�ỰID�����

            buffer = Encoder(true);
        }
        public BufferEntity(IPEndPoint endPoint, byte[] buffer)
        {
            this.endpoint = endPoint;
            this.buffer = buffer;
            DeCode();
        }
        public bool isFull = false;
        //�����ķ����л�
        private void DeCode()
        {
            if(buffer.Length>=4)
            {
                protoSize = BitConverter.ToInt32(buffer, 0);//ת��Ϊint����
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
                //��bufferʣ�µ����� ���Ƶ�proto
                Array.Copy(buffer, 32, proto, 0, protoSize);
            }
        }
        //����Ľӿ�
        //��Ҫȷ���Ƿ���Ackȷ�ϱ���
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
                //������
            }
            else
            {
                //ҵ������׷��
                Array.Copy(proto, 0, data, 32, proto.Length);
            }
            buffer = data;
            return data;
        }


       
    }
}

