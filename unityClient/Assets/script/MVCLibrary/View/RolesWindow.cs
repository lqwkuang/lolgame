using ProtoMsg;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Game.Net;

namespace Game.View
{
    public class RolesWindow : BaseWindow
    {
        public RolesWindow()
        {
            selfType = WindowType.RolesWindow;
            scenesType = ScenesType.Login;
            resident = false;
            resName= "UIPrefab/Roles/RolesWindow";
        }
        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
        }
        InputField InputField;
        protected override void Awake()
        {
            base.Awake();
            InputField = transform.Find("RolesBG/InputField").GetComponent<InputField>();
        }

        protected override void OnAddListener()
        {
            base.OnAddListener();
            NetEvent.Instance.AddEventListener(1201, HandleRolesCreateS2C);
        }

        private void HandleRolesCreateS2C(BufferEntity entity)
        {
            //�����ɫ�Ƿ񴴽��ɹ����߼�
            RolesCreateS2C s2cMSG = ProtobufHelper.FromBytes<RolesCreateS2C>(entity.proto);
            if(s2cMSG.Result==0)
            {
                //�����ɫ
                RolesCtrl.Instance.SaveRolesInfo(s2cMSG.RolesInfo);
                //�رյ�ǰ�������
                Close();
                //�򿪴�������
                WindowManager.Instance.OpenWindow(WindowType.LobbyWindow);
            }
            else
            {
                //��ɫ�Ѿ����� ����ʧ��
                Debug.Log("��ɫ�Ѿ����� ����ʧ��");
                WindowManager.Instance.ShowTips("�Ѿ�������ͬ�Ľ�ɫ��������ʧ��"); 
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
            NetEvent.Instance.RemoveEventListener(1201, HandleRolesCreateS2C);
        }

        protected override void RegisterUIEvent()
        {
            base.RegisterUIEvent();
            for(int i=0;i<buttonList.Length;i++)
            {
                switch(buttonList[i].name)
                {
                    case "StartBtn":
                        buttonList[i].onClick.AddListener(StartBtnOnClick);
                        break;
                    default:
                        break;
                }
            }
        }

        //���������ɫ
        private void StartBtnOnClick()
        {
            RolesCreateC2S c2sMSG = new RolesCreateC2S();
            c2sMSG.NickName = InputField.text;

            Bufferfactory.CreateAndSendpackage(1201, c2sMSG);
        }
    }
}

