using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Onecreepstate
{
    ldl,
    attack1,
    attack2,
    attack3,
    attack4,
    move,
    death,
}
public class OneCreepldl : EntityFSM
{
    private OneCreepFsm creepfsm;
    public OneCreepldl(OneCreepFsm fsm)
    {
        this.creepfsm = fsm;
    }
    public override void Enter()
    {
        base.Enter();
        creepfsm.PlayAnima(Onecreepstate.ldl);
    }

    public override void Exit()
    {
        base.Exit();
        creepfsm.ExitAnima(Onecreepstate.ldl);
    }

    public override void Update()
    {
        base.Update();
    }
}
