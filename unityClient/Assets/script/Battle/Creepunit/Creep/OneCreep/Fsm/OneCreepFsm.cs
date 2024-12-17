using DG.Tweening;
using ProtoMsg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class OneCreepFsm : MonoBehaviour
{
    private Dictionary<Onecreepstate, EntityFSM> StateDic=new Dictionary<Onecreepstate, EntityFSM>();
    public EntityFSM currstate;//当前状态
    public Onecreepstate currCreepState;//当前状态
    public Animator animator;
    public Vector3 Poscenter=new Vector3(0,0,0);//领地中心
    public Vector3 LookatVec3 = new Vector3(0, 0, 0);//在原地的旋转度数
    public GameObject player;//攻击目标
    public NavMeshAgent agent;
    public HeroAttributeEntity currentAttribute;
    public HeroAttributeEntity totalAttribute;
    public GameObject HUD;
    Vector3 hudOffset = new Vector3(0, 5.2f, 0);//HUD位置偏移
    Text HPText;
    Image HPFill;
    //攻击记时
    private float attackRate=5f;
    private float timaval = 5f;
    //脱战记时
    private float outattack = 5f;
    private float OutTimeval = 0;
    [Header("攻击一")]
    public bool isAttack1 = false;
    private Vector3 Attack1center = new Vector3(0, 2, 0);
    private Vector3 Attack1Size = new Vector3(8, 4, 8);
    [Header("攻击二")]
    public bool isAttack2 = false;
    private Vector3 Attack2center = new Vector3(10, 2, -0.5f);
    private Vector3 Attack2Size=new Vector3(4,2,4);
    [Header("攻击三")]
    public bool isAttack3 = false;
    private Vector3 Attack3center = new Vector3(10, 4, 0);
    private Vector3 Attack3Size = new Vector3(4, 6, 4);
    [Header("攻击四")]
    public bool isAttack4 = false;
    public bool isAttack4Two = false;
    private Vector3 Attack4center = new Vector3(15, 2, 0);
    private Vector3 Attack4Size = new Vector3(6, 3, 6);
    private Vector3 Attack4Twocenter = new Vector3(20, 2, 0);
    private Vector3 Attack4TwoSize = new Vector3(3, 2, 8);
    // Start is called before the first frame update
    private Vector3 center1;
    private Vector3 center2;
    private Vector3 center3;
    private Vector3 center4;
    private Vector3 center5;
    public void Init()
    {
        StateDic[Onecreepstate.ldl] = new OneCreepldl(this);
        StateDic[Onecreepstate.attack1] = new OneCreepAttack1(this);
        StateDic[Onecreepstate.attack2] = new OneCreepAttack2(this);
        StateDic[Onecreepstate.attack3] = new OneCreepAttack3(this);
        StateDic[Onecreepstate.attack4] = new OneCreepAttack4(this);
        StateDic[Onecreepstate.move] = new OneCreepWalk(this,Poscenter,LookatVec3);
        StateDic[Onecreepstate.death] = new OneCreepDeath(this);
        animator = transform.GetComponent<Animator>();
        agent = transform.GetComponent<NavMeshAgent>();
        totalAttribute = HeroAttributeConfig.GetInstance(1001);//暂时用英雄的属性配置替代
        currentAttribute = HeroAttributeConfig.GetInstance(1001);
        //设置位置
        this.transform.position = Poscenter;
        this.transform.rotation =Quaternion.Euler(LookatVec3);
        //HUD
        HUD = Resmanager.Instance.LoadHUD();
        HUD.transform.position = transform.position + hudOffset;
        HUD.transform.parent = this.transform;

        HPFill = HUD.transform.Find("HP/Fill").GetComponent<Image>();

        HPText = HUD.transform.Find("HP/Text").GetComponent<Text>();
        HUD.transform.Find("NickName").GetComponent<Text>().text="红buffer";
        HUD.transform.Find("Level/Text").GetComponent<Text>().text="";
        HUDUpdate(true);
        ResetAllstate();
    }
    public void ResetAllstate()//重置状态(复活)
    {
        transform.gameObject.SetActive(true);
        currentAttribute = totalAttribute;
        ToNext(Onecreepstate.ldl);
    }
    void Start()
    {
      //  Init();
    }

    // Update is called once per frame
    void Update()
    {
        if(currstate!=null)
        currstate.Update();
        if(Input.GetKeyDown(KeyCode.Q))
        {
            ToNext(Onecreepstate.attack1);
        }
        else if(Input.GetKeyDown(KeyCode.W))
        {
            ToNext(Onecreepstate.attack2);
        }
        else if(Input.GetKeyDown(KeyCode.E))
        {
            ToNext(Onecreepstate.attack3);
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            ToNext(Onecreepstate.attack4);
        }
        else if(Input.GetKeyDown(KeyCode.Space))
        {
            ToNext(Onecreepstate.death);
        }
        timaval += Time.deltaTime;
        TotalAttack();
        OutAttack();
    }
    public void ToNext(Onecreepstate state)//切换
    {
        if(currstate!=null)
        currstate.Exit();
        StateDic[state].Enter();
        currstate = StateDic[state];
        currCreepState = state;
    }
    public void PlayAnima(Onecreepstate state)
    {
        animator.SetBool(state.ToString(), true);
    }
    public void ExitAnima(Onecreepstate state)
    {
        animator.SetBool(state.ToString(), false);
    }
    public void DoAnimaend()
    {
        ToNext(Onecreepstate.ldl);
    }//动画结束
    public void DoAttack1()//攻击一的攻击逻辑
    {
        Vector3 center = new Vector3(transform.forward.x * Attack1center.x, Attack1center.y, transform.forward.z*Attack1center.x);
        Collider[] colliders = Physics.OverlapBox(center + transform.position, Attack1Size, Quaternion.Euler(0, -transform.rotation.eulerAngles.y, 0));
        foreach(Collider it in colliders)
        {
            if(it.transform.tag=="Player")
            {
                float hurt = InjuryManager.Instance.hitInjuryCreep(it.GetComponent<PlayerCtrl>().currentAttribute, 100, 0);
                it.GetComponent<PlayerCtrl>().OnSkillHit((int)hurt);
            }
        }
    }
    public void DoAttack2()//攻击二的攻击逻辑
    {
        Vector3 center = new Vector3(transform.forward.x * Attack2center.x, Attack2center.y, transform.forward.z * Attack2center.x);
        Collider[] colliders= Physics.OverlapBox(center +transform.position, Attack2Size, Quaternion.Euler(0, -transform.rotation.eulerAngles.y, 0));
        foreach(var it in colliders)
        {
            if(it.transform.tag=="Player")
            {
                float hurt = InjuryManager.Instance.hitInjuryCreep(it.GetComponent<PlayerCtrl>().currentAttribute, 100, 0);
                it.GetComponent<PlayerCtrl>().OnSkillHit((int)hurt);
            }
        }
    }
    public void Doattack2Toattack1()//攻击二转换为攻击三
    {
        ToNext(Onecreepstate.attack1);
    }
    public void DoAttack3()//攻击三的攻击逻辑
    {
        Vector3 center = new Vector3(transform.forward.x * Attack3center.x, Attack3center.y, transform.forward.z * Attack3center.x);
        Collider[] colliders= Physics.OverlapBox(center + transform.position, Attack3Size, Quaternion.Euler(0, -transform.rotation.eulerAngles.y, 0));
        foreach (var it in colliders)
        {
            if (it.transform.tag == "Player")
            {
                float hurt = InjuryManager.Instance.hitInjuryCreep(it.GetComponent<PlayerCtrl>().currentAttribute, 100, 0);
                it.GetComponent<PlayerCtrl>().OnSkillHit((int)hurt);
            }
        }
    }
    public void DoAttack4()//攻击四的攻击逻辑
    {
        Vector3 center = new Vector3(transform.forward.x * Attack4center.x, Attack4center.y, transform.forward.z * Attack4center.x);
        Collider[] colliders= Physics.OverlapBox(center + transform.position, Attack4Size, Quaternion.Euler(0, -transform.rotation.eulerAngles.y, 0));
        foreach (var it in colliders)
        {
            if (it.transform.tag == "Player")
            {
                float hurt = InjuryManager.Instance.hitInjuryCreep(it.GetComponent<PlayerCtrl>().currentAttribute, 100, 0);
                it.GetComponent<PlayerCtrl>().OnSkillHit((int)hurt);
            }
        }
    }
    public void DoAttack4Two()//攻击四的二段伤害
    {
        Vector3 center = new Vector3(transform.forward.x * Attack4Twocenter.x, Attack4Twocenter.y, transform.forward.z * Attack4Twocenter.x);
        Collider[] colliders= Physics.OverlapBox(center + transform.position, Attack4TwoSize,Quaternion.Euler(0,-transform.rotation.eulerAngles.y,0));
        foreach (var it in colliders)
        {
            if (it.transform.tag == "Player")
            {
                float hurt = InjuryManager.Instance.hitInjuryCreep(it.GetComponent<PlayerCtrl>().currentAttribute, 100, 0);
                it.GetComponent<PlayerCtrl>().OnSkillHit((int)hurt);
            }
        }
    }
    private void TotalAttack()//总攻击逻辑
    {
        if (player == null)
            return;
        if(timaval<attackRate)
        {
            return;
        }
        
        float dis = Vector3.Distance(player.transform.position, transform.position);
        if (10f<dis&&dis<=18f)
        {
            if(currCreepState==Onecreepstate.ldl||currCreepState==Onecreepstate.move)
            {
                timaval = 0;
                ToNext(Onecreepstate.attack4);
            }
        }
        else if(5f<dis&&dis<=13f)
        {
            if (currCreepState == Onecreepstate.ldl || currCreepState == Onecreepstate.move)
            {
                timaval = 0;
                int i = Random.Range(0, 2);
                if (i == 0)
                    ToNext(Onecreepstate.attack2);
                else
                    ToNext(Onecreepstate.attack3);
            }
        }
        else if(dis<=8f)
        {
            if (currCreepState == Onecreepstate.ldl || currCreepState == Onecreepstate.move)
            {
                timaval = 0;
                ToNext(Onecreepstate.attack1);
            }
        }
        else
        {
            timaval = 0;
            ToNext(Onecreepstate.move);
        }
    }
    private void OutAttack()//脱战逻辑
    {
        if (player == null)
            return;
        float dis = Vector3.Distance(player.transform.position, Poscenter);
        if(dis>20f)
        {
            OutTimeval += Time.deltaTime;
            if(OutTimeval>= outattack)
            {
                player = null;//完全脱战
                //野怪回到原地
                ToNext(Onecreepstate.move);
            }
        }
        else
        {
            OutTimeval -= Time.deltaTime;
            if(OutTimeval<=0)
            {
                OutTimeval = 0;
            }
        }
    }
    public void Dodeath()//死亡动画结束
    {
        this.gameObject.SetActive(false);
        HUD.SetActive(false);//关闭UI
    }
    public void Onhurt(float hurt,GameObject target)//减少血量
    {
        if(player==null)
        {
            player = target;
        }
        currentAttribute.HP -= hurt;
        if(currentAttribute.HP<=0)
        {
            ToNext(Onecreepstate.death); //死亡然后禁用
        }
        else
        {
            HUDUpdate();
        }
    }
    public void HUDUpdate(bool init = false)//更新UI
    {
        HPText.text = $"{currentAttribute.HP}/{totalAttribute.HP}";

        if (init == true)
        {
            HPFill.fillAmount = 1;
        }
        else
        {
            HPFill.DOFillAmount(currentAttribute.HP / totalAttribute.HP, 0.2f).SetAutoKill(true);
        }
    }

    private void OnDrawGizmos()//测试检测范围
    {
        Gizmos.color = Color.blue;
        center1 = new Vector3(transform.forward.x * Attack1center.x, Attack1center.y, transform.forward.z * Attack1center.x);
        center2 = new Vector3(transform.forward.x * Attack2center.x, Attack2center.y, transform.forward.z * Attack2center.x);
        center3 = new Vector3(transform.forward.x * Attack3center.x, Attack3center.y, transform.forward.z * Attack3center.x);
        center4 = new Vector3(transform.forward.x * Attack4center.x, Attack4center.y, transform.forward.z * Attack4center.x);
        center5 = new Vector3(transform.forward.x * Attack4Twocenter.x, Attack4Twocenter.y, transform.forward.z * Attack4Twocenter.x);
        if (isAttack2)
        Gizmos.DrawWireCube(center2+transform.position, Attack2Size);
        if(isAttack1)
        Gizmos.DrawWireCube(center1+transform.position, Attack1Size);
        if(isAttack3)
        Gizmos.DrawWireCube(center3 + transform.position, Attack3Size);
        if(isAttack4)
        Gizmos.DrawWireCube(center4 + transform.position, Attack4Size);
        if(isAttack4Two)
        Gizmos.DrawWireCube(center5 + transform.position, Attack4TwoSize);
    }

}
