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
    public int HeroID;//Ӣ�۱��
    public HeroAttributeEntity currentAttribute;
    public HeroAttributeEntity totalAttribute;

    public GameObject HUD;
    public GameObject minmapIcon;//С��ͼͷ��
    Vector3 hudOffset = new Vector3(0, 3.1f, 0);

    Text HPText, MPText,LevelText,NickNameText;
    Image HPFill, MPFill;

    //����ս��״̬���� ���� ���� ���� ���� ���� ����ȼ� �ȵ�
    public int experience;//����
    public int totalexperience;//�ܾ������˾�����һ��
    public bool isSuperArmor;//�Ƿ����
    internal void Init(PlayerInfo item)
    {
        this.playerInfo = item;
        //Ӣ�۲����Լ��Ľ�ɫ
        isSelf= PlayerModel.Instance.CheckIsSelf(item.RolesInfo.RolesID);
        if(isSelf)
        {
            //this.gameObject.layer = LayerMask.NameToLayer("UI");
            this.transform.AddComponent<VisualFieldObject>();
        }
        //С��ͼͷ��
        minmapIcon = Resmanager.Instance.LoadMinmapIcon($"{item.HeroID}Mapicon");
        minmapIcon.GetComponent<Canvas>().worldCamera = GameObject.Find("mapCamera").GetComponent<Camera>();
        minmapIcon.transform.position = transform.position + new Vector3(0, 2, 0);
        minmapIcon.transform.SetParent(transform);
        //�õ�Ӣ�۱��
        HeroID = item.HeroID;
        //�����ʱ���λ��
        spawnPosition = transform.position;
        spawnRotation = transform.eulerAngles;
        //��ȡ�������� ��ǰ������ �����ܵ�����
        currentAttribute=HeroAttributeConfig.GetInstance(playerInfo.HeroID);
        totalAttribute= HeroAttributeConfig.GetInstance(playerInfo.HeroID);

        RoomModel.Instance.SaveHeroAttribute(playerInfo.RolesInfo.RolesID,currentAttribute,totalAttribute);
        //�����HUD Ѫ�� ���� �ǳ� �ȼ�
        HUD = Resmanager.Instance.LoadHUD();
        HUD.transform.position = transform.position + hudOffset;
        HUD.transform.eulerAngles = Camera.main.transform.eulerAngles;

        HPFill = HUD.transform.Find("HP/Fill").GetComponent<Image>();
        MPFill = HUD.transform.Find("MP/Fill").GetComponent<Image>();

        HPText = HUD.transform.Find("HP/Text").GetComponent<Text>();
        MPText = HUD.transform.Find("MP/Text").GetComponent<Text>();
        NickNameText = HUD.transform.Find("NickName").GetComponent<Text>();
        LevelText = HUD.transform.Find("Level/Text").GetComponent<Text>();

        //��ӽ�ɫ���ܽű�
        AddHeroSkill(HeroID);
        //���ܹ�����
        skillManager = this.gameObject.AddComponent<SkillManager>();
        skillManager.Init(this);

        //����������
        animatorManager = this.gameObject.AddComponent<AnimatorManager>();
        animatorManager.Init(this);


        //��ɫ״̬�� 
        playerFSM = this.gameObject.AddComponent<PlayerFSM>();
        playerFSM.Init(this);

        //��ɫװ��
        playerbag = this.gameObject.AddComponent<Playerbag>();
        playerbag.Init(this);

        if(isSelf==true)
        {
            battleWindow = (BattleWindow)WindowManager.Instance.GetWindow(WindowType.BattleWindow);
        }
        //����ĸ���
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
        //��������
        experience = 0;
        totalexperience = 100;
        isSuperArmor = false;
        //���½���
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
    #region ���ܻص�����
    /// <summary>
    /// ��ײ�����ܴ����� Q
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
            //�ж��Ƿ�ͬ����Ӫ��
            if(hitplayerInfo.TeamID!=playerInfo.TeamID)
            {
                //����buffer����
                Heroskill.target = gameObject;
                //���ù�������
                Heroskill.targettype = Targettype.hero;
                //����˺������buffer
                int hurt = (int)InjuryManager.Instance.hitInjury(currentAttribute, hitplayerCtrl.currentAttribute, Heroskill.SkillQ(), 0);
                hitplayerCtrl.OnSkillHit(hurt);
                if (eConfig.destroyMode == DestroyMode.OnHit_DifferentCampPlayer || eConfig.destroyMode == DestroyMode.OnHit_AllPlayer)
                {
                    isDestroy = true;
                    //��������
                   // Destroy(eConfig.gameObject);
                }
            }
            else
            {
                //ͬһ��Ӫ
                return;
            }
        }
        //Ұ��
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
                //��������
                // Destroy(eConfig.gameObject);
            }
            //������Ѫ �������������Ӧ��״̬
        }/*
        //��
        else if (other.CompareTag("Soldier"))
        {
            //�������ͬ����Ӫ ������Ѫ �������������Ӧ��״̬
        }
        //����������ˮ��
        else if (other.CompareTag("Tower"))
        {
            //�������ͬ����Ӫ ������Ѫ �������������Ӧ��״̬
        }
        //ˮ��
        else if (other.CompareTag("Crystal"))
        {
            //�������ͬ����Ӫ ������Ѫ �������������Ӧ��״̬
        }*/

        //��¡��ը��Ч
        if (isDestroy && eConfig.hitLoad != null)
        {
            //��¡��ը��
            GamePool.Instance.Addobj($"{HeroID}Qhit", eConfig.hitLoad);
            GameObject hitObj = GamePool.Instance.GetGameobject($"{HeroID}Qhit");
            //GameObject hitObj = GameObject.Instantiate(eConfig.hitLoad);
            hitObj.SetActive(true);
            hitObj.transform.position = eConfig.moveRoot.transform.position;

            //��������
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
            //�ж��Ƿ�ͬ����Ӫ��
            if (hitplayerInfo.TeamID != playerInfo.TeamID)
            {
                //����buffer����
                Heroskill.target = gameObject;
                //
                Heroskill.targettype = Targettype.hero;
                //����˺������buffer
                int hurt = (int)InjuryManager.Instance.hitInjury(currentAttribute, hitplayerCtrl.currentAttribute, Heroskill.SkillW(), 0);
                hitplayerCtrl.OnSkillHit(hurt);
                if (eConfig.destroyMode == DestroyMode.OnHit_DifferentCampPlayer || eConfig.destroyMode == DestroyMode.OnHit_AllPlayer)
                {
                    isDestroy = true;
                    //��������
                    // Destroy(eConfig.gameObject);
                }
            }
            else
            {
                //ͬһ��Ӫ
                return;
            }
        }
        //Ұ��
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
                //��������
                // Destroy(eConfig.gameObject);
            }
            //������Ѫ �������������Ӧ��״̬
            //������Ѫ �������������Ӧ��״̬
        }
        /*//��
        else if (other.CompareTag("Soldier"))
        {
            //�������ͬ����Ӫ ������Ѫ �������������Ӧ��״̬
        }
        //����������ˮ��
        else if (other.CompareTag("Tower"))
        {
            //�������ͬ����Ӫ ������Ѫ �������������Ӧ��״̬
        }
        //ˮ��
        else if (other.CompareTag("Crystal"))
        {
            //�������ͬ����Ӫ ������Ѫ �������������Ӧ��״̬
        }*/

        //��¡��ը��Ч
        if (isDestroy && eConfig.hitLoad != null)
        {
            //��¡��ը��
            GamePool.Instance.Addobj($"{HeroID}Whit", eConfig.hitLoad);
            GameObject hitObj = GamePool.Instance.GetGameobject($"{HeroID}Whit");
            //GameObject hitObj = GameObject.Instantiate(eConfig.hitLoad);
            hitObj.SetActive(true);
            hitObj.transform.position = eConfig.moveRoot.transform.position;

            //��������
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
            //�ж��Ƿ�ͬ����Ӫ��
            if (hitplayerInfo.TeamID != playerInfo.TeamID)
            {
                //����buffer����
                Heroskill.target = gameObject;
                //
                Heroskill.targettype = Targettype.hero;
                //����˺������buffer
                int hurt = (int)InjuryManager.Instance.hitInjury(currentAttribute, hitplayerCtrl.currentAttribute, Heroskill.SkillE(), 0);
                hitplayerCtrl.OnSkillHit(hurt);
                if (eConfig.destroyMode == DestroyMode.OnHit_DifferentCampPlayer || eConfig.destroyMode == DestroyMode.OnHit_AllPlayer)
                {
                    isDestroy = true;
                    //��������
                    // Destroy(eConfig.gameObject);
                }
            }
            else
            {
                //ͬһ��Ӫ
                return;
            }
        }
        //Ұ��
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
                //��������
                // Destroy(eConfig.gameObject);
            }
            //������Ѫ �������������Ӧ��״̬
            //������Ѫ �������������Ӧ��״̬
        }
        /*//��
        else if (other.CompareTag("Soldier"))
        {
            //�������ͬ����Ӫ ������Ѫ �������������Ӧ��״̬
        }
        //����������ˮ��
        else if (other.CompareTag("Tower"))
        {
            //�������ͬ����Ӫ ������Ѫ �������������Ӧ��״̬
        }
        //ˮ��
        else if (other.CompareTag("Crystal"))
        {
            //�������ͬ����Ӫ ������Ѫ �������������Ӧ��״̬
        }*/

        //��¡��ը��Ч
        if (isDestroy && eConfig.hitLoad != null)
        {
            //��¡��ը��
            GamePool.Instance.Addobj($"{HeroID}Ehit", eConfig.hitLoad);
            GameObject hitObj = GamePool.Instance.GetGameobject($"{HeroID}Ehit");
            //GameObject hitObj = GameObject.Instantiate(eConfig.hitLoad);
            hitObj.SetActive(true);
            hitObj.transform.position = eConfig.moveRoot.transform.position;

            //��������
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
            //�ж��Ƿ�ͬ����Ӫ��
            if (hitplayerInfo.TeamID != playerInfo.TeamID)
            {
                //����buffer����
                Heroskill.target = gameObject;
                //
                Heroskill.targettype = Targettype.hero;
                //����˺������buffer
                int hurt = (int)InjuryManager.Instance.hitInjury(currentAttribute, hitplayerCtrl.currentAttribute, Heroskill.SkillR(), 0);
                hitplayerCtrl.OnSkillHit(hurt);
                if (eConfig.destroyMode == DestroyMode.OnHit_DifferentCampPlayer || eConfig.destroyMode == DestroyMode.OnHit_AllPlayer)
                {
                    isDestroy = true;
                    //��������
                    // Destroy(eConfig.gameObject);
                }
            }
            else
            {
                //ͬһ��Ӫ
                return;
            }
        }
        //Ұ��
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
                //��������
                // Destroy(eConfig.gameObject);
            }
            //������Ѫ �������������Ӧ��״̬
            //������Ѫ �������������Ӧ��״̬
        }
        /*//��
        else if (other.CompareTag("Soldier"))
        {
            //�������ͬ����Ӫ ������Ѫ �������������Ӧ��״̬
        }
        //����������ˮ��
        else if (other.CompareTag("Tower"))
        {
            //�������ͬ����Ӫ ������Ѫ �������������Ӧ��״̬
        }
        //ˮ��
        else if (other.CompareTag("Crystal"))
        {
            //�������ͬ����Ӫ ������Ѫ �������������Ӧ��״̬
        }*/

        //��¡��ը��Ч
        if (isDestroy && eConfig.hitLoad != null)
        {
            //��¡��ը��
            GamePool.Instance.Addobj($"{HeroID}Rhit", eConfig.hitLoad);
            GameObject hitObj = GamePool.Instance.GetGameobject($"{HeroID}Rhit");
            //GameObject hitObj = GameObject.Instantiate(eConfig.hitLoad);
            hitObj.SetActive(true);
            hitObj.transform.position = eConfig.moveRoot.transform.position;

            //��������
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
            //�ж��Ƿ�ͬ����Ӫ��
            if (hitplayerInfo.TeamID != playerInfo.TeamID)
            {
                //����buffer����
                Heroskill.target = gameObject;
                //
                Heroskill.targettype = Targettype.hero;
                //����˺������buffer
                int hurt = (int)InjuryManager.Instance.hitInjury(currentAttribute, hitplayerCtrl.currentAttribute, Heroskill.SkillA(), 0);
                hitplayerCtrl.OnSkillHit(hurt);
                if (eConfig.destroyMode == DestroyMode.OnHit_DifferentCampPlayer || eConfig.destroyMode == DestroyMode.OnHit_AllPlayer)
                {
                    isDestroy = true;
                    //��������
                    // Destroy(eConfig.gameObject);
                }
            }
            else
            {
                //ͬһ��Ӫ
                return;
            }
        }
        //Ұ��
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
                //��������
                // Destroy(eConfig.gameObject);
            }
            //������Ѫ �������������Ӧ��״̬
            //������Ѫ �������������Ӧ��״̬
        }
        /* //��
         else if (other.CompareTag("Soldier"))
         {
             //�������ͬ����Ӫ ������Ѫ �������������Ӧ��״̬
         }
         //����������ˮ��
         else if (other.CompareTag("Tower"))
         {
             //�������ͬ����Ӫ ������Ѫ �������������Ӧ��״̬
         }
         //ˮ��
         else if (other.CompareTag("Crystal"))
         {
             //�������ͬ����Ӫ ������Ѫ �������������Ӧ��״̬
         }*/

        //��¡��ը��Ч
        if (isDestroy && eConfig.hitLoad != null)
        {
            //��¡��ը��
            GamePool.Instance.Addobj($"{HeroID}Ahit", eConfig.hitLoad);
            GameObject hitObj = GamePool.Instance.GetGameobject($"{HeroID}Ahit");
            //GameObject hitObj = GameObject.Instantiate(eConfig.hitLoad);
            hitObj.SetActive(true);
            hitObj.transform.position = eConfig.moveRoot.transform.position;

            //��������
            //Destroy(eConfig.gameObject);
            //Destroy(hitObj, 2f);
            eConfig.gameObject.SetActive(false);
            GamePool.Instance.Recely($"{HeroID}Rhit", hitObj, 2f);
        }
    }
    #endregion
    /// <summary>
    /// �յ������˺�
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
            //���뵽����״̬
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
    /// Ѫ������
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
        //����״̬
        currentAttribute.HP = totalAttribute.HP;
        currentAttribute.MP = totalAttribute.MP;
    }
    /// <summary>
    /// ���Ӣ�ۼ��ܽű�
    /// </summary>
    /// <param name="HeroID">Ӣ��ID</param>
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
