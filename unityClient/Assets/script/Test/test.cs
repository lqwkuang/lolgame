using Game.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    public List<equipEntity> equips=new List<equipEntity>();
    // Start is called before the first frame update
    void Start()
    {
        StoreEquiModel.Instance.Init();
        equips.Add(StoreEquiModel.Instance.GetEquip(0));
        equips.Add(StoreEquiModel.Instance.GetEquip(0));
        equips.Add(StoreEquiModel.Instance.GetEquip(0));
        equips[0].ID = 100;
        Debug.Log($"0{equips[0].ID}---1{equips[1].ID}-----2{equips[2].ID}");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
}
