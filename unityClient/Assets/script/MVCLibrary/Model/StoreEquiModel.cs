using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Model
{
    public class StoreEquiModel:Singleton<StoreEquiModel>
    {
        //0 是多兰盾 1是多兰剑 2是鞋子 3是萃取 4是真眼
        public List<equipEntity> AllequipMents;//所有类型的装备
        public List<Sprite> ALLequipSprit;//所有图片
        public void Init()
        {

            AllequipMents = new List<equipEntity>();
            AllequipMents.Add(new equipEntity(0, 1, "多兰盾", "被攻击后可以缓慢恢复80的生命值\r\n300生命", 300, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 450, 0));
            AllequipMents.Add(new equipEntity(1, 1, "多兰剑", "杀人之剑", 0, 0, 10, 0, 0, 0, 0, 0, 0, 0, 0, 0, 450, 0));
            AllequipMents.Add(new equipEntity(2, 1, "鞋子", "飞起来", 0, 0, 0, 0, 0, 0, 0, 0, 0, 60, 0, 0, 300, 0));
            AllequipMents.Add(new equipEntity(3, 1, "萃取", "额外金币", 0, 0, 0, 0, 0, 0, 0, 0, 0, 7, 0, 0, 450, 0));
            AllequipMents.Add(new equipEntity(4, 1, "真眼", "获得真实视野", 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 75, 10));//测试用

            //图片存储
            ALLequipSprit = new List<Sprite>();
            ALLequipSprit.Add(Resmanager.Instance.LoadEquipIcon(0));
            ALLequipSprit.Add(Resmanager.Instance.LoadEquipIcon(1));
            ALLequipSprit.Add(Resmanager.Instance.LoadEquipIcon(2));
            ALLequipSprit.Add(Resmanager.Instance.LoadEquipIcon(3));
            ALLequipSprit.Add(Resmanager.Instance.LoadEquipIcon(4));
        }
        public equipEntity GetEquip(int Id)
        {
            return AllequipMents[Id];
        }
        public Sprite GetSprite(int ID)
        {
            return ALLequipSprit[ID];
        }
    }

}
