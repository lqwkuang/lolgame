using ProtoMsg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Game.View;
public class EntityFSM 
{
    public Transform transform;
    public PlayerFSM fsm; //管理类
    private Action<float> action;
    //进入状态
    public virtual void Enter() {
        AddListener();
    }

    //状态更新中
    public virtual void Update() { 
    
    }

    //退出状态
    public virtual void Exit() {
        RemoveListener();
    }

    //监听一些游戏事件
    public virtual void AddListener() { 
    
    }

    //移除掉监听的事件
    public virtual void RemoveListener()
    {

    }

    //处理每一帧的网络消息
    public virtual void HandleCMD(BattleUserInputS2C s2cMSG)
    {
        if(s2cMSG.CMD.Key==KeyCode.A.GetHashCode())
        {
            fsm.gameObject.GetComponent<SkillManager>().DoCooling(KeyCode.A,action);
            HandleSkillEvent(s2cMSG);
        }
        else if(s2cMSG.CMD.Key == KeyCode.Q.GetHashCode())
        {
            fsm.gameObject.GetComponent<SkillManager>().DoCooling(KeyCode.Q, action);
            HandleSkillEvent(s2cMSG);
        }
        else if (s2cMSG.CMD.Key == KeyCode.W.GetHashCode())
        {
            fsm.gameObject.GetComponent<SkillManager>().DoCooling(KeyCode.W, action);
            HandleSkillEvent(s2cMSG);
        }
        else if (s2cMSG.CMD.Key == KeyCode.E.GetHashCode())
        {
            fsm.gameObject.GetComponent<SkillManager>().DoCooling(KeyCode.E, action);
            HandleSkillEvent(s2cMSG);
        }
        else if (s2cMSG.CMD.Key == KeyCode.R.GetHashCode())
        {
            fsm.gameObject.GetComponent<SkillManager>().DoCooling(KeyCode.R, action);
            HandleSkillEvent(s2cMSG);
        }
        else if (s2cMSG.CMD.Key == KeyCode.D.GetHashCode())
        {
            fsm.gameObject.GetComponent<SkillManager>().DoCooling(KeyCode.D, action);
            HandleSkillEvent(s2cMSG);
        }
        else if (s2cMSG.CMD.Key == KeyCode.F.GetHashCode())
        {
            fsm.gameObject.GetComponent<SkillManager>().DoCooling(KeyCode.F, action);
            HandleSkillEvent(s2cMSG);
        }
        else if (s2cMSG.CMD.Key == KeyCode.B.GetHashCode())
        {
            HandleSkillEvent(s2cMSG);
        }
        else if (s2cMSG.CMD.Key == KeyCode.Mouse1.GetHashCode())
        {
            //移动
            HandleMoveEvent(s2cMSG);
        }
    }
    public virtual void HandleMoveEvent(BattleUserInputS2C s2cMSG)
    {
        fsm.moveCMD = s2cMSG;
    }
    public virtual void HandleSkillEvent(BattleUserInputS2C s2cMSG)
    {
        fsm.skillCMD = s2cMSG;
    }
}
