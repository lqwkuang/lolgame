using Mobaserver.Match;
using Mobaserver.Room;
using ProtoMsg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mobaserver.Player
{
    public class PlayerEntity
    {
        public int session;//会话ID
        public UserInfo userInfo;
        public RolesInfo rolesInfo;

        //匹配信息
        public MatchEntity matchEntity;
        //房间信息
        internal RoomEntity roomEntity;
        //阵营ID
        internal int TeamID;

        
       


        //用户销毁的时候
        public void Destroy()
        {
            Debug.Log("用户断开连接");

        }
    }
}
