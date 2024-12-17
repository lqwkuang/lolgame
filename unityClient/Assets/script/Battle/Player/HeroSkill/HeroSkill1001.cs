using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroSkill1001 : baseHeroSkill
{
    public override int SkillE()
    {
        if(targettype==Targettype.hero)
        {
            //80%法强 3秒的50%减速
            curttart = transform.GetComponent<PlayerCtrl>().currentAttribute;
            int hurt = (int)(0.8f * curttart.Spells);
            //特殊效果 3秒50%减速
            SlowspeedBuffer buffer= target.AddComponent<SlowspeedBuffer>();
            buffer.Init(target, 3);
            buffer.setRange(0.5f);
            return hurt;
        }
        else if(targettype==Targettype.creep)
        {
            //80%法强 3秒的50%减速
            curttart = transform.GetComponent<PlayerCtrl>().currentAttribute;
            int hurt = (int)(0.8f * curttart.Spells);
            //特殊效果 3秒50%减速
           /* SlowspeedBuffer buffer = target.AddComponent<SlowspeedBuffer>();
            buffer.Init(target, 3);
            buffer.setRange(0.5f);*/
            return hurt;
        }
        else
        {
            return 0;
        }
       
    }

    public override int SkillQ()
    {
        if (targettype == Targettype.hero)
        {
            //Q 50%的攻击力 50%的法强 1.5秒的眩晕
            curttart = transform.GetComponent<PlayerCtrl>().currentAttribute;
            int hurt = (int)(0.5f * curttart.Power + 0.5f * curttart.Spells);
            //添加特殊效果
            buffer = target.AddComponent<dizzybuffer>();
            buffer.Init(target, 1.5f);
            return hurt;
        }
        else if (targettype == Targettype.creep)
        {
            //Q 50%的攻击力 50%的法强 1.5秒的眩晕
            curttart = transform.GetComponent<PlayerCtrl>().currentAttribute;
            int hurt = (int)(0.5f * curttart.Power + 0.5f * curttart.Spells);
            //添加特殊效果
            /*buffer = target.AddComponent<dizzybuffer>();
            buffer.Init(target, 1.5f);*/
            return hurt;
        }
        else
        {
            return 0;
        }
                   
    }

    public override int SkillR()
    {
        if (targettype == Targettype.hero)
        {
            return 50;
        }
        else if (targettype == Targettype.creep)
        {
            return 50;
        }
        else
            return 0;
                    
    }

    public override int SkillW()
    {
        if (targettype == Targettype.hero)
        {
            //W 150%的法强
            curttart = transform.GetComponent<PlayerCtrl>().currentAttribute;
            int hurt = (int)(1.5f * curttart.Spells);
            return hurt;
        }
        else if (targettype == Targettype.creep)
        {
            //W 150%的法强
            curttart = transform.GetComponent<PlayerCtrl>().currentAttribute;
            int hurt = (int)(1.5f * curttart.Spells);
            return hurt;
        }
        else
        {
            return 0;
        }
                   
    }
    public override int SkillA()
    {
        if (targettype == Targettype.hero)
        {
            //平A是法强
            curttart = transform.GetComponent<PlayerCtrl>().currentAttribute;
            return (int)curttart.Spells;
        }
        else if (targettype == Targettype.creep)
        {
            //平A是法强
            curttart = transform.GetComponent<PlayerCtrl>().currentAttribute;
            return (int)curttart.Spells;
        }
        else
            return 0;
                  
    }
}
