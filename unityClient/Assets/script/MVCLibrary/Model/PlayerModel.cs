using ProtoMsg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Model
{
    public class PlayerModel : Singleton<PlayerModel>
    {
        internal RolesInfo rolesInfo;
        internal RoomInfo roomInfo;
        //检查是否是自己的角色
        public bool CheckIsSelf(int rolesID)
        {
            return rolesInfo.RolesID == rolesID;
        }
    }
}
