using Mobaserver.Net;
using Mobaserver.Player;
using MobaServer.MySql;
using ProtoMsg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mobaserver.GameModule
{
    public class UserModule : GameModuleBase<UserModule>
    {
        public override void AddListener()
        {
            base.AddListener();
            NetEvent.Instance.AddEventListener(1000, HandleUserRegisterC2S);
            NetEvent.Instance.AddEventListener(1001, HandleUserLoginC2S);
        }
        //登录功能
        private void HandleUserLoginC2S(BufferEntity entity)
        {
            //数据库交互
            //反序列化 得到客户端的 发送的数据
            UserLoginC2S c2sMSG= ProtobufHelper.FromBytes<UserLoginC2S>(entity.proto);
            Debug.Log($"登录功能{c2sMSG.UserInfo.Account}");
            //匹配记录
            string sqlCMD = MySqlCMD.Where("Account",c2sMSG.UserInfo.Account)+MySqlCMD.And("Password",c2sMSG.UserInfo.Password);

            UserLoginS2C s2cMSG = new UserLoginS2C();
            UserInfo userInfo= DBUserInfo.Instance.Select(sqlCMD);
            if(userInfo!=null)
            {
                Debug.Log($"登录成功{userInfo.Account}");
                s2cMSG.UserInfo = userInfo;
                s2cMSG.Result = 0;//登录成功

                PlayerManager.Add(entity.session, s2cMSG.UserInfo.ID, new PlayerEntity()
                {
                    userInfo = s2cMSG.UserInfo,
                    session=entity.session,

                });

                RolesInfo rolesInfo= DBRolesInfo.Instance.Select(MySqlCMD.Where("ID", s2cMSG.UserInfo.ID));
                if(rolesInfo!=null)
                {
                    s2cMSG.RolesInfo = rolesInfo;
                    //获取到了角色信息 缓存起来
                    PlayerEntity playerEntity= PlayerManager.GetPlayerEntityFromSession(entity.session);
                    playerEntity.rolesInfo = rolesInfo;
                }
                else
                {

                }
            }
            else
            {
                Debug.Log("登录失败");
                s2cMSG.Result = 2;//账号和密码不匹配
            }
            //发送结果
            Bufferfactory.CreqateAndSendPackage(entity, s2cMSG);

        }
        //注册功能
        private void HandleUserRegisterC2S(BufferEntity entity)
        {
            UserRegisterC2S c2sMSG = ProtobufHelper.FromBytes<UserRegisterC2S>(entity.proto);
            Debug.Log($"注册功能{c2sMSG.UserInfo.Account}");
            UserRegisterS2C s2cMSG = new UserRegisterS2C();
            if (DBUserInfo.Instance.Select(MySqlCMD.Where("Account", c2sMSG.UserInfo.Account))!=null)
            {
                Debug.Log("账号已被注册");
                s2cMSG.Result = 3;
            }
            else
            {
               bool result=  DBUserInfo.Instance.Insert(c2sMSG.UserInfo);
                if(result)
                {
                    s2cMSG.Result = 0;//注册成功
                }
                else
                {
                    s2cMSG.Result = 4;//未知原因导致的失败
                }
            }
            //返回结果
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
            NetEvent.Instance.RemoveEventListener(1000, HandleUserRegisterC2S);
            NetEvent.Instance.RemoveEventListener(1001, HandleUserLoginC2S);
        }
    }
}
