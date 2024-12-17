using ProtoMsg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneCreepWalk :EntityFSM
{
    private OneCreepFsm creepfsm;
    private float movespeed=5;
    public Vector3 Poscenter;
    public Vector3 rotations;
    public OneCreepWalk(OneCreepFsm fsm,Vector3 vector,Vector3 rotation)
    {
        this.creepfsm = fsm;
        this.Poscenter = vector;
        this.rotations = rotation;
    }

    public override void AddListener()
    {
        base.AddListener();
    }

    public override void Enter()
    {
        base.Enter();
        creepfsm.PlayAnima(Onecreepstate.move);
        creepfsm.agent.enabled = true;
    }

    public override void Exit()
    {
        base.Exit();
        creepfsm.agent.enabled = false;
        creepfsm.ExitAnima(Onecreepstate.move);
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
        //creepfsm.transform.parent.Translate(creepfsm.transform.parent.forward * movespeed * Time.deltaTime,Space.World);
        //creepfsm.transform.Translate(creepfsm.transform.forward * movespeed * Time.deltaTime, Space.World);
        if (creepfsm.player != null)
        {
            creepfsm.agent.SetDestination(creepfsm.player.transform.position);
        }
        else
        {
            float dis = Vector3.Distance(creepfsm.transform.position, Poscenter);
            if(dis<=0.15f)
            {
               
                creepfsm.transform.rotation = Quaternion.Euler(rotations.x, rotations.y, rotations.z);
                creepfsm.ToNext(Onecreepstate.ldl);
            }
            else
            {
                
                creepfsm.agent.SetDestination(Poscenter);
            }
            
        } 
        if(creepfsm.agent.pathStatus!=UnityEngine.AI.NavMeshPathStatus.PathComplete)
        {
            creepfsm.ToNext(Onecreepstate.ldl);
        }
    }
}
