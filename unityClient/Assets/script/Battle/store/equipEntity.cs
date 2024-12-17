using ProtoMsg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//装备
public class equipEntity
{
    public int ID;//ID
    public int Count;//数量
    public string Name;//名称
    public string Descroption;//描述
    public float HP;//生命
    public float MP;//魔法值
    public float Power;//攻击力
    public float Spells;//法强
    public float Armor;//护甲
    public float MagicResistance;//魔抗
    public float ArmorBreaking;//破甲
    public float PierceThrough;//穿透
    public float Crit;//暴击
    public float MoveSpeed;//移速
    public float AttackSpeed;//攻速
    public float CoolingShrinkage;//冷却收缩
    public int Cost;//所需要的金币
    public int SpeiskillID;//特殊效果的ID

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
