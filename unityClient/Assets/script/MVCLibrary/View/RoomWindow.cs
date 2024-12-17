using Game.Ctrl;
using Game.Net;
using Game.View;
using JetBrains.Annotations;
using ProtoMsg;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game.View
{
    public class RoomWindow : BaseWindow
    {
        public RoomWindow()
        {
            selfType = WindowType.RoomWindow;
            scenesType = ScenesType.Login;
            resident = false;
            resName = "UIPrefab/Room/RoomWindow";
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            //回车键
            if(Input.GetKeyDown(KeyCode.Return))
            {
                Bufferfactory.CreateAndSendpackage(1404, new RoomSendMsgC2S()
                {
                    Text = ChatInput.text,
                });
                ChatInput.text = "";
            }
        }
        Transform SkillInfo;
        Text Time,ChatText;
        int time;
        Transform Team_HeroA_item;
        Transform Team_HeroB_item;

        Dictionary<int, GameObject> rolesDIC;

        Scrollbar ChatVertical;

        Image SkillA, SkillB;

        InputField ChatInput;
        protected override void Awake()
        {
            base.Awake();
            //战斗监听器的初始化 防止刚进入战斗场景，还没初始化时，服务器发送的消息无法接收到
            BattleListener.Instance.Init();

            grid = 0;
            isLock = false;
            lockHeroID = 0;
            time = 10;
            playerLoadDIC = new Dictionary<int, GameObject>();
            rolesDIC = new Dictionary<int, GameObject>();

            ChatInput=transform.Find("ChatInput").GetComponent<InputField>();
            SkillInfo = transform.Find("SkillInfo");
            Time = transform.Find("Time").GetComponent<Text>();
            ChatText = transform.Find("ChatBG/Scroll View/Viewport/Content/ChatText").GetComponent<Text>();

            Team_HeroA_item = transform.Find("TeamA/Team_HeroA_item");
            Team_HeroB_item = transform.Find("TeamB/Team_HeroB_item");

            SkillA=transform.Find("SkillA").GetComponent<Image>();
            SkillB = transform.Find("SkillB").GetComponent<Image>();
            //聊天窗的滚动条
            ChatVertical = transform.Find("ChatBG/Scroll View/ChatVertical").GetComponent<Scrollbar>();

            ct = new CancellationTokenSource();
            TimeDown();
            //房间信息
            RoomInfo roomInfo= RolesCtrl.Instance.GetRoomInfo();
            for(int i=0;i<roomInfo.TeamA.Count;i++)
            {
                GameObject go= GameObject.Instantiate(Team_HeroA_item.gameObject, Team_HeroA_item.parent, false);
                go.transform.Find("Hero_NickName").GetComponent<Text>().text = roomInfo.TeamA[i].NickName;
                go.SetActive(true);
                rolesDIC[roomInfo.TeamA[i].RolesID] = go;
            }
            for (int i = 0; i < roomInfo.TeamB.Count; i++)
            {
                GameObject go = GameObject.Instantiate(Team_HeroB_item.gameObject, Team_HeroB_item.parent, false);
                go.transform.Find("Hero_NickName").GetComponent<Text>().text = roomInfo.TeamB[i].NickName;
                go.SetActive(true);
                rolesDIC[roomInfo.TeamB[i].RolesID] = go;
            }
        }

        CancellationTokenSource ct;//用于取消线程
        async void TimeDown()
        {
            while(time>0)
            {
                await Task.Delay(1000);//每隔一秒
                if(!ct.IsCancellationRequested)
                {
                    time -= 1;
                    Time.text = $"倒计时{time}";
                }
            }
        }

        protected override void OnAddListener()
        {
            base.OnAddListener();
            NetEvent.Instance.AddEventListener(1400, HandleRoomSelectHeroS2C);
            NetEvent.Instance.AddEventListener(1401, HandleRoomSelectHeroSkillS2C);
            NetEvent.Instance.AddEventListener(1403, HandleRoomCloseS2C);
            NetEvent.Instance.AddEventListener(1404, HandleRoomSendMsgS2C);
            NetEvent.Instance.AddEventListener(1405, HandleRoomLockHeroS2C);
            NetEvent.Instance.AddEventListener(1406, HandleRoomLoadProgressS2C);
            NetEvent.Instance.AddEventListener(1407, HandleRoomToBattleS2C);
        }

        Dictionary<int, GameObject> playerLoadDIC;
        Transform HeroA_item, HeroB_item;
        //加载进入战斗
        private void HandleRoomToBattleS2C(BufferEntity entity)
        {
            RoomToBattleS2C s2cMSG = ProtobufHelper.FromBytes<RoomToBattleS2C>(entity.proto);
            RoomCtrl.Instance.SavePlayerList(s2cMSG.PlayerList);

            transform.Find("LoadBG").gameObject.SetActive(true);
            HeroA_item = transform.Find("LoadBG/L_TeamA/HeroA_item");
            HeroB_item = transform.Find("LoadBG/L_TeamB/HeroB_item");


            for (int i=0;i<s2cMSG.PlayerList.Count;i++)
            {
                GameObject go;
                //A队伍
                if (s2cMSG.PlayerList[i].TeamID==0)
                {
                    go = GameObject.Instantiate(HeroA_item.gameObject, HeroA_item.parent, false);
                }
                //B队伍
                else
                {
                    go = GameObject.Instantiate(HeroB_item.gameObject, HeroB_item.parent, false);
                }

                go.transform.GetComponent<Image>().sprite
                    = Resmanager.Instance.LoadHeroTexture(s2cMSG.PlayerList[i].HeroID);
                go.transform.Find("NickName").GetComponent<Text>().text
                    = s2cMSG.PlayerList[i].RolesInfo.NickName;
                go.transform.Find("SkillA").GetComponent<Image>().sprite
                    = Resmanager.Instance.LoadGeneralSkill(s2cMSG.PlayerList[i].SkillA);
                go.transform.Find("SkillB").GetComponent<Image>().sprite
                    = Resmanager.Instance.LoadGeneralSkill(s2cMSG.PlayerList[i].SkillB);
                go.transform.Find("Progress").GetComponent<Text>().text
                    = "0%";

                go.gameObject.SetActive(true);
                //缓存克隆出来的游戏物体 更新进度
                playerLoadDIC[s2cMSG.PlayerList[i].RolesInfo.RolesID] = go;
            }

            async= SceneManager.LoadSceneAsync("Level01");
            async.allowSceneActivation = false;//不激活场景

            //定时的去发送加载进度
            SendProgeress();

        }
        AsyncOperation async;

        async void SendProgeress()
        {
            Bufferfactory.CreateAndSendpackage(1406, new RoomLoadProgressC2S()
            {
                LoadProgress = (int)(async.progress >= 0.89f?100:async.progress * 100)
            });
            await Task.Delay(500);
            if(ct.IsCancellationRequested==true)
            {
                return;
            }
            SendProgeress();
        }

        //加载进度
        private void HandleRoomLoadProgressS2C(BufferEntity entity)
        {
            RoomLoadProgressS2C s2cMSG = ProtobufHelper.FromBytes<RoomLoadProgressS2C>(entity.proto);
            //更新界面
            if(s2cMSG.IsBattleStart==true)
            {
                ct.Cancel();
                for(int i=0;i<s2cMSG.RolesID.Count;i++)
                {
                    playerLoadDIC[s2cMSG.RolesID[i]].transform.Find("Progress")
                        .GetComponent<Text>().text = "100%";
                }
                async.allowSceneActivation = true;
                Close();
            }
            else
            {
                //如果还不能进入战斗场景
                for (int i = 0; i < s2cMSG.RolesID.Count; i++)
                {
                    playerLoadDIC[s2cMSG.RolesID[i]].transform.Find("Progress")
                        .GetComponent<Text>().text = $"{s2cMSG.LoadProgress[i]}%";
                }
            }
        }


        //锁定英雄的协议
        private void HandleRoomLockHeroS2C(BufferEntity entity)
        {
            RoomLockHeroS2C s2cMSG = ProtobufHelper.FromBytes<RoomLockHeroS2C>(entity.proto);
            rolesDIC[s2cMSG.RolesID].transform.Find("Hero_State").GetComponent<Text>().text =
                "已锁定";
            Debug.Log("LockHero");
            if(RoomCtrl.Instance.CheckIsSelfRoles(s2cMSG.RolesID))
            {
                isLock = true;//已经锁定英雄
            }

        }
        //发送聊天信息
        private void HandleRoomSendMsgS2C(BufferEntity entity)
        {
            RoomSendMsgS2C s2cMSG = ProtobufHelper.FromBytes<RoomSendMsgS2C>(entity.proto);
            ChatText.text += $"{RoomCtrl.Instance.GetNickName(s2cMSG.RolesID)}:{s2cMSG.Text}\n";
            ChatVertical.value = 0;

        }
        //解散房间
        private void HandleRoomCloseS2C(BufferEntity entity)
        {
            RoomCloseS2C s2cMSG = ProtobufHelper.FromBytes<RoomCloseS2C>(entity.proto);

            Close();
            RoomCtrl.Instance.RemoveRoomInfo();

            WindowManager.Instance.OpenWindow(WindowType.LobbyWindow);
        }
        //选择召唤师技能
        private void HandleRoomSelectHeroSkillS2C(BufferEntity entity)
        {
            RoomSelectHeroSkillS2C s2cMSG = ProtobufHelper.FromBytes<RoomSelectHeroSkillS2C>(entity.proto);
            if(s2cMSG.GridID==0)
            {
                rolesDIC[s2cMSG.RolesID].transform.Find("Hero_SkillA").GetComponent<Image>().sprite
                    = Resmanager.Instance.LoadGeneralSkill(s2cMSG.SkillID);
                if (RoomCtrl.Instance.CheckIsSelfRoles(s2cMSG.RolesID))
                {
                    SkillA.sprite = Resmanager.Instance.LoadGeneralSkill(s2cMSG.SkillID);
                    //关闭技能选择面板
                    SkillInfo.gameObject.SetActive(false);
                }
            }
            else
            {
                rolesDIC[s2cMSG.RolesID].transform.Find("Hero_SkillB").GetComponent<Image>().sprite
                   = Resmanager.Instance.LoadGeneralSkill(s2cMSG.SkillID);
                if (RoomCtrl.Instance.CheckIsSelfRoles(s2cMSG.RolesID))
                {
                    SkillB.sprite = Resmanager.Instance.LoadGeneralSkill(s2cMSG.SkillID);
                    SkillInfo.gameObject.SetActive(false);
                }
            }
        }
        //选择了英雄的头像，更新头像
        private void HandleRoomSelectHeroS2C(BufferEntity entity)
        {
            Debug.Log($"选择的英雄是");
            RoomSelectHeroS2C s2cMSG = ProtobufHelper.FromBytes<RoomSelectHeroS2C>(entity.proto);
            Debug.Log($"{s2cMSG.HeroID}");
            rolesDIC[s2cMSG.RolesID].transform.Find("Hero_Head").GetComponent<Image>().sprite
                     = Resmanager.Instance.LoadRoundHead(s2cMSG.HeroID.ToString());
            if(RoomCtrl.Instance.CheckIsSelfRoles(s2cMSG.RolesID))
            {
                //lockHeroID 缓存当前选择的英雄ID
                lockHeroID = s2cMSG.HeroID;
            }
            
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            ct.Cancel();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override void OnRemoveListener()
        {
            base.OnRemoveListener();
            NetEvent.Instance.RemoveEventListener(1400, HandleRoomSelectHeroS2C);
            NetEvent.Instance.RemoveEventListener(1401, HandleRoomSelectHeroSkillS2C);
            NetEvent.Instance.RemoveEventListener(1403, HandleRoomCloseS2C);
            NetEvent.Instance.RemoveEventListener(1404, HandleRoomSendMsgS2C);
            NetEvent.Instance.RemoveEventListener(1405, HandleRoomLockHeroS2C);
            NetEvent.Instance.RemoveEventListener(1406, HandleRoomLoadProgressS2C);
            NetEvent.Instance.RemoveEventListener(1407, HandleRoomToBattleS2C);
        }

        int lockHeroID;
        bool isLock;//是否已经锁定英雄
        protected override void RegisterUIEvent()
        {
            base.RegisterUIEvent();
            for(int i=0;i<buttonList.Length;i++)
            {
                switch(buttonList[i].name)
                {
                    case "Hero1001":
                        SendSelectHero(buttonList[i], 1001);
                        break;
                    case "Hero1002":
                        SendSelectHero(buttonList[i], 1002);
                        break;
                    case "Hero1003":
                        SendSelectHero(buttonList[i], 1003);
                        break;
                    case "Hero1004":
                        SendSelectHero(buttonList[i], 1004);
                        break;
                    case "Hero1005":
                        SendSelectHero(buttonList[i], 1005);
                        break;
                    case "Lock":
                        buttonList[i].onClick.AddListener(() =>
                        {
                            if (isLock == false)
                            {
                                if(lockHeroID==0)
                                {
                                    Debug.Log("请先选择英雄再锁定");
                                    return;
                                }
                                isLock = true;//避免重复发送锁定报文给服务器
                                Bufferfactory.CreateAndSendpackage(1405, new RoomLockHeroC2S() { HeroID=lockHeroID});
                            }
                        });
                        break;
                    case "SkillA":
                        buttonList[i].onClick.AddListener(SkillAOnClick);
                        break;
                    case "SkillB":
                        buttonList[i].onClick.AddListener(SkillBOnClick);
                        break;
                    case "chuansong":
                        SendSelectSkill(buttonList[i], 102);
                        break;
                    case "dianran":
                        SendSelectSkill(buttonList[i], 103);
                        break;
                    case "jinghua":
                        SendSelectSkill(buttonList[i], 104);
                        break;
                    case "pnigzhang":
                        SendSelectSkill(buttonList[i], 105);
                        break;
                    case "xuruo":
                        SendSelectSkill(buttonList[i], 107);
                        break;
                    case "zhiliao":
                        SendSelectSkill(buttonList[i], 108);
                        break;
                    case "chengjie":
                        SendSelectSkill(buttonList[i], 101);
                        break;
                    case "shanxian":
                        SendSelectSkill(buttonList[i], 106);
                        break;
                    default:
                        break;
                }
            }
        }

        private void SendSelectSkill(Button btn, int skillid)
        {
            btn.onClick.AddListener(() =>
            {
                if (isLock == false)
                {
                    Bufferfactory.CreateAndSendpackage(1401, new RoomSelectHeroSkillC2S()
                    {
                        SkillID = skillid,
                        GridID = grid
                    });
                }
            });
        }

        private void SkillBOnClick()
        {
            grid = 1;
            //打开技能选择面板
            SkillInfo.gameObject.SetActive(true);
        }
        int grid;
        private void SkillAOnClick()
        {
            grid = 0;
            SkillInfo.gameObject.SetActive(true);
        }
        //发送选择的英雄
        private void SendSelectHero(Button btn,int heroID)
        {
            
            btn.onClick.AddListener(() =>
            {
                if (isLock == false)
                {
                    Debug.Log("selcet");
                    Bufferfactory.CreateAndSendpackage(1400, new RoomSelectHeroC2S { HeroID = heroID });
                }
            });
        }
    }
}

