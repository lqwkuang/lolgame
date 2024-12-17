using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Model
{
    public class StoreEquiModel:Singleton<StoreEquiModel>
    {
        //0 �Ƕ����� 1�Ƕ����� 2��Ь�� 3����ȡ 4������
        public List<equipEntity> AllequipMents;//�������͵�װ��
        public List<Sprite> ALLequipSprit;//����ͼƬ
        public void Init()
        {

            AllequipMents = new List<equipEntity>();
            AllequipMents.Add(new equipEntity(0, 1, "������", "����������Ի����ָ�80������ֵ\r\n300����", 300, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 450, 0));
            AllequipMents.Add(new equipEntity(1, 1, "������", "ɱ��֮��", 0, 0, 10, 0, 0, 0, 0, 0, 0, 0, 0, 0, 450, 0));
            AllequipMents.Add(new equipEntity(2, 1, "Ь��", "������", 0, 0, 0, 0, 0, 0, 0, 0, 0, 60, 0, 0, 300, 0));
            AllequipMents.Add(new equipEntity(3, 1, "��ȡ", "������", 0, 0, 0, 0, 0, 0, 0, 0, 0, 7, 0, 0, 450, 0));
            AllequipMents.Add(new equipEntity(4, 1, "����", "�����ʵ��Ұ", 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 75, 10));//������

            //ͼƬ�洢
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
