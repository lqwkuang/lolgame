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

        //��ʼ��
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
        //���ص�¼���
        private void HandleUserLoginS2C(BufferEntity entity)
        {
            UserLoginS2C s2cMSG = ProtobufHelper.FromBytes<UserLoginS2C>(entity.proto);
            switch (s2cMSG.Result)
            {
                case 0:
                    Debug.Log("��¼�ɹ�");
                    //��������
                    if(s2cMSG.RolesInfo!=null)
                    {
                        //�������� 
                        LoginCtrl.Instance.SaveRolesInfo(s2cMSG.RolesInfo);
                        //��ת������
                        WindowManager.Instance.OpenWindow(WindowType.LobbyWindow);
                    }
                    else
                    {
                        //���н�ɫ����
                        WindowManager.Instance.OpenWindow(WindowType.RolesWindow);
                    }

                    Close();//�ر��Լ�

                    break;
                case 1:
                    break;
                case 2:
                    Debug.Log("�˺����벻ƥ��");
                    WindowManager.Instance.ShowTips("�˺����벻ƥ��");
                    break;
                default:
                    break;
            }
        }

        //����ע����
        private void HandleUserRegisterS2C(BufferEntity entity)
        {
            UserRegisterS2C s2cMSG = ProtobufHelper.FromBytes<UserRegisterS2C>(entity.proto);
            switch(s2cMSG.Result)
            {
                case 0:
                    Debug.Log("ע��ɹ�");
                    //����ʾ���ڣ���ʾע��ɹ�
                    WindowManager.Instance.ShowTips("ע��ɹ�");
                    break;
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    Debug.Log("�˺��ѱ�ע��");
                    //����ʾ���ڣ���ʾע��ʧ��
                    WindowManager.Instance.ShowTips("�˺��ѱ�ע��");
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

        //��¼����¼�
        private void LoginBtnOnClick()
        {
            if (string.IsNullOrEmpty(AccountInput.text))
            {
                Debug.Log("�˺�Ϊ��--------");
                return;
            }
            if (string.IsNullOrEmpty(PwdInput.text))
            {
                Debug.Log("����Ϊ��--------");
                return;
            }
            UserLoginC2S c2sMSG = new UserLoginC2S();
            c2sMSG.UserInfo = new UserInfo();
            c2sMSG.UserInfo.Account = AccountInput.text;
            c2sMSG.UserInfo.Password = PwdInput.text;

            Bufferfactory.CreateAndSendpackage(1001, c2sMSG);
        }

        //ע�ᰴť����¼�
        private void RegisterBtnOnClick()
        {
            if(string.IsNullOrEmpty(AccountInput.text))
            {
                Debug.Log("�˺�Ϊ��--------");
                return;
            }
            if(string.IsNullOrEmpty(PwdInput.text))
            {
                Debug.Log("����Ϊ��--------");
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
