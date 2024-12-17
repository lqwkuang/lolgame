using ProtoMsg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneCreepAttack4 : EntityFSM
{
    private OneCreepFsm creepfsm;
    public OneCreepAttack4(OneCreepFsm fsm)
    {
        this.creepfsm = fsm;
    }
    public override void AddListener()
    {
        base.AddListener();
    }

    public override void Enter()
    {
        base.Enter();
        creepfsm.PlayAnima(Onecreepstate.attack4);
    }

    public override void Exit()
    {
        base.Exit();
        creepfsm.ExitAnima(Onecreepstate.attack4);
    }

    public override void HandleCMD(BattleUserInputS2C s2cMSG)
    {
        base.HandleCMD(s2cMSG);
    }

    public override void HandleMoveEvent(BattleUserInputS2C s2cMSG)
    {
        base.HandleMoveEvent(s2cMSG);
    }

    public override void HandleSkillEvent(BattleUserInputS2C s2cMSG)
    {
        base.HandleSkillEvent(s2cMSG);
    }

    public override void RemoveListener()
    {
        base.RemoveListener();
    }
   
}
