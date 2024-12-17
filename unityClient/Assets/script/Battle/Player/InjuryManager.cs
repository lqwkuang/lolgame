using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InjuryManager : Singleton<InjuryManager>
{
    //计算伤害
    //type 0 是物理 1是魔法 2是真实
    public float hitInjury(HeroAttributeEntity player, HeroAttributeEntity hitplayer,float hurt,int type)
    {
        if (type == 0)
        {
            float curArmor = hitplayer.Armor - player.ArmorBreaking;
            return (1-curArmor / (curArmor + 100)) * hurt;
        }
        else if(type == 1)
        {
            float curArmor = hitplayer.MagicResistance - player.ArmorBreaking;
            return (1-curArmor / (curArmor + 100)) * hurt;
        }
        else if(type==2)
        {
            return hurt;
        }
        else
        {
            return 0;
        }
            
    }
    //野怪伤害
    public float hitInjuryCreep(HeroAttributeEntity player,int hurt,int type)
    {
        if (type == 0)
        {
            float curArmor = player.Armor;
            return (1 - curArmor / (curArmor + 100)) * hurt;
        }
        else if (type == 1)
        {
            float curArmor = player.MagicResistance ;
            return (1 - curArmor / (curArmor + 100)) * hurt;
        }
        else if (type == 2)
        {
            return hurt;
        }
        else
        {
            return 0;
        }
    }
}
