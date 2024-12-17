using Game.Ctrl;
using Game.Net;
using ProtoMsg;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.View
{
    public class LoginWindow : BaseWindow
    {
        public LoginWindow()
        {
            selfType = WindowType.LobbyWindow;
            scenesType = ScenesType.Login;
            resident = false;
            resName = "UIPrefab/User/LoginWindow";
        }
        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
        }

        InputField AccountInput;
        InputField PwdInput;

        //初始化
        protected override void Awake()
        {
            base.Awake();
            AccountInput = transform.Find("UserBack/AccountInput").GetComponent<InputField>();
            PwdInput= transform.Find("UserBack/PwdInput").GetComponent<InputField>();
        }

        protected override void OnAddListener()
        {
            base.OnAddListener();
            NetEvent.Instance.AddEventListener(1000, HandleUserRegisterS2C);
            NetEvent.Instance.AddEventListener(1001, HandleUserLoginS2C);
        }
        //返回登录结果
        private void HandleUserLoginS2C(BufferEntity entity)
        {
            UserLoginS2C s2cMSG = ProtobufHelper.FromBytes<UserLoginS2C>(entity.proto);
            switch (s2cMSG.Result)
            {
                case 0:
                    Debug.Log("登录成功");
                    //保存数据
                    if(s2cMSG.RolesInfo!=null)
                    {
                        //保存数据 
                        LoginCtrl.Instance.SaveRolesInfo(s2cMSG.RolesInfo);
                        //跳转到大厅
                        WindowManager.Instance.OpenWindow(WindowType.LobbyWindow);
                    }
                    else
                    {
                        //进行角色创建
                        WindowManager.Instance.OpenWindow(WindowType.RolesWindow);
                    }

                    Close();//关闭自己

                    break;
                case 1:
                    break;
                case 2:
                    Debug.Log("账号密码不匹配");
                    WindowManager.Instance.ShowTips("账号密码不匹配");
                    break;
                default:
                    break;
            }
        }

        //返回注册结果
        private void HandleUserRegisterS2C(BufferEntity entity)
        {
            UserRegisterS2C s2cMSG = ProtobufHelper.FromBytes<UserRegisterS2C>(entity.proto);
            switch(s2cMSG.Result)
            {
                case 0:
                    Debug.Log("注册成功");
                    //打开提示窗口，显示注册成功
                    WindowManager.Instance.ShowTips("注册成功");
                    break;
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    Debug.Log("账号已被注册");
                    //打开提示窗口，显示注册失败
                    WindowManager.Instance.ShowTips("账号已被注册");
                    break;
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override void OnRemoveListener()
        {
            base.OnRemoveListener();
        }

        protected override void RegisterUIEvent()
        {
            base.RegisterUIEvent();
            for(int i=0;i<buttonList.Length; i++)
            {
                switch(buttonList[i].name)
                {
                    case "RegisterBtn":
                        buttonList[i].onClick.AddListener(RegisterBtnOnClick);
                        break;
                    case "LoginBtn":
                        buttonList[i].onClick.AddListener(LoginBtnOnClick);
                        break;
                    default:
                        break;
                }
                    
            }
        }

        //登录点击事件
        private void LoginBtnOnClick()
        {
            if (string.IsNullOrEmpty(AccountInput.text))
            {
                Debug.Log("账号为空--------");
                return;
            }
            if (string.IsNullOrEmpty(PwdInput.text))
            {
                Debug.Log("密码为空--------");
                return;
            }
            UserLoginC2S c2sMSG = new UserLoginC2S();
            c2sMSG.UserInfo = new UserInfo();
            c2sMSG.UserInfo.Account = AccountInput.text;
            c2sMSG.UserInfo.Password = PwdInput.text;

            Bufferfactory.CreateAndSendpackage(1001, c2sMSG);
        }

        //注册按钮点击事件
        private void RegisterBtnOnClick()
        {
            if(string.IsNullOrEmpty(AccountInput.text))
            {
                Debug.Log("账号为空--------");
                return;
            }
            if(string.IsNullOrEmpty(PwdInput.text))
            {
                Debug.Log("密码为空--------");
                return;
            }
            UserRegisterC2S c2sMSG = new UserRegisterC2S();
            c2sMSG.UserInfo = new UserInfo();
            c2sMSG.UserInfo.Account = AccountInput.text;
            c2sMSG.UserInfo.Password = PwdInput.text;
            Bufferfactory.CreateAndSendpackage(1000, c2sMSG);
        }
    }
}
