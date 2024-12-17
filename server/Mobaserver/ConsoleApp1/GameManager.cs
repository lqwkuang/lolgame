using Mobaserver.GameModule;
using Mobaserver.Net;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mobaserver
{
    class GameManager
    {
        static void Main(string[] are)
        {
            Console.WriteLine("启动服务器......");

            GameModuleInit();//游戏模块的初始化

            NetSystemInit();

            Console.ReadLine();//读取一行用户的输入
        }
        public static USocket uSocket;
        static void NetSystemInit()
        {
            uSocket = new USocket(DispatchNetEvent);
            Debug.Log("网络系统初始化完成");
        }
        static void DispatchNetEvent(BufferEntity buffer)
        {
            //进行报文分发

            NetEvent.Instance.Dispatch(buffer.messageID, buffer);
        }
        static void GameModuleInit()
        {
            UserModule.Instance.Init();
            RolesModule.Instance.Init();
            LobbyModule.Instance.Init();
            RoomModule.Instance.Init();
            BattleModule.Instance.Init();
        }
    }

}
