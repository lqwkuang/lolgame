using ProtoMsg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdIe : EntityFSM
{
    public PlayerIdIe(Transform transform, PlayerFSM fsm)
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
        this.fsm.agent.enabled = false;
        fsm.playerCtrl.animatorManager.Play(PlayerAnimationClip.Idle);
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
        fsm.ToNext(FSMState.Move);
    }

    public override void HandleSkillEvent(BattleUserInputS2C s2cMSG)
    {
        base.HandleSkillEvent(s2cMSG);
        fsm.ToNext(FSMState.Skill);
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
