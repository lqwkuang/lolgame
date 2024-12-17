using ProtoMsg;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerSkill : EntityFSM
{
    public PlayerSkill(Transform transform,PlayerFSM fsm)
    {
        this.transform = transform;
        this.fsm = fsm;
    }
    public override void AddListener()
    {
        base.AddListener();
    }

    public override void Enter()
    {
        base.Enter();
        //朝向鼠标的位置
        transform.LookAt(fsm.skillCMD.CMD.MousePosition.ToVector3());
        if(fsm.skillCMD.CMD.Key==KeyCode.Q.GetHashCode())
        {
            fsm.playerCtrl.animatorManager.Play(PlayerAnimationClip.SkillQ);
        }
        else if(fsm.skillCMD.CMD.Key == KeyCode.W.GetHashCode())
        {
            fsm.playerCtrl.animatorManager.Play(PlayerAnimationClip.SkillW);
        }
        else if (fsm.skillCMD.CMD.Key == KeyCode.E.GetHashCode())
        {
            fsm.playerCtrl.animatorManager.Play(PlayerAnimationClip.SkillE);
        }
        else if (fsm.skillCMD.CMD.Key == KeyCode.R.GetHashCode())
        {
            fsm.playerCtrl.animatorManager.Play(PlayerAnimationClip.SkillR);
        }
        else if (fsm.skillCMD.CMD.Key == KeyCode.A.GetHashCode())
        {
            fsm.playerCtrl.animatorManager.Play(PlayerAnimationClip.NormalAttack);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void HandleCMD(BattleUserInputS2C s2cMSG)
    {
        base.HandleCMD(s2cMSG);
    }

    public override void HandleMoveEvent(BattleUserInputS2C s2cMSG)
    {
        base.HandleMoveEvent(s2cMSG);
        //移动事件不处理
    }

    public override void HandleSkillEvent(BattleUserInputS2C s2cMSG)
    {
        base.HandleSkillEvent(s2cMSG);
        //如果是技能事件
        //点燃 治疗 护盾 -->可=可以叠加的技能
        if(fsm.skillCMD.CMD.Key==KeyCode.D.GetHashCode())
        {

        }
        else if(fsm.skillCMD.CMD.Key == KeyCode.F.GetHashCode())
        {

        }
        else
        {
            Debug.Log("请等待本次技能释放完毕");
        }
        
    }

    public override void RemoveListener()
    {
        base.RemoveListener();
    }

    public override void Update()
    {
        base.Update();
    }
}
