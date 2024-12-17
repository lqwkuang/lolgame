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
            //昵称 段位 金币 钻石
            RolesName = transform.Find("LobbyBG/RolesName").GetComponent<Text>();
            Duan = transform.Find("LobbyBG/Duan").GetComponent<Text>();
            GoldCount = transform.Find("LobbyBG/GoldCount").GetComponent<Text>();
            DiamondsCount = transform.Find("LobbyBG/DiamondsCount").GetComponent<Text>();
            //匹配按钮 排位按钮
            MatchModeBtn = transform.Find("LobbyBG/MatchModeBtn");
            QualifyingBtn = transform.Find("LobbyBG/QualifyingBtn");
            StopMatchBtn = transform.Find("LobbyBG/StopMatchBtn");
            //提示
            MatchTips = transform.Find("LobbyBG/MatchTips").GetComponent<Text>();
        }

        protected override void OnAddListener()
        {
            base.OnAddListener();
            NetEvent.Instance.AddEventListener(1300, HandleLobbyToMatchS2C);
            NetEvent.Instance.AddEventListener(1301, HandleLobbyUpdateMatchStateS2C);
            NetEvent.Instance.AddEventListener(1302, HandleLobbyQuitMatchS2C);
        }
        //退出匹配结果
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
        //更新匹配状态
        private void HandleLobbyUpdateMatchStateS2C(BufferEntity entity)
        {
            LobbyUpdateMatchStateS2C s2cMSG = ProtobufHelper.FromBytes<LobbyUpdateMatchStateS2C>(entity.proto);
            if(s2cMSG.Result==0)
            {
                MatchModeBtn.gameObject.SetActive(true);
                QualifyingBtn.gameObject.SetActive(true);
                StopMatchBtn.gameObject.SetActive(false);
                MatchTips.gameObject.SetActive(false);

                //房间信息
                RolesCtrl.Instance.SaveRoomInfo(s2cMSG.RoomInfo);
                Close();
                WindowManager.Instance.OpenWindow(WindowType.RoomWindow);
                
            }
            else
            {

            }
        }
        //进入匹配的结果
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
                //无法进行匹配 可能是被惩罚
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            //获取到角色信息进行更新
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
        //停止匹配
        private void StopMatchBtnOnClick()
        {
            Debug.Log("------暂停匹配");
            Bufferfactory.CreateAndSendpackage(1302, new LobbyQuitMatchC2S());
        }

        //点击匹配按钮
        private void MatchModeBtnOnClick()
        {
            Bufferfactory.CreateAndSendpackage(1300, new LobbyToMatchC2S());
        }
    }
}

