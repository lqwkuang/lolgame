using ProtoMsg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMove : EntityFSM
{
    public PlayerMove(Transform transform, PlayerFSM fsm)
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
        fsm.playerCtrl.animatorManager.Play(PlayerAnimationClip.Run);
        fsm.agent.enabled = true;
        //设置目标点 数据来源 服务器发送过来的 客户端发给服务器
        destination = fsm.moveCMD.CMD.MousePosition.ToVector3();
        fsm.agent.SetDestination(destination);
        
        isMove = true;
    }
    bool isMove = false;
    Vector3 destination;
    public override void Exit()
    {
        base.Exit();
        fsm.agent.enabled = false;
    }

    public override void HandleCMD(BattleUserInputS2C s2cMSG)
    {
        base.HandleCMD(s2cMSG);
    }

    public override void HandleMoveEvent(BattleUserInputS2C s2cMSG)
    {
        base.HandleMoveEvent(s2cMSG);
        //更新目标位置
        destination = fsm.moveCMD.CMD.MousePosition.ToVector3();
        fsm.agent.SetDestination(destination);
    }

    public override void HandleSkillEvent(BattleUserInputS2C s2cMSG)
    {
        base.HandleSkillEvent(s2cMSG);
        //进入技能状态
        fsm.ToNext(FSMState.Skill);
    }

    public override void RemoveListener()
    {
        base.RemoveListener();
    }

    public override void Update()
    {
        base.Update();
        if(isMove)
        {
            if(fsm.agent.pathStatus!=NavMeshPathStatus.PathComplete)
            {
                Debug.LogError($"路径不完整，{fsm.agent.pathStatus}");
                fsm.ToNext(FSMState.Idle);
                return;
            }
            float des = Vector3.Distance(this.transform.position, destination);
            if (des<=0.15f)
            {
                fsm.agent.SetDestination(transform.position);
                fsm.ToNext(FSMState.Idle);
                //停止 服务器去做预判 然后发送停止移动的指令给客户端
            }
        }
    }
}
