using FogOfWar.CPU;
using FogOfWar.Demo;
using Game.Ctrl;
using Game.Model;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class fieldEye : MonoSingleton<fieldEye>
{
    public int teamID;
    public FieldManagerTest fieldManager;
    /// <summary>ÿ���ݴ���ռ������������,id�Ǳ�� </summary>
    public Dictionary<int, List<Tuple<int, int>>> GlassDic;
    private Glassdata glassdata1;
    public void Init()
    {
        fieldManager = FindObjectOfType<FieldManagerTest>();
        //fieldManager = GameObject.Find("Ground").GetComponent<FieldManagerTest>();
        teamID = RoomCtrl.Instance.GetTeamID(PlayerModel.Instance.rolesInfo.RolesID);
        if (teamID == 0)
        {
            //����A
            GameObject[] tempos = GameObject.FindGameObjectsWithTag("Atower");
            for (int i = 0; i < tempos.Length; i++)
            {
                tempos[i].AddComponent<VisualFieldObject>();
            }

        }
        else if (teamID == 1)
        {
            //����B
            GameObject[] tempos = GameObject.FindGameObjectsWithTag("Btower");
            for (int i = 0; i < tempos.Length; i++)
            {
                tempos[i].AddComponent<VisualFieldObject>();
            }
        }
        GlassDic = new Dictionary<int, List<Tuple<int, int>>>();
        string jsonpth = Path.Combine(Application.persistentDataPath, "Glassdata");
        if (File.Exists(jsonpth))
        {
            string json = File.ReadAllText(jsonpth);
            glassdata1 = JsonUtility.FromJson<Glassdata>(json);
        }
        for (int i = 0; i < glassdata1.x.Count; i++)
        {
            if (GlassDic.ContainsKey(glassdata1.ID[i]))
            {
                GlassDic[glassdata1.ID[i]].Add(new Tuple<int, int>(glassdata1.x[i], glassdata1.z[i]));//�ѲݴԵ��������������
            }
            else
            {
                GlassDic[glassdata1.ID[i]] = new List<Tuple<int, int>>();
                GlassDic[glassdata1.ID[i]].Add(new Tuple<int, int>(glassdata1.x[i], glassdata1.z[i]));//�ѲݴԵ��������������
            }

        }
    }
    /// <summary>
    /// ����GlassesID����Grids(ȥ��)
    /// </summary>
    /// <param name="GlassID"></param>
    public void UpdateGraidDete(int GlassID)
    {
        for (int i = 0; i < GlassDic[GlassID].Count; i++)
        {
            int x = GlassDic[GlassID][i].Item1;
            int z = GlassDic[GlassID][i].Item2;
            fieldManager.grids[x, z] = GridType.Ground;
        }
    }
    /// <summary>
    /// ����GlassesID����Grids(���)
    /// </summary>
    /// <param name="GlassID"></param>
    public void UpdateGraidAdd(int GlassID)
    {
        for (int i = 0; i < GlassDic[GlassID].Count; i++)
        {
            int x = GlassDic[GlassID][i].Item1;
            int z = GlassDic[GlassID][i].Item2;
            fieldManager.grids[x, z] = GridType.Glass;
        }
    }
}
