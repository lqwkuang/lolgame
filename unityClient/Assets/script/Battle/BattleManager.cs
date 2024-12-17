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
    //10��Ӣ�۵�λ��
    //Ұ��Ұ�ֵ����� ������������
    //������λ��
    public Vector3[] spawnPosition = new Vector3[10]
    {
        //A�����λ��
        new Vector3(-6.25f,0,-8.96f),
        new Vector3(-2.26f,0,-3.71f),
        new Vector3(-6.71f,0,-4.01f),
        new Vector3(-4.28f,0,-5.89f),
        new Vector3(-2.02f,0,-8.23f),
        //B�����λ��
        new Vector3(-95.198f,0,-96.542f),
        new Vector3(-99.892f,0,-101.4f),
        new Vector3(-95.432f,0,-101.49f),
        new Vector3(-97.962f,0,-99.409f),
        new Vector3(-99.7443f,0,-96.884f),
    };
    //��ʼ�ĽǶ�
    public Vector3[] spawnRotation = new Vector3[10]
    {
        //A����ĽǶ�
        new Vector3(0,242.49f,0),
        new Vector3(0,-122.251f,0),
        new Vector3(0,-152.659f,0),
        new Vector3(0,230.56f,0),
        new Vector3(0,-149.089f,0),
        //B����ĽǶ�
        new Vector3(0,67.403f,0),
        new Vector3(0,-297.338f,0),
        new Vector3(0,-327.746f,0),
        new Vector3(0,55.473f,0),
        new Vector3(0,-324.176f,0),
    };
    //�洢ÿ����ɫ�Ŀ�����
    public Dictionary<int, PlayerCtrl> playerCtrlDIC = new Dictionary<int, PlayerCtrl>();
    //��ʼ������
    //ȷ���Ƿ��ʼ�����
    bool isInit = false;
    private void Awake()
    { 
        Setinstanc(this);
        //��Ұ����ʼ��
        //fieldRay.Instance.Init();
        //����س�ʼ��
        GamePool.Instance.Init();
        //ȡ�û��������
        foreach (var item in RoomModel.Instance.playerInfos)
        {
            //������ɫ
           GameObject hero= Resmanager.Instance.LoadHero(item.HeroID);
            //��������
            hero.name = $"Hero{item.PosID}";
            //��������λ��
            hero.transform.position = spawnPosition[item.PosID];
            hero.transform.rotation = Quaternion.Euler(spawnRotation[item.PosID]);
            //��ӿ�����
            PlayerCtrl playerCtrl= hero.AddComponent<PlayerCtrl>();
            //��������
            playerCtrlDIC[item.RolesInfo.RolesID] = playerCtrl;
            RoomModel.Instance.playerObjects[item.RolesInfo.RolesID] = hero;
            //ÿ����ɫ�Լ�Ҫ��ʼ�� ÿ��������
            playerCtrl.Init(item);
            //����ش���
            GamePool.Instance.Addobj($"{item.HeroID}Q",Resmanager.Instance.LoadEffect(item.HeroID,"Q"));
            GamePool.Instance.Addobj($"{item.HeroID}W", Resmanager.Instance.LoadEffect(item.HeroID, "W"));
            GamePool.Instance.Addobj($"{item.HeroID}E", Resmanager.Instance.LoadEffect(item.HeroID, "E"));
            GamePool.Instance.Addobj($"{item.HeroID}R", Resmanager.Instance.LoadEffect(item.HeroID, "R"));
            GamePool.Instance.Addobj($"{item.HeroID}A", Resmanager.Instance.LoadEffect(item.HeroID, "A"));
            //�ж���Ӫ��ʼ�� ��Ұϵͳ
            if (item.TeamID==fieldEye.Instance.teamID)
            {
                //fieldRay.Instance.Addpos(playerCtrl.transform);
                //��ӹ���
            }
            else
            {
                //���Ŀ��                
                //fieldRay.Instance.Addtarget(playerCtrl.transform);
            }
            
        }
        //Ұ�ֳ�ʼ��
        CreepManager.Instance.Init();

        //��Ұ��ʼ��
        fieldEye.Instance.Init();
        //Camera.main.GetComponent<VisualFieldRecorder>().Init(); 
        
        //��ʼ��װ����Ϣ
        StoreEquiModel.Instance.Init();
        //buffer��������ʼ��
        bufferManager.Instance.Init();
        //����ս������
        WindowManager.Instance.OpenWindow(WindowType.BattleWindow);
        //����������ĳ�ʼ��
        this.gameObject.AddComponent<InputCtrl>().Init(playerCtrlDIC[PlayerModel.Instance.rolesInfo.RolesID]);
        isInit = true;
    }

    //���������¼�����ҵ�FsM ���������¼�
    public void  HandleCMD(BattleUserInputS2C s2cMSG)
    {
        //��ȷ�� ���������������ĸ���ҷ���
        //Ȼ��������Ľ�ɫ������-FSM״̬�� ȥ��������¼�
        playerCtrlDIC[s2cMSG.CMD.RolesID].playerFSM.HandleCMD(s2cMSG);
    }
    // ÿ֡ȡ�����¼����в���
    void Update()
    {
        if(isInit)
        {
            //����Ŀ��� �Ǵӻ����֡����
            BattleListener.Instance.PlayerFrame(HandleCMD);
        }
    }

    private void OnDestroy()
    {
        RoomModel.Instance.Clear();
    }
}
