using FogOfWar.CPU;
using Game.Ctrl;
using Game.Model;
using ProtoMsg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoSingleton<BattleManager>
{
    //BattleSceneConfig
    //10个英雄的位置
    //野区野怪的配置 防御塔的配置
    //出生的位置
    public Vector3[] spawnPosition = new Vector3[10]
    {
        //A队伍的位置
        new Vector3(-6.25f,0,-8.96f),
        new Vector3(-2.26f,0,-3.71f),
        new Vector3(-6.71f,0,-4.01f),
        new Vector3(-4.28f,0,-5.89f),
        new Vector3(-2.02f,0,-8.23f),
        //B队伍的位置
        new Vector3(-95.198f,0,-96.542f),
        new Vector3(-99.892f,0,-101.4f),
        new Vector3(-95.432f,0,-101.49f),
        new Vector3(-97.962f,0,-99.409f),
        new Vector3(-99.7443f,0,-96.884f),
    };
    //初始的角度
    public Vector3[] spawnRotation = new Vector3[10]
    {
        //A队伍的角度
        new Vector3(0,242.49f,0),
        new Vector3(0,-122.251f,0),
        new Vector3(0,-152.659f,0),
        new Vector3(0,230.56f,0),
        new Vector3(0,-149.089f,0),
        //B队伍的角度
        new Vector3(0,67.403f,0),
        new Vector3(0,-297.338f,0),
        new Vector3(0,-327.746f,0),
        new Vector3(0,55.473f,0),
        new Vector3(0,-324.176f,0),
    };
    //存储每个角色的控制器
    public Dictionary<int, PlayerCtrl> playerCtrlDIC = new Dictionary<int, PlayerCtrl>();
    //初始化工作
    //确定是否初始化完成
    bool isInit = false;
    private void Awake()
    { 
        Setinstanc(this);
        //视野检测初始化
        //fieldRay.Instance.Init();
        //对象池初始化
        GamePool.Instance.Init();
        //取得缓存的数据
        foreach (var item in RoomModel.Instance.playerInfos)
        {
            //创建角色
           GameObject hero= Resmanager.Instance.LoadHero(item.HeroID);
            //设置名称
            hero.name = $"Hero{item.PosID}";
            //设置它的位置
            hero.transform.position = spawnPosition[item.PosID];
            hero.transform.rotation = Quaternion.Euler(spawnRotation[item.PosID]);
            //添加控制器
            PlayerCtrl playerCtrl= hero.AddComponent<PlayerCtrl>();
            //缓存起来
            playerCtrlDIC[item.RolesInfo.RolesID] = playerCtrl;
            RoomModel.Instance.playerObjects[item.RolesInfo.RolesID] = hero;
            //每个角色自己要初始化 每个控制器
            playerCtrl.Init(item);
            //对象池创建
            GamePool.Instance.Addobj($"{item.HeroID}Q",Resmanager.Instance.LoadEffect(item.HeroID,"Q"));
            GamePool.Instance.Addobj($"{item.HeroID}W", Resmanager.Instance.LoadEffect(item.HeroID, "W"));
            GamePool.Instance.Addobj($"{item.HeroID}E", Resmanager.Instance.LoadEffect(item.HeroID, "E"));
            GamePool.Instance.Addobj($"{item.HeroID}R", Resmanager.Instance.LoadEffect(item.HeroID, "R"));
            GamePool.Instance.Addobj($"{item.HeroID}A", Resmanager.Instance.LoadEffect(item.HeroID, "A"));
            //判断阵营初始化 视野系统
            if (item.TeamID==fieldEye.Instance.teamID)
            {
                //fieldRay.Instance.Addpos(playerCtrl.transform);
                //添加光照
            }
            else
            {
                //添加目标                
                //fieldRay.Instance.Addtarget(playerCtrl.transform);
            }
            
        }
        //野怪初始化
        CreepManager.Instance.Init();

        //视野初始化
        fieldEye.Instance.Init();
        //Camera.main.GetComponent<VisualFieldRecorder>().Init(); 
        
        //初始化装备信息
        StoreEquiModel.Instance.Init();
        //buffer管理器初始化
        bufferManager.Instance.Init();
        //加载战斗界面
        WindowManager.Instance.OpenWindow(WindowType.BattleWindow);
        //输入管理器的初始化
        this.gameObject.AddComponent<InputCtrl>().Init(playerCtrlDIC[PlayerModel.Instance.rolesInfo.RolesID]);
        isInit = true;
    }

    //发送网络事件的玩家的FsM 处理他的事件
    public void  HandleCMD(BattleUserInputS2C s2cMSG)
    {
        //先确定 这条操作命令是哪个玩家发的
        //然后调用它的角色控制器-FSM状态机 去处理这个事件
        playerCtrlDIC[s2cMSG.CMD.RolesID].playerFSM.HandleCMD(s2cMSG);
    }
    // 每帧取网络事件进行播放
    void Update()
    {
        if(isInit)
        {
            //输出的控制 是从缓存的帧数据
            BattleListener.Instance.PlayerFrame(HandleCMD);
        }
    }

    private void OnDestroy()
    {
        RoomModel.Instance.Clear();
    }
}
