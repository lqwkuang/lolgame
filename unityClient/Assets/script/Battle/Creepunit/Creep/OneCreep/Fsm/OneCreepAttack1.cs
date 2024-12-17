using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneCreepAttack1 : EntityFSM
{
    private OneCreepFsm creepfsm;
    public OneCreepAttack1(OneCreepFsm fsm)
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
        creepfsm.PlayAnima(Onecreepstate.attack1);
    }

    public override void Exit()
    {
        base.Exit();
        creepfsm.ExitAnima(Onecreepstate.attack1);
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
