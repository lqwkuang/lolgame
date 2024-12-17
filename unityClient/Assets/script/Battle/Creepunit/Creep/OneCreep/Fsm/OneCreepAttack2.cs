using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneCreepAttack2 : EntityFSM
{
    private OneCreepFsm creepfsm;
    private float timval;
    private float movespeed=5;
    public OneCreepAttack2(OneCreepFsm fsm)
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
        timval = 0;
        creepfsm.PlayAnima(Onecreepstate.attack2);
    }

    public override void Exit()
    {
        base.Exit();
        creepfsm.ExitAnima(Onecreepstate.attack2);
    }

    public override void RemoveListener()
    {
        base.RemoveListener();
    }

    public override void Update()
    {
        timval += Time.deltaTime;
        if(timval>=1&&timval<=1.2f)
        {
            //creepfsm.transform.Translate(creepfsm.transform.forward * movespeed * Time.deltaTime,Space.World);
            //creepfsm.transform.parent.Translate(creepfsm.transform.parent.forward * movespeed * Time.deltaTime, Space.World);
        }
        base.Update();
    }
}
