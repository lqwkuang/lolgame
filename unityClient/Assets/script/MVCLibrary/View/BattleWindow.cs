using Game.Ctrl;
using ProtoMsg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.View
{
    public class BattleWindow :BaseWindow
    {
        Playerbag playerbag;
       public BattleWindow()
       {
            selfType = WindowType.BattleWindow;
            scenesType = ScenesType.Battle;
            resident = false;
            resName = "UIPrefab/Battle/BattleWindow";
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            UpdateSkill();
        }
        Image skillQ, skillW, skillE, skillR, skillP, skillD, skillF;
        Image fillQ, fillW, fillE, fillR, fillP, fillD, fillF;
        public Image equip1, equip2, equip3, equip4, equip5, equip6;
        public Text playerMoney;

        Text Level, Armor, Attack, MagicResistance, Crit, MoveSpeed;
        Text HP, MP;
        Image fillHP, fillMP;
        PlayerInfo playerInfo;
        PlayerCtrl playerCtrl;//自己的控制器
        SkillManager skillManager;//自己的技能管理
        protected override void Awake()
        {
            base.Awake();
            playerCtrl = RoomCtrl.Instance.GetSelfPlayerCtrl();
            playerbag = playerCtrl.gameObject.GetComponent<Playerbag>();
            playerInfo = playerCtrl.playerInfo;
            skillManager = playerCtrl.skillManager;
            skillQ = transform.Find("Bottom/SkillInfo/SkillQ").GetComponent<Image>();
            skillW = transform.Find("Bottom/SkillInfo/SkillW").GetComponent<Image>();
            skillE = transform.Find("Bottom/SkillInfo/SkillE").GetComponent<Image>();
            skillR = transform.Find("Bottom/SkillInfo/SkillR").GetComponent<Image>();
            skillP = transform.Find("Bottom/SkillInfo/SkillP").GetComponent<Image>();

            skillD = transform.Find("Bottom/SkillInfo/SkillD").GetComponent<Image>();
            skillF = transform.Find("Bottom/SkillInfo/SkillF").GetComponent<Image>();

            fillQ = transform.Find("Bottom/SkillInfo/SkillQ/Fill").GetComponent<Image>();
            fillW = transform.Find("Bottom/SkillInfo/SkillW/Fill").GetComponent<Image>();
            fillE = transform.Find("Bottom/SkillInfo/SkillE/Fill").GetComponent<Image>();
            fillR = transform.Find("Bottom/SkillInfo/SkillR/Fill").GetComponent<Image>();
            fillP = transform.Find("Bottom/SkillInfo/SkillP/Fill").GetComponent<Image>();

            fillD = transform.Find("Bottom/SkillInfo/SkillD/Fill").GetComponent<Image>();
            fillF = transform.Find("Bottom/SkillInfo/SkillF/Fill").GetComponent<Image>();

            //装备格子
            equip1 = transform.Find("Bottom/Equipment/1/Fill").GetComponent<Image>();
            equip2 = transform.Find("Bottom/Equipment/2/Fill").GetComponent<Image>();
            equip3 = transform.Find("Bottom/Equipment/3/Fill").GetComponent<Image>();
            equip4 = transform.Find("Bottom/Equipment/5/Fill").GetComponent<Image>();
            equip5 = transform.Find("Bottom/Equipment/6/Fill").GetComponent<Image>();
            equip6 = transform.Find("Bottom/Equipment/7/Fill").GetComponent<Image>();
            //钱
            playerMoney = transform.Find("Bottom/Equipment/Gold/Text").GetComponent<Text>();


            HP = transform.Find("Bottom/SkillInfo/HP/Text").GetComponent<Text>();
            fillHP = transform.Find("Bottom/SkillInfo/HP/Fill").GetComponent<Image>();
            MP = transform.Find("Bottom/SkillInfo/MP/Text").GetComponent<Text>();
            fillMP = transform.Find("Bottom/SkillInfo/MP/Fill").GetComponent<Image>();

            skillQ.sprite = Resmanager.Instance.LoadHeroSkill(playerInfo.HeroID,"q");
            skillW.sprite = Resmanager.Instance.LoadHeroSkill(playerInfo.HeroID, "w");
            skillE.sprite = Resmanager.Instance.LoadHeroSkill(playerInfo.HeroID, "e");
            skillR.sprite = Resmanager.Instance.LoadHeroSkill(playerInfo.HeroID, "r");
            skillP.sprite = Resmanager.Instance.LoadHeroSkill(playerInfo.HeroID, "p");

            //D F 通用技能的配置
            skillD.sprite = Resmanager.Instance.LoadGeneralSkill(playerInfo.SkillA);
            skillF.sprite = Resmanager.Instance.LoadGeneralSkill(playerInfo.SkillB);

            //遮罩
            fillQ.sprite = Resmanager.Instance.LoadHeroSkill(playerInfo.HeroID, "q_cooling");
            fillW.sprite = Resmanager.Instance.LoadHeroSkill(playerInfo.HeroID, "w_cooling");
            fillE.sprite = Resmanager.Instance.LoadHeroSkill(playerInfo.HeroID, "e_cooling");
            fillR.sprite = Resmanager.Instance.LoadHeroSkill(playerInfo.HeroID, "r_cooling");
            fillP.sprite = Resmanager.Instance.LoadHeroSkill(playerInfo.HeroID, "p_cooling");

            var mAttribute = playerCtrl.totalAttribute;

            Level = transform.Find("Bottom/SelfAttribute/Info/Level/Text").GetComponent<Text>();
            Armor = transform.Find("Bottom/SelfAttribute/Info/Armor/Text").GetComponent<Text>();
            Attack = transform.Find("Bottom/SelfAttribute/Info/Attack/Text").GetComponent<Text>();
            MagicResistance = transform.Find("Bottom/SelfAttribute/Info/MagicResistance/Text").GetComponent<Text>();
            Crit = transform.Find("Bottom/SelfAttribute/Info/Crit/Text").GetComponent<Text>();
            MoveSpeed = transform.Find("Bottom/SelfAttribute/Info/MoveSpeed/Text").GetComponent<Text>();

            UpateAttrature();
        }

        Transform selectHead;
        Text select_Level, select_HP, select_Attack, select_MagicResistance, select_Crit, select_MoveSpeed, select_KillText, select_KillSoldier;
        Image select_Head, select_HPFill, select_MPFill;
        Image select_equipment1, select_equipment2, select_equipment3, select_equipment4, select_equipment5, select_equipment6, select_equipment7;
        //显示点击到的物体信息
        public void ShowSelectObjectInfo(GameObject p)
        {
            if (selectHead == null)
            {
                selectHead = transform.Find("SelectHead");

                //属性
                select_Level = selectHead.Find("Attribute/Info/Level/Text").GetComponent<Text>();
                select_HP = selectHead.Find("Attribute/Info/HP/Text").GetComponent<Text>();
                select_Attack = selectHead.Find("Attribute/Info/Attack/Text").GetComponent<Text>();
                select_MagicResistance = selectHead.Find("Attribute/Info/MagicResistance/Text").GetComponent<Text>();
                select_Crit = selectHead.Find("Attribute/Info/Crit/Text").GetComponent<Text>();
                select_MoveSpeed = selectHead.Find("Attribute/Info/MoveSpeed/Text").GetComponent<Text>();

                //头像_图片
                select_Head = selectHead.Find("Head").GetComponent<Image>();
                //血量
                select_HPFill = selectHead.Find("HP/Fill").GetComponent<Image>();
                select_MPFill = selectHead.Find("MP/Fill").GetComponent<Image>();

                //装备 1234567
                select_equipment1 = selectHead.Find("Equipment/1").GetComponent<Image>();
                select_equipment2 = selectHead.Find("Equipment/2").GetComponent<Image>();
                select_equipment3 = selectHead.Find("Equipment/3").GetComponent<Image>();
                select_equipment4 = selectHead.Find("Equipment/4").GetComponent<Image>();
                select_equipment5 = selectHead.Find("Equipment/5").GetComponent<Image>();
                select_equipment6 = selectHead.Find("Equipment/6").GetComponent<Image>();
                select_equipment7 = selectHead.Find("Equipment/7").GetComponent<Image>();
                //战绩 击杀/死亡/助攻
                select_KillText = selectHead.Find("KillInfo/Text").GetComponent<Text>();
                //击杀的小兵数量
                select_KillSoldier = selectHead.Find("KillSoldier/Text").GetComponent<Text>();
            }

            PlayerCtrl playerCtrl= p.transform.GetComponent<PlayerCtrl>();
            HeroAttributeEntity heroAttribute = playerCtrl.totalAttribute;

            //更新属性面板
            select_Level.text = heroAttribute.Level.ToString() + "级";
            select_HP.text = heroAttribute.HP.ToString();
            select_Attack.text = heroAttribute.Power.ToString();
            select_MagicResistance.text = heroAttribute.MagicResistance.ToString();
            select_Crit.text = heroAttribute.Crit.ToString();
            select_MoveSpeed.text = heroAttribute.MoveSpeed.ToString();

            select_Head.sprite = Resmanager.Instance.LoadHeadIcon(playerCtrl.playerInfo.HeroID);
            HeroAttributeEntity heroCurrentAttribute = playerCtrl.totalAttribute;

            select_HPFill.fillAmount = heroCurrentAttribute.HP / heroAttribute.HP;
            select_MPFill.fillAmount = heroCurrentAttribute.MP / heroAttribute.MP;

            //装备图片 现在没有 TODO
            select_equipment1.sprite = Resmanager.Instance.LoadHeroSkill(playerInfo.HeroID, "q");
            select_equipment2.sprite = Resmanager.Instance.LoadHeroSkill(playerInfo.HeroID, "w");
            select_equipment3.sprite = Resmanager.Instance.LoadHeroSkill(playerInfo.HeroID, "e");
            select_equipment4.sprite = Resmanager.Instance.LoadHeroSkill(playerInfo.HeroID, "r");
            select_equipment5.sprite = Resmanager.Instance.LoadHeroSkill(playerInfo.HeroID, "p");
            select_equipment6.sprite = Resmanager.Instance.LoadHeroSkill(playerInfo.HeroID, "p");
            select_equipment7.sprite = Resmanager.Instance.LoadHeroSkill(playerInfo.HeroID, "p");

            selectHead.gameObject.SetActive(true);
        }
        //更新自身的技能显示 以及金币
        public void UpdateSkill()
        {
            fillQ.fillAmount = skillManager.SurplusTimeprecent(KeyCode.Q);      
            fillW.fillAmount = skillManager.SurplusTimeprecent(KeyCode.W);       
            fillE.fillAmount = skillManager.SurplusTimeprecent(KeyCode.E);        
            fillR.fillAmount = skillManager.SurplusTimeprecent(KeyCode.R);          
            fillD.fillAmount = skillManager.SurplusTimeprecent(KeyCode.D);            
            fillF.fillAmount = skillManager.SurplusTimeprecent(KeyCode.F);
            //金币
            playerMoney.text = playerbag.GetplayerMoney().ToString();
        }
        //更新英雄属性 hp mp 移速。。。
        public void UpateAttrature()
        {
            HeroAttributeEntity curAttr=playerCtrl.currentAttribute;
            HeroAttributeEntity totalAttr=playerCtrl.totalAttribute;
            HP.text = curAttr.HP.ToString()+"/"+totalAttr.HP.ToString();
            fillHP.fillAmount = curAttr.HP / totalAttr.HP;
            MP.text = curAttr.MP.ToString() + "/" + totalAttr.MP.ToString();
            fillMP.fillAmount = curAttr.MP / totalAttr.MP;


            Level.text = totalAttr.Level.ToString() + "级";
            Attack.text = totalAttr.Power.ToString();
            MagicResistance.text = totalAttr.MagicResistance.ToString();
            Armor.text = totalAttr.Armor.ToString();
            Crit.text = totalAttr.Crit.ToString();
            MoveSpeed.text = totalAttr.MoveSpeed.ToString();
        }

        protected override void OnAddListener()
        {
            base.OnAddListener();
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
            for(int i=0;i<buttonList.Length;i++)
            {
                switch(buttonList[i].name)
                {
                    case "Gold":
                        buttonList[i].onClick.AddListener(() => {
                            WindowManager.Instance.OpenWindow(WindowType.StoreWindow);
                        });
                        break;
                }
            }
        }
    }

}
