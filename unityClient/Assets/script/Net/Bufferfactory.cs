using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google;
using Google.Protobuf;

namespace Game.Net
{
    public class Bufferfactory
    {
       enum MessageType
        {
            ACk=0,//确认报文
            Login=1,//业务逻辑的报文
        }
        //创建并且发送报文
        public static BufferEntity CreateAndSendpackage(int messageID,IMessage message)
        {
            JsonHelper.Log(messageID, message);
            BufferEntity buffer = new BufferEntity(Usocket.local.endPoint, Usocket.local.sessionID, 0, 0, MessageType.Login.GetHashCode(), messageID, ProtobufHelper.ToBytes(message));
            Usocket.local.Send(buffer);
            return buffer;
        }
    }
}


