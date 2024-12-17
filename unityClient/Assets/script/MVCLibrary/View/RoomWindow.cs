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
            //�س���
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
            //ս���������ĳ�ʼ�� ��ֹ�ս���ս����������û��ʼ��ʱ�����������͵���Ϣ�޷����յ�
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
            //���촰�Ĺ�����
            ChatVertical = transform.Find("ChatBG/Scroll View/ChatVertical").GetComponent<Scrollbar>();

            ct = new CancellationTokenSource();
            TimeDown();
            //������Ϣ
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

        CancellationTokenSource ct;//����ȡ���߳�
        async void TimeDown()
        {
            while(time>0)
            {
                await Task.Delay(1000);//ÿ��һ��
                if(!ct.IsCancellationRequested)
                {
                    time -= 1;
                    Time.text = $"����ʱ{time}";
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
        //���ؽ���ս��
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
                //A����
                if (s2cMSG.PlayerList[i].TeamID==0)
                {
                    go = GameObject.Instantiate(HeroA_item.gameObject, HeroA_item.parent, false);
                }
                //B����
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
                //�����¡��������Ϸ���� ���½���
                playerLoadDIC[s2cMSG.PlayerList[i].RolesInfo.RolesID] = go;
            }

            async= SceneManager.LoadSceneAsync("Level01");
            async.allowSceneActivation = false;//�������

            //��ʱ��ȥ���ͼ��ؽ���
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

        //���ؽ���
        private void HandleRoomLoadProgressS2C(BufferEntity entity)
        {
            RoomLoadProgressS2C s2cMSG = ProtobufHelper.FromBytes<RoomLoadProgressS2C>(entity.proto);
            //���½���
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
                //��������ܽ���ս������
                for (int i = 0; i < s2cMSG.RolesID.Count; i++)
                {
                    playerLoadDIC[s2cMSG.RolesID[i]].transform.Find("Progress")
                        .GetComponent<Text>().text = $"{s2cMSG.LoadProgress[i]}%";
                }
            }
        }


        //����Ӣ�۵�Э��
        private void HandleRoomLockHeroS2C(BufferEntity entity)
        {
            RoomLockHeroS2C s2cMSG = ProtobufHelper.FromBytes<RoomLockHeroS2C>(entity.proto);
            rolesDIC[s2cMSG.RolesID].transform.Find("Hero_State").GetComponent<Text>().text =
                "������";
            Debug.Log("LockHero");
            if(RoomCtrl.Instance.CheckIsSelfRoles(s2cMSG.RolesID))
            {
                isLock = true;//�Ѿ�����Ӣ��
            }

        }
        //����������Ϣ
        private void HandleRoomSendMsgS2C(BufferEntity entity)
        {
            RoomSendMsgS2C s2cMSG = ProtobufHelper.FromBytes<RoomSendMsgS2C>(entity.proto);
            ChatText.text += $"{RoomCtrl.Instance.GetNickName(s2cMSG.RolesID)}:{s2cMSG.Text}\n";
            ChatVertical.value = 0;

        }
        //��ɢ����
        private void HandleRoomCloseS2C(BufferEntity entity)
        {
            RoomCloseS2C s2cMSG = ProtobufHelper.FromBytes<RoomCloseS2C>(entity.proto);

            Close();
            RoomCtrl.Instance.RemoveRoomInfo();

            WindowManager.Instance.OpenWindow(WindowType.LobbyWindow);
        }
        //ѡ���ٻ�ʦ����
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
                    //�رռ���ѡ�����
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
        //ѡ����Ӣ�۵�ͷ�񣬸���ͷ��
        private void HandleRoomSelectHeroS2C(BufferEntity entity)
        {
            Debug.Log($"ѡ���Ӣ����");
            RoomSelectHeroS2C s2cMSG = ProtobufHelper.FromBytes<RoomSelectHeroS2C>(entity.proto);
            Debug.Log($"{s2cMSG.HeroID}");
            rolesDIC[s2cMSG.RolesID].transform.Find("Hero_Head").GetComponent<Image>().sprite
                     = Resmanager.Instance.LoadRoundHead(s2cMSG.HeroID.ToString());
            if(RoomCtrl.Instance.CheckIsSelfRoles(s2cMSG.RolesID))
            {
                //lockHeroID ���浱ǰѡ���Ӣ��ID
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
        bool isLock;//�Ƿ��Ѿ�����Ӣ��
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
                                    Debug.Log("����ѡ��Ӣ��������");
                                    return;
                                }
                                isLock = true;//�����ظ������������ĸ�������
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
            //�򿪼���ѡ�����
            SkillInfo.gameObject.SetActive(true);
        }
        int grid;
        private void SkillAOnClick()
        {
            grid = 0;
            SkillInfo.gameObject.SetActive(true);
        }
        //����ѡ���Ӣ��
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

