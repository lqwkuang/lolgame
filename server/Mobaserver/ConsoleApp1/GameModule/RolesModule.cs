using Mobaserver.Net;
using Mobaserver.Player;
using MobaServer.MySql;
using MySql.Data.MySqlClient;
using ProtoMsg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mobaserver.GameModule
{
    public class RolesModule : GameModuleBase<RolesModule>
    {
        public override void AddListener()
        {
            base.AddListener();
            NetEvent.Instance.AddEventListener(1201, HandleRolesCreateC2S);
        }

        private void HandleRolesCreateC2S(BufferEntity entity)
        {
            //去数据查询一下角色表有没有相同的名称
            RolesCreateC2S c2sMSG = ProtobufHelper.FromBytes<RolesCreateC2S>(entity.proto);

            RolesCreateS2C s2cMSG = new RolesCreateS2C();
            //数据库查询 结果为空 说明没有该角色 可以成功创建
            if(DBRolesInfo.Instance.Select(MySqlCMD.Where("NickName",c2sMSG.NickName))==null)
            {
                //用户ID
                PlayerEntity playerEntity = PlayerManager.GetPlayerEntityFromSession(entity.session); 


                RolesInfo rolesInfo = new RolesInfo();
                rolesInfo.NickName = c2sMSG.NickName;
                rolesInfo.ID = playerEntity.userInfo.ID;
                rolesInfo.RolesID = playerEntity.userInfo.ID;

                bool result= DBRolesInfo.Instance.Insert(rolesInfo);
                if(result==true)
                {
                    s2cMSG.Result = 0;
                    s2cMSG.RolesInfo = rolesInfo;
                    //缓存角色的信息
                    playerEntity.rolesInfo = rolesInfo;


                }
                else
                {
                    s2cMSG.Result = 2;//未知的异常
                    Debug.Log($"插入角色数据存在异常!昵称{c2sMSG.NickName}");
                }
            }
            else
            {
                s2cMSG.Result = 1;//
            }
            Bufferfactory.CreqateAndSendPackage(entity, s2cMSG); 
        }

        public override void Init()
        {
            base.Init();
        }

        public override void Release()
        {
            base.Release();
            
        }

        public override void RemoveListener()
        {
            base.RemoveListener();
            NetEvent.Instance.RemoveEventListener(1201, HandleRolesCreateC2S);
        }
    }
}
