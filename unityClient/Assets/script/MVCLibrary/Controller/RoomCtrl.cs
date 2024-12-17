using Game.Model;
using Google.Protobuf.Collections;
using ProtoMsg;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Ctrl
{
    public class RoomCtrl :Singleton<RoomCtrl>
    {
        //获取阵营
        public int GetTeamID(int rolesID)
        {
            for (int i = 0; i < PlayerModel.Instance.roomInfo.TeamA.Count; i++)
            {
                if (PlayerModel.Instance.roomInfo.TeamA[i].RolesID == rolesID)
                {
                    return 0;
                }
                if (PlayerModel.Instance.roomInfo.TeamB[i].RolesID == rolesID)
                {
                    return 1;
                }
            }
            return -1;
        }
        //检查英雄是否是自己的ID
        public bool CheckIsSelfRoles(int rolesID)
        {
            return PlayerModel.Instance.rolesInfo.RolesID == rolesID;
        }
        //房间解散时调用 清除房间信息
        public void RemoveRoomInfo()
        {
            PlayerModel.Instance.roomInfo = null;
        }
        //获取角色昵称
        public string GetNickName(int rolesID)
        {
            for(int i=0;i<PlayerModel.Instance.roomInfo.TeamA.Count;i++)
            {
                if (PlayerModel.Instance.roomInfo.TeamA[i].RolesID==rolesID)
                {
                    return PlayerModel.Instance.roomInfo.TeamA[i].NickName;
                }
                if (PlayerModel.Instance.roomInfo.TeamB[i].RolesID == rolesID)
                {
                    return PlayerModel.Instance.roomInfo.TeamB[i].NickName;
                }
            }
            return "";
        }
        //保存角色信息
        public void SavePlayerList(RepeatedField<PlayerInfo> playerInfos)
        {
            RoomModel.Instance.playerInfos = playerInfos;
        }
        //获取角色信息
        public PlayerInfo GetSelfPlayerInfo()
        {
            return RoomModel.Instance.playerObjects[PlayerModel.Instance.rolesInfo.RolesID].
                GetComponent<PlayerCtrl>().playerInfo;
        }
        //获取角色控制器 自己的
        public PlayerCtrl GetSelfPlayerCtrl()
        {
            return RoomModel.Instance.playerObjects[PlayerModel.Instance.rolesInfo.RolesID].
                GetComponent<PlayerCtrl>();
        }
    }
}

