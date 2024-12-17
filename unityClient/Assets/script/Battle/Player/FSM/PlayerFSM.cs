using ProtoMsg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 角色有限状态机的管理类
/// </summary>
public class PlayerFSM : MonoBehaviour
{
    Dictionary<FSMState, EntityFSM> playerState = new Dictionary<FSMState, EntityFSM>();
    FSMState currentState = FSMState.None;

    public PlayerCtrl playerCtrl;

    public NavMeshAgent agent;//寻路导航的组件
    //初始化
    public void Init(PlayerCtrl playerCtrl) {
        this.playerCtrl = playerCtrl;
        //A*寻路的插件 放在服务器 地图的信息：网格信息 障碍物
        agent = this.transform.GetComponent<NavMeshAgent>();

        playerState[FSMState.Idle] = new PlayerIdIe(transform, this);
        playerState[FSMState.Move] = new PlayerMove(transform, this);
        playerState[FSMState.Skill] = new PlayerSkill(transform, this);
        playerState[FSMState.Relive] = new PlayerRelive(transform, this);
        playerState[FSMState.Dead] = new PlayerDead(transform, this);
        playerState[FSMState.Dance] = new Playerdizzy(transform, this);
        ToNext(FSMState.Idle);
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.A))
        //{
        //    ToNext(FSMState.Move);
        //}
        if(currentState!=FSMState.None)
        {
            playerState[currentState].Update();
        }
        
    }
    //切换到下一状态
    public void ToNext(FSMState nextState) {
        if (currentState!=FSMState.None)
        {
            playerState[currentState].Exit();
        }
        playerState[nextState].Enter();
        currentState = nextState;
    }

    public BattleUserInputS2C moveCMD;
    public BattleUserInputS2C skillCMD;

    //处理网络消息
    public void HandleCMD(BattleUserInputS2C s2cMSG)
    {
        playerState[currentState].HandleCMD(s2cMSG);
    }
}
