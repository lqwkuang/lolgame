using Game.Model;
using ProtoMsg;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Ctrl
{
    public class LoginCtrl : Singleton<LoginCtrl>
    {
        internal void SaveRolesInfo(RolesInfo rolesInfo)
        {
            PlayerModel.Instance.rolesInfo = rolesInfo;
        }
    }
}

