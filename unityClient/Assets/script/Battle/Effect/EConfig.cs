using Game.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//挂载在每个特效物体上,为什么呢?
public class EConfig : MonoBehaviour
{
    #region 由特效人员进行配置
   
    public float moveSpeed;//移动的速度
    public MoveType moveType;//移动的类型
    public float destroyTime;//销毁的时间
    public GameObject hitLoad;//碰撞时候触发的特效
    public GameObject spawnLoad;//出生时的特效,如枪口

    public Transform moveRoot;//运动的根节点
    public string effectID;//特效ID=英雄ID+技能按键名称

    public DestroyMode destroyMode;//销毁模式
    //作用类型 敌方或者友方 ..

    #endregion

    #region 由程序动态配置
    [HideInInspector]
    public int releaserID;//释放者ID
    [HideInInspector]
    public string lockTag;//锁定的目标的标签
    [HideInInspector]
    public int lockID;//锁定的目标ID

    [HideInInspector]
    public Vector3 skillDirection;//方向
    [HideInInspector]
    public Vector3 spawnPositon;//出生的旋转
    [HideInInspector]
    public Transform trackObject;

    public Action<EConfig,GameObject> hitAction;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="releaserID">释放技能的用户ID</param>
    /// <param name="lockTag">锁定目标的标签</param>
    /// <param name="lockID">锁定目标的ID</param>
    /// <param name="skillDirection">技能方向</param>
    /// <param name="spawnPositon">出生位置</param>
    /// <param name="hitAction">碰撞到的回调</param>
    public  void Init(int releaserID, string lockTag, int lockID,Vector3 skillDirection, Vector3 spawnPositon,Action<EConfig,GameObject> hitAction)
    {
        if(hitAction!=null)
        {
            this.hitAction = hitAction;
        }
        this.releaserID = releaserID;
        this.lockTag = lockTag;
        this.lockID = lockID;
        this.skillDirection = skillDirection;
        this.spawnPositon = spawnPositon;
        if (moveType==MoveType.TrackMove)
        {
            if (lockTag=="Player")
            {
                //获取到跟踪的物体
                trackObject = RoomModel.Instance.playerObjects[lockID].transform;
            }
        }

        //移动的根物体
        if (moveRoot != null)
        {
            //碰撞和移动是保持在同一个节点下 

            //加上特效移动的脚本
            if (moveRoot.gameObject.GetComponent<EMove>()==null)
            {
                moveRoot.gameObject.AddComponent<EMove>().Init(this);
            }
            //加上碰撞控制的脚本 把碰撞的处理回调 传递进去
            if(moveRoot.gameObject.GetComponent<EHit>() == null)
            {
                moveRoot.gameObject.AddComponent<EHit>().Init(this);
            }
        }

      
    }

    public void Awake()
    {
      
    }

    #endregion
}


public enum MoveType
{
    DirectMove,
    TrackMove,
}

/// <summary>
/// 销毁模式
/// </summary>
public enum DestroyMode
{
    OnHit_SameCampPlayer,
    OnHit_DifferentCampPlayer,
    OnHit_AllPlayer,
    OnDestroyTimeEnd,//当销毁时间结束 执行销毁 也就是不让触碰的时候销毁 因为有些技能是持续性的 

    //可以配置更多 比如有的技能是可以攻击防御塔的 有的是不可以的
}