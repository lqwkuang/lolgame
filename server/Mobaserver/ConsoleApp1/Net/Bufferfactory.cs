using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mobaserver.Net
{
    class Bufferfactory
    {
        enum MessageType
        {
            ACk = 0,//确认报文
            Login = 1,//业务逻辑的报文
        }

        //创建并且发送报文
        public static BufferEntity CreqateAndSendPackage(UClient uclient,int messageID,IMessage message)
        {
            //如果处于连接状态
            if(uclient.isConnect)
            {
                //打印protoBuf 按照json的格式
                Debug.Log(messageID, message);
                BufferEntity bufferEntity = new BufferEntity(uclient.endPoint, uclient.session, 0, 0, MessageType.Login.GetHashCode(), messageID, ProtobufHelper.ToBytes(message));

                uclient.Send(bufferEntity);
                return bufferEntity;
                
            }
            return null;
        }
        internal static BufferEntity CreqateAndSendPackage(BufferEntity request,IMessage message)
        {
            UClient client = GameManager.uSocket.GetClient(request.session);
            return CreqateAndSendPackage(client, request.messageID, message);
        }


    }
}
