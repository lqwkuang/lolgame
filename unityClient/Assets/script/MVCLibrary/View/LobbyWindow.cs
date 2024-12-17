using Game.Net;
using ProtoMsg;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.View
{
    public class LobbyWindow : BaseWindow
    {
        public LobbyWindow()
        {
            selfType = WindowType.LobbyWindow;
            scenesType = ScenesType.Login;
            resident = false;
            resName = "UIPrefab/Lobby/LobbyWindow";
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
        }

        Transform MatchModeBtn, QualifyingBtn,StopMatchBtn;
        Text RolesName, Duan, GoldCount, DiamondsCount,MatchTips;
        protected override void Awake()
        {
            base.Awake();
            //�ǳ� ��λ ��� ��ʯ
            RolesName = transform.Find("LobbyBG/RolesName").GetComponent<Text>();
            Duan = transform.Find("LobbyBG/Duan").GetComponent<Text>();
            GoldCount = transform.Find("LobbyBG/GoldCount").GetComponent<Text>();
            DiamondsCount = transform.Find("LobbyBG/DiamondsCount").GetComponent<Text>();
            //ƥ�䰴ť ��λ��ť
            MatchModeBtn = transform.Find("LobbyBG/MatchModeBtn");
            QualifyingBtn = transform.Find("LobbyBG/QualifyingBtn");
            StopMatchBtn = transform.Find("LobbyBG/StopMatchBtn");
            //��ʾ
            MatchTips = transform.Find("LobbyBG/MatchTips").GetComponent<Text>();
        }

        protected override void OnAddListener()
        {
            base.OnAddListener();
            NetEvent.Instance.AddEventListener(1300, HandleLobbyToMatchS2C);
            NetEvent.Instance.AddEventListener(1301, HandleLobbyUpdateMatchStateS2C);
            NetEvent.Instance.AddEventListener(1302, HandleLobbyQuitMatchS2C);
        }
        //�˳�ƥ����
        private void HandleLobbyQuitMatchS2C(BufferEntity entity)
        {
            LobbyQuitMatchS2C s2cMSG = ProtobufHelper.FromBytes<LobbyQuitMatchS2C>(entity.proto);
            if(s2cMSG.Result==0)
            {
                MatchModeBtn.gameObject.SetActive(true);
                QualifyingBtn.gameObject.SetActive(true);

                StopMatchBtn.gameObject.SetActive(false);
                MatchTips.gameObject.SetActive(false);
            }
        }
        //����ƥ��״̬
        private void HandleLobbyUpdateMatchStateS2C(BufferEntity entity)
        {
            LobbyUpdateMatchStateS2C s2cMSG = ProtobufHelper.FromBytes<LobbyUpdateMatchStateS2C>(entity.proto);
            if(s2cMSG.Result==0)
            {
                MatchModeBtn.gameObject.SetActive(true);
                QualifyingBtn.gameObject.SetActive(true);
                StopMatchBtn.gameObject.SetActive(false);
                MatchTips.gameObject.SetActive(false);

                //������Ϣ
                RolesCtrl.Instance.SaveRoomInfo(s2cMSG.RoomInfo);
                Close();
                WindowManager.Instance.OpenWindow(WindowType.RoomWindow);
                
            }
            else
            {

            }
        }
        //����ƥ��Ľ��
        private void HandleLobbyToMatchS2C(BufferEntity entity)
        {
            LobbyToMatchS2C s2cMSG = ProtobufHelper.FromBytes<LobbyToMatchS2C>(entity.proto);
            if(s2cMSG.Result==0)
            {
                MatchModeBtn.gameObject.SetActive(false);
                QualifyingBtn.gameObject.SetActive(false);
                StopMatchBtn.gameObject.SetActive(true);
                MatchTips.gameObject.SetActive(true);
            }
            else
            {
                //�޷�����ƥ�� �����Ǳ��ͷ�
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            //��ȡ����ɫ��Ϣ���и���
            RolesInfo roles= RolesCtrl.Instance.GetRolesInfo();
            RolesName.text = roles.NickName;
            Duan.text = roles.VictoryPoint.ToString();
            GoldCount.text = roles.GoldCoin.ToString();
            DiamondsCount.text = roles.Diamonds.ToString();
        }

        protected override void OnRemoveListener()
        {
            base.OnRemoveListener();
            NetEvent.Instance.RemoveEventListener(1300, HandleLobbyToMatchS2C);
            NetEvent.Instance.RemoveEventListener(1301, HandleLobbyUpdateMatchStateS2C);
            NetEvent.Instance.RemoveEventListener(1302, HandleLobbyQuitMatchS2C);
        }

        protected override void RegisterUIEvent()
        {
            base.RegisterUIEvent();
            for(int i=0;i<buttonList.Length;i++)
            {
                switch(buttonList[i].name)
                {
                    case "MatchModeBtn":
                        buttonList[i].onClick.AddListener(MatchModeBtnOnClick);
                        break;
                    case "StopMatchBtn":
                        buttonList[i].onClick.AddListener(StopMatchBtnOnClick);
                        break;
                    default:
                        break;
                }
            }
        }
        //ֹͣƥ��
        private void StopMatchBtnOnClick()
        {
            Debug.Log("------��ͣƥ��");
            Bufferfactory.CreateAndSendpackage(1302, new LobbyQuitMatchC2S());
        }

        //���ƥ�䰴ť
        private void MatchModeBtnOnClick()
        {
            Bufferfactory.CreateAndSendpackage(1300, new LobbyToMatchC2S());
        }
    }
}

