using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Ұ�ֺ�С���Ŀ���(���ɣ�����)
public class CreepManager : Singleton<CreepManager>
{
    private List<Vector3> PosVec3;
    private List<Vector3> RoateVec3;
    private List<GameObject> Allcreep;
    public void Init()
    {
        PosVec3 = new List<Vector3>();
        RoateVec3 = new List<Vector3>();
        Allcreep = new List<GameObject>();
        PosVec3.Add(new Vector3(-67,0,-33));
        RoateVec3.Add(new Vector3(0,212,0));
        //����Ұ��һ
        GameObject obj= Resmanager.Instance.LoadCreep(1001);
        Allcreep.Add(obj);
        //��ʼ��
        obj.GetComponent<OneCreepFsm>().Poscenter = PosVec3[0];
        obj.GetComponent<OneCreepFsm>().LookatVec3 = RoateVec3[0];
        obj.GetComponent<OneCreepFsm>().Init();
        
    }
}
