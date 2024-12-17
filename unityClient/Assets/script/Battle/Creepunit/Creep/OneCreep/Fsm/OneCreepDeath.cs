using ProtoMsg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneCreepDeath : EntityFSM
{
    private OneCreepFsm creepfsm;
    public OneCreepDeath(OneCreepFsm fsm)
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
        creepfsm.PlayAnima(Onecreepstate.death);
    }

    public override void Exit()
    {
        base.Exit();
        creepfsm.ExitAnima(Onecreepstate.death);
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

    public override void Update()
    {
        base.Update();
    }
}
