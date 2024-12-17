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
    //房间解散时调用 清除房间信息
    //移动到RoomCtrl了
    //public void RemoveRoomInfo()
    //{
    //    PlayerModel.Instance.roomInfo = null;
    //}
    //得到房间信息
    public RoomInfo GetRoomInfo()
    {
        return PlayerModel.Instance.roomInfo;
    }
}
