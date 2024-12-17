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
        //��ȡ��Ӫ
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
        //���Ӣ���Ƿ����Լ���ID
        public bool CheckIsSelfRoles(int rolesID)
        {
            return PlayerModel.Instance.rolesInfo.RolesID == rolesID;
        }
        //�����ɢʱ���� ���������Ϣ
        public void RemoveRoomInfo()
        {
            PlayerModel.Instance.roomInfo = null;
        }
        //��ȡ��ɫ�ǳ�
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
        //�����ɫ��Ϣ
        public void SavePlayerList(RepeatedField<PlayerInfo> playerInfos)
        {
            RoomModel.Instance.playerInfos = playerInfos;
        }
        //��ȡ��ɫ��Ϣ
        public PlayerInfo GetSelfPlayerInfo()
        {
            return RoomModel.Instance.playerObjects[PlayerModel.Instance.rolesInfo.RolesID].
                GetComponent<PlayerCtrl>().playerInfo;
        }
        //��ȡ��ɫ������ �Լ���
        public PlayerCtrl GetSelfPlayerCtrl()
        {
            return RoomModel.Instance.playerObjects[PlayerModel.Instance.rolesInfo.RolesID].
                GetComponent<PlayerCtrl>();
        }
    }
}

