using Game.Model;
using ProtoMsg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RolesCtrl : Singleton<RolesCtrl>
{
    public void SaveRolesInfo(RolesInfo rolesInfo)
    {
        PlayerModel.Instance.rolesInfo = rolesInfo;
    }
    public RolesInfo GetRolesInfo()
    {
       return PlayerModel.Instance.rolesInfo;
    }
    public void SaveRoomInfo(RoomInfo roomInfo)
    {
        PlayerModel.Instance.roomInfo = roomInfo;
    }
    //�����ɢʱ���� ���������Ϣ
    //�ƶ���RoomCtrl��
    //public void RemoveRoomInfo()
    //{
    //    PlayerModel.Instance.roomInfo = null;
    //}
    //�õ�������Ϣ
    public RoomInfo GetRoomInfo()
    {
        return PlayerModel.Instance.roomInfo;
    }
}
