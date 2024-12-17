using Game.Model;
using ProtoMsg;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Game.View;
using FogOfWar.CPU;

public class PlayerCtrl : MonoBehaviour
{
    public SkillManager skillManager;
    public AnimatorManager animatorManager;
    public PlayerFSM playerFSM;
    public BattleWindow battleWindow;
    public Playerbag playerbag;
    public baseHeroSkill Heroskill;

    Vector3 spawnPosition;
    Vector3 spawnRotation;
    public PlayerInfo playerInfo;
    public bool isSelf = false;
    public int HeroID;//英雄编号
    public HeroAttributeEntity currentAttribute;
    public HeroAttributeEntity totalAttribute;

    public GameObject HUD;
    public GameObject minmapIcon;//小地图头像
    Vector3 hudOffset = new Vector3(0, 3.1f, 0);

    Text HPText, MPText,LevelText,NickNameText;
    Image HPFill, MPFill;

    //自身战斗状态属性 霸体 免伤 减伤 减速 伏地 经验等级 等等
    public int experience;//经验
    public int totalexperience;//总经验满了就是升一级
    public bool isSuperArmor;//是否霸体
    internal void Init(PlayerInfo item)
    {
        this.playerInfo = item;
        //英雄不是自己的角色
        isSelf= PlayerModel.Instance.CheckIsSelf(item.RolesInfo.RolesID);
        if(isSelf)
        {
            //this.gameObject.layer = LayerMask.NameToLayer("UI");
            this.transform.AddComponent<VisualFieldObject>();
        }
        //小地图头像
        minmapIcon = Resmanager.Instance.LoadMinmapIcon($"{item.HeroID}Mapicon");
        minmapIcon.GetComponent<Canvas>().worldCamera = GameObject.Find("mapCamera").GetComponent<Camera>();
        minmapIcon.transform.position = transform.position + new Vector3(0, 2, 0);
        minmapIcon.transform.SetParent(transform);
        //得到英雄编号
        HeroID = item.HeroID;
        //复活的时候的位置
        spawnPosition = transform.position;
        spawnRotation = transform.eulerAngles;
        //获取它的属性 当前的属性 还有总的属性
        currentAttribute=HeroAttributeConfig.GetInstance(playerInfo.HeroID);
        totalAttribute= HeroAttributeConfig.GetInstance(playerInfo.HeroID);

        RoomModel.Instance.SaveHeroAttribute(playerInfo.RolesInfo.RolesID,currentAttribute,totalAttribute);
        //人物的HUD 血条 蓝条 昵称 等级
        HUD = Resmanager.Instance.LoadHUD();
        HUD.transform.position = transform.position + hudOffset;
        HUD.transform.eulerAngles = Camera.main.transform.eulerAngles;

        HPFill = HUD.transform.Find("HP/Fill").GetComponent<Image>();
        MPFill = HUD.transform.Find("MP/Fill").GetComponent<Image>();

        HPText = HUD.transform.Find("HP/Text").GetComponent<Text>();
        MPText = HUD.transform.Find("MP/Text").GetComponent<Text>();
        NickNameText = HUD.transform.Find("NickName").GetComponent<Text>();
        LevelText = HUD.transform.Find("Level/Text").GetComponent<Text>();

        //添加角色技能脚本
        AddHeroSkill(HeroID);
        //技能管理器
        skillManager = this.gameObject.AddComponent<SkillManager>();
        skillManager.Init(this);

        //动画管理器
        animatorManager = this.gameObject.AddComponent<AnimatorManager>();
        animatorManager.Init(this);


        //角色状态机 
        playerFSM = this.gameObject.AddComponent<PlayerFSM>();
        playerFSM.Init(this);

        //角色装备
        playerbag = this.gameObject.AddComponent<Playerbag>();
        playerbag.Init(this);

        if(isSelf==true)
        {
            battleWindow = (BattleWindow)WindowManager.Instance.GetWindow(WindowType.BattleWindow);
        }
        //相机的跟随
        if (isSelf==true)
        {
            if(playerInfo.TeamID==0)
            {
                Camera.main.transform.eulerAngles = new Vector3(45, 180, 0);
            }
            else
            {
                Camera.main.transform.eulerAngles = new Vector3(45, -180, 0);
            }
        }
        //属性设置
        experience = 0;
        totalexperience = 100;
        isSuperArmor = false;
        //更新界面
        HUDUpdate(true);
    }
    Vector3 CameraOffset = new Vector3(0, 11, 10);
    private void LateUpdate()
    {
        if (HUD != null)
        {
            HUD.transform.position = transform.position + hudOffset;
        }
        if (isSelf)
        {
            Camera.main.transform.position = this.transform.position+CameraOffset;
        }
    }
    #region 技能回调配置
    /// <summary>
    /// 碰撞到技能触发器 Q
    /// </summary>
    /// <param name="eConfig"></param>
    /// <param name="gameObject"></param>
    public void OnSkillQTrriger(EConfig eConfig,GameObject gameObject)
    {
        bool isDestroy=false;
        if (gameObject.CompareTag("Player"))
        {
            PlayerCtrl hitplayerCtrl = gameObject.GetComponent<PlayerCtrl>();
            PlayerInfo hitplayerInfo = hitplayerCtrl.playerInfo;
            //判断是否同个阵营的
            if(hitplayerInfo.TeamID!=playerInfo.TeamID)
            {
                //设置buffer对象
                Heroskill.target = gameObject;
                //设置攻击类型
                Heroskill.targettype = Targettype.hero;
                //造成伤害并添加buffer
                int hurt = (int)InjuryManager.Instance.hitInjury(currentAttribute, hitplayerCtrl.currentAttribute, Heroskill.SkillQ(), 0);
                hitplayerCtrl.OnSkillHit(hurt);
                if (eConfig.destroyMode == DestroyMode.OnHit_DifferentCampPlayer || eConfig.destroyMode == DestroyMode.OnHit_AllPlayer)
                {
                    isDestroy = true;
                    //并且销毁
                   // Destroy(eConfig.gameObject);
                }
            }
            else
            {
                //同一阵营
                return;
            }
        }
        //野怪
        else if (gameObject.CompareTag("Monster"))
        {
            Heroskill.target = gameObject;
            Heroskill.targettype = Targettype.creep;
            OneCreepFsm fsm = gameObject.GetComponent<OneCreepFsm>();
            int hurt = (int)InjuryManager.Instance.hitInjury(currentAttribute, fsm.currentAttribute, Heroskill.SkillQ(),0);
            fsm.Onhurt(hurt,this.gameObject);
            if (eConfig.destroyMode == DestroyMode.OnHit_DifferentCampPlayer || eConfig.destroyMode == DestroyMode.OnHit_AllPlayer)
            {
                isDestroy = true;
                //并且销毁
                // Destroy(eConfig.gameObject);
            }
            //让他扣血 并且让它进入对应的状态
        }/*
        //兵
        else if (other.CompareTag("Soldier"))
        {
            //如果不是同个阵营 让他扣血 并且让它进入对应的状态
        }
        //防御塔或者水晶
        else if (other.CompareTag("Tower"))
        {
            //如果不是同个阵营 让他扣血 并且让它进入对应的状态
        }
        //水晶
        else if (other.CompareTag("Crystal"))
        {
            //如果不是同个阵营 让他扣血 并且让它进入对应的状态
        }*/

        //克隆爆炸特效
        if (isDestroy && eConfig.hitLoad != null)
        {
            //克隆爆炸物
            GamePool.Instance.Addobj($"{HeroID}Qhit", eConfig.hitLoad);
            GameObject hitObj = GamePool.Instance.GetGameobject($"{HeroID}Qhit");
            //GameObject hitObj = GameObject.Instantiate(eConfig.hitLoad);
            hitObj.SetActive(true);
            hitObj.transform.position = eConfig.moveRoot.transform.position;

            //并且销毁
            //Destroy(eConfig.gameObject);
            //Destroy(hitObj, 2f);
            eConfig.gameObject.SetActive(false);
            GamePool.Instance.Recely($"{HeroID}Qhit", hitObj, 2f);
        }
    }
    //W
    public void OnSkillWTrriger(EConfig eConfig, GameObject gameObject)
    {
        bool isDestroy = false;
        if (gameObject.CompareTag("Player"))
        {
            PlayerCtrl hitplayerCtrl = gameObject.GetComponent<PlayerCtrl>();
            PlayerInfo hitplayerInfo = hitplayerCtrl.playerInfo;
            //判断是否同个阵营的
            if (hitplayerInfo.TeamID != playerInfo.TeamID)
            {
                //设置buffer对象
                Heroskill.target = gameObject;
                //
                Heroskill.targettype = Targettype.hero;
                //造成伤害并添加buffer
                int hurt = (int)InjuryManager.Instance.hitInjury(currentAttribute, hitplayerCtrl.currentAttribute, Heroskill.SkillW(), 0);
                hitplayerCtrl.OnSkillHit(hurt);
                if (eConfig.destroyMode == DestroyMode.OnHit_DifferentCampPlayer || eConfig.destroyMode == DestroyMode.OnHit_AllPlayer)
                {
                    isDestroy = true;
                    //并且销毁
                    // Destroy(eConfig.gameObject);
                }
            }
            else
            {
                //同一阵营
                return;
            }
        }
        //野怪
        else if (gameObject.CompareTag("Monster"))
        {
            Heroskill.target = gameObject;
            Heroskill.targettype = Targettype.creep;
            OneCreepFsm fsm = gameObject.GetComponent<OneCreepFsm>();
            int hurt = (int)InjuryManager.Instance.hitInjury(currentAttribute, fsm.currentAttribute, Heroskill.SkillW(), 0);
            fsm.Onhurt(hurt, this.gameObject);
            if (eConfig.destroyMode == DestroyMode.OnHit_DifferentCampPlayer || eConfig.destroyMode == DestroyMode.OnHit_AllPlayer)
            {
                isDestroy = true;
                //并且销毁
                // Destroy(eConfig.gameObject);
            }
            //让他扣血 并且让它进入对应的状态
            //让他扣血 并且让它进入对应的状态
        }
        /*//兵
        else if (other.CompareTag("Soldier"))
        {
            //如果不是同个阵营 让他扣血 并且让它进入对应的状态
        }
        //防御塔或者水晶
        else if (other.CompareTag("Tower"))
        {
            //如果不是同个阵营 让他扣血 并且让它进入对应的状态
        }
        //水晶
        else if (other.CompareTag("Crystal"))
        {
            //如果不是同个阵营 让他扣血 并且让它进入对应的状态
        }*/

        //克隆爆炸特效
        if (isDestroy && eConfig.hitLoad != null)
        {
            //克隆爆炸物
            GamePool.Instance.Addobj($"{HeroID}Whit", eConfig.hitLoad);
            GameObject hitObj = GamePool.Instance.GetGameobject($"{HeroID}Whit");
            //GameObject hitObj = GameObject.Instantiate(eConfig.hitLoad);
            hitObj.SetActive(true);
            hitObj.transform.position = eConfig.moveRoot.transform.position;

            //并且销毁
            //Destroy(eConfig.gameObject);
            //Destroy(hitObj, 2f);
            eConfig.gameObject.SetActive(false);
            GamePool.Instance.Recely($"{HeroID}Whit", hitObj,2f);
        }
    }
    //E
    public void OnSkillETrriger(EConfig eConfig, GameObject gameObject)
    {
        bool isDestroy = false;
        if (gameObject.CompareTag("Player"))
        {
            PlayerCtrl hitplayerCtrl = gameObject.GetComponent<PlayerCtrl>();
            PlayerInfo hitplayerInfo = hitplayerCtrl.playerInfo;
            //判断是否同个阵营的
            if (hitplayerInfo.TeamID != playerInfo.TeamID)
            {
                //设置buffer对象
                Heroskill.target = gameObject;
                //
                Heroskill.targettype = Targettype.hero;
                //造成伤害并添加buffer
                int hurt = (int)InjuryManager.Instance.hitInjury(currentAttribute, hitplayerCtrl.currentAttribute, Heroskill.SkillE(), 0);
                hitplayerCtrl.OnSkillHit(hurt);
                if (eConfig.destroyMode == DestroyMode.OnHit_DifferentCampPlayer || eConfig.destroyMode == DestroyMode.OnHit_AllPlayer)
                {
                    isDestroy = true;
                    //并且销毁
                    // Destroy(eConfig.gameObject);
                }
            }
            else
            {
                //同一阵营
                return;
            }
        }
        //野怪
        else if (gameObject.CompareTag("Monster"))
        {
            Heroskill.target = gameObject;
            Heroskill.targettype = Targettype.creep;
            OneCreepFsm fsm = gameObject.GetComponent<OneCreepFsm>();
            int hurt = (int)InjuryManager.Instance.hitInjury(currentAttribute, fsm.currentAttribute, Heroskill.SkillE(), 0);
            fsm.Onhurt(hurt, this.gameObject);
            if (eConfig.destroyMode == DestroyMode.OnHit_DifferentCampPlayer || eConfig.destroyMode == DestroyMode.OnHit_AllPlayer)
            {
                isDestroy = true;
                //并且销毁
                // Destroy(eConfig.gameObject);
            }
            //让他扣血 并且让它进入对应的状态
            //让他扣血 并且让它进入对应的状态
        }
        /*//兵
        else if (other.CompareTag("Soldier"))
        {
            //如果不是同个阵营 让他扣血 并且让它进入对应的状态
        }
        //防御塔或者水晶
        else if (other.CompareTag("Tower"))
        {
            //如果不是同个阵营 让他扣血 并且让它进入对应的状态
        }
        //水晶
        else if (other.CompareTag("Crystal"))
        {
            //如果不是同个阵营 让他扣血 并且让它进入对应的状态
        }*/

        //克隆爆炸特效
        if (isDestroy && eConfig.hitLoad != null)
        {
            //克隆爆炸物
            GamePool.Instance.Addobj($"{HeroID}Ehit", eConfig.hitLoad);
            GameObject hitObj = GamePool.Instance.GetGameobject($"{HeroID}Ehit");
            //GameObject hitObj = GameObject.Instantiate(eConfig.hitLoad);
            hitObj.SetActive(true);
            hitObj.transform.position = eConfig.moveRoot.transform.position;

            //并且销毁
            //Destroy(eConfig.gameObject);
            //Destroy(hitObj, 2f);
            eConfig.gameObject.SetActive(false);
            GamePool.Instance.Recely($"{HeroID}Ehit", hitObj, 2f);
        }
    }
    //R
    public void OnSkillRTrriger(EConfig eConfig, GameObject gameObject)
    {
        bool isDestroy = false;
        if (gameObject.CompareTag("Player"))
        {
            PlayerCtrl hitplayerCtrl = gameObject.GetComponent<PlayerCtrl>();
            PlayerInfo hitplayerInfo = hitplayerCtrl.playerInfo;
            //判断是否同个阵营的
            if (hitplayerInfo.TeamID != playerInfo.TeamID)
            {
                //设置buffer对象
                Heroskill.target = gameObject;
                //
                Heroskill.targettype = Targettype.hero;
                //造成伤害并添加buffer
                int hurt = (int)InjuryManager.Instance.hitInjury(currentAttribute, hitplayerCtrl.currentAttribute, Heroskill.SkillR(), 0);
                hitplayerCtrl.OnSkillHit(hurt);
                if (eConfig.destroyMode == DestroyMode.OnHit_DifferentCampPlayer || eConfig.destroyMode == DestroyMode.OnHit_AllPlayer)
                {
                    isDestroy = true;
                    //并且销毁
                    // Destroy(eConfig.gameObject);
                }
            }
            else
            {
                //同一阵营
                return;
            }
        }
        //野怪
        else if (gameObject.CompareTag("Monster"))
        {
            Heroskill.target = gameObject;
            Heroskill.targettype = Targettype.creep;
            OneCreepFsm fsm = gameObject.GetComponent<OneCreepFsm>();
            int hurt = (int)InjuryManager.Instance.hitInjury(currentAttribute, fsm.currentAttribute, Heroskill.SkillR(), 0);
            fsm.Onhurt(hurt, this.gameObject);
            if (eConfig.destroyMode == DestroyMode.OnHit_DifferentCampPlayer || eConfig.destroyMode == DestroyMode.OnHit_AllPlayer)
            {
                isDestroy = true;
                //并且销毁
                // Destroy(eConfig.gameObject);
            }
            //让他扣血 并且让它进入对应的状态
            //让他扣血 并且让它进入对应的状态
        }
        /*//兵
        else if (other.CompareTag("Soldier"))
        {
            //如果不是同个阵营 让他扣血 并且让它进入对应的状态
        }
        //防御塔或者水晶
        else if (other.CompareTag("Tower"))
        {
            //如果不是同个阵营 让他扣血 并且让它进入对应的状态
        }
        //水晶
        else if (other.CompareTag("Crystal"))
        {
            //如果不是同个阵营 让他扣血 并且让它进入对应的状态
        }*/

        //克隆爆炸特效
        if (isDestroy && eConfig.hitLoad != null)
        {
            //克隆爆炸物
            GamePool.Instance.Addobj($"{HeroID}Rhit", eConfig.hitLoad);
            GameObject hitObj = GamePool.Instance.GetGameobject($"{HeroID}Rhit");
            //GameObject hitObj = GameObject.Instantiate(eConfig.hitLoad);
            hitObj.SetActive(true);
            hitObj.transform.position = eConfig.moveRoot.transform.position;

            //并且销毁
            //Destroy(eConfig.gameObject);
            //Destroy(hitObj, 2f);
            eConfig.gameObject.SetActive(false);
            GamePool.Instance.Recely($"{HeroID}Rhit", hitObj, 2f);
        }
    }
    //A
    public void OnSkillATrriger(EConfig eConfig, GameObject gameObject)
    {
        bool isDestroy = false;
        if (gameObject.CompareTag("Player"))
        {
            PlayerCtrl hitplayerCtrl = gameObject.GetComponent<PlayerCtrl>();
            PlayerInfo hitplayerInfo = hitplayerCtrl.playerInfo;
            //判断是否同个阵营的
            if (hitplayerInfo.TeamID != playerInfo.TeamID)
            {
                //设置buffer对象
                Heroskill.target = gameObject;
                //
                Heroskill.targettype = Targettype.hero;
                //造成伤害并添加buffer
                int hurt = (int)InjuryManager.Instance.hitInjury(currentAttribute, hitplayerCtrl.currentAttribute, Heroskill.SkillA(), 0);
                hitplayerCtrl.OnSkillHit(hurt);
                if (eConfig.destroyMode == DestroyMode.OnHit_DifferentCampPlayer || eConfig.destroyMode == DestroyMode.OnHit_AllPlayer)
                {
                    isDestroy = true;
                    //并且销毁
                    // Destroy(eConfig.gameObject);
                }
            }
            else
            {
                //同一阵营
                return;
            }
        }
        //野怪
        else if (gameObject.CompareTag("Monster"))
        {
            Heroskill.target = gameObject;
            Heroskill.targettype = Targettype.creep;
            OneCreepFsm fsm = gameObject.GetComponent<OneCreepFsm>();
            int hurt = (int)InjuryManager.Instance.hitInjury(currentAttribute, fsm.currentAttribute, Heroskill.SkillA(), 0);
            fsm.Onhurt(hurt, this.gameObject);
            if (eConfig.destroyMode == DestroyMode.OnHit_DifferentCampPlayer || eConfig.destroyMode == DestroyMode.OnHit_AllPlayer)
            {
                isDestroy = true;
                //并且销毁
                // Destroy(eConfig.gameObject);
            }
            //让他扣血 并且让它进入对应的状态
            //让他扣血 并且让它进入对应的状态
        }
        /* //兵
         else if (other.CompareTag("Soldier"))
         {
             //如果不是同个阵营 让他扣血 并且让它进入对应的状态
         }
         //防御塔或者水晶
         else if (other.CompareTag("Tower"))
         {
             //如果不是同个阵营 让他扣血 并且让它进入对应的状态
         }
         //水晶
         else if (other.CompareTag("Crystal"))
         {
             //如果不是同个阵营 让他扣血 并且让它进入对应的状态
         }*/

        //克隆爆炸特效
        if (isDestroy && eConfig.hitLoad != null)
        {
            //克隆爆炸物
            GamePool.Instance.Addobj($"{HeroID}Ahit", eConfig.hitLoad);
            GameObject hitObj = GamePool.Instance.GetGameobject($"{HeroID}Ahit");
            //GameObject hitObj = GameObject.Instantiate(eConfig.hitLoad);
            hitObj.SetActive(true);
            hitObj.transform.position = eConfig.moveRoot.transform.position;

            //并且销毁
            //Destroy(eConfig.gameObject);
            //Destroy(hitObj, 2f);
            eConfig.gameObject.SetActive(false);
            GamePool.Instance.Recely($"{HeroID}Rhit", hitObj, 2f);
        }
    }
    #endregion
    /// <summary>
    /// 收到技能伤害
    /// </summary>
    /// <param name="hurt"></param>
    public void OnSkillHit(int hurt)
    {
        currentAttribute.HP -= hurt;
        if(currentAttribute.HP<=0)
        {
            currentAttribute.HP = 0;
            HUDUpdate();
            if(isSelf==true)
            {
                battleWindow.UpateAttrature();
            }
            //进入到死亡状态
            playerFSM.ToNext(FSMState.Dead);
        }
        else
        {
            HUDUpdate();
            if (isSelf == true)
            {
                battleWindow.UpateAttrature();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /// <summary>
    /// 血条更新
    /// </summary>
    /// <param name="init"></param>
    public void HUDUpdate(bool init=false)
    {
        HPText.text = $"{currentAttribute.HP}/{totalAttribute.HP}";
        MPText.text = $"{currentAttribute.MP}/{totalAttribute.MP}";
        LevelText.text = currentAttribute.Level.ToString();
        NickNameText.text = playerInfo.RolesInfo.NickName;

        if (init==true)
        {
            HPFill.fillAmount = 1;
            MPFill.fillAmount = 1;
        }
        else
        {
            HPFill.DOFillAmount(currentAttribute.HP / totalAttribute.HP, 0.2f).SetAutoKill(true);
            MPFill.DOFillAmount(currentAttribute.MP / totalAttribute.MP, 0.2f).SetAutoKill(true);
        } 
    }

    internal void Relive()
    {
        this.transform.position= spawnPosition;
        this.transform.eulerAngles= spawnRotation;
        //重置状态
        currentAttribute.HP = totalAttribute.HP;
        currentAttribute.MP = totalAttribute.MP;
    }
    /// <summary>
    /// 添加英雄技能脚本
    /// </summary>
    /// <param name="HeroID">英雄ID</param>
    private void AddHeroSkill(int HeroID)
    {
        switch (HeroID)
        {
            case 1001:
                Heroskill=this.AddComponent<HeroSkill1001>();
                break;
            default:
                break;
        }
    }
}
