using ProtoMsg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//װ��
public class equipEntity
{
    public int ID;//ID
    public int Count;//����
    public string Name;//����
    public string Descroption;//����
    public float HP;//����
    public float MP;//ħ��ֵ
    public float Power;//������
    public float Spells;//��ǿ
    public float Armor;//����
    public float MagicResistance;//ħ��
    public float ArmorBreaking;//�Ƽ�
    public float PierceThrough;//��͸
    public float Crit;//����
    public float MoveSpeed;//����
    public float AttackSpeed;//����
    public float CoolingShrinkage;//��ȴ����
    public int Cost;//����Ҫ�Ľ��
    public int SpeiskillID;//����Ч����ID

    public equipEntity(int iD, int count, string name, string descroption, float hP, float mP, float power, float spells, float armor, float magicResistance, float armorBreaking, float pierceThrough, float crit, float moveSpeed, float attackSpeed, float coolingShrinkage, int cost, int speiskillID)
    {
        ID = iD;
        Count = count;
        Name = name;
        Descroption = descroption;
        HP = hP;
        MP = mP;
        Power = power;
        Spells = spells;
        Armor = armor;
        MagicResistance = magicResistance;
        ArmorBreaking = armorBreaking;
        PierceThrough = pierceThrough;
        Crit = crit;
        MoveSpeed = moveSpeed;
        AttackSpeed = attackSpeed;
        CoolingShrinkage = coolingShrinkage;
        Cost = cost;
        SpeiskillID = speiskillID;
    }
}
