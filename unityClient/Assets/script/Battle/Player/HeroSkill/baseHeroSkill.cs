using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Targettype//���ֹ�������ʲô���͵�����
{
    hero,//������Ӣ��
    creep//������Ұ��
}
public class baseHeroSkill : MonoBehaviour
{
    public HeroAttributeEntity curttart;
    public basebuffer buffer;
    public GameObject target;
    public Targettype targettype;
    
    public int InputCmd(KeyCode keyCode)
    {
        if(keyCode==KeyCode.Q)
        {
            return SkillQ();
        }
        else if(keyCode==KeyCode.W)
        {
            return SkillW();
        }
        else if(keyCode==KeyCode.E)
        {
            return SkillE();
        }
        else if(keyCode==KeyCode.R)
        {
            return SkillR();
        }
        else
        {
            return 0;
        }
    }
    public virtual int SkillQ()
    {
        return 0;
    }
    public virtual int SkillW()
    {
        return 0;
    }
    public virtual int SkillE()
    {
        return 0;
    }
    public virtual int SkillR()
    {
        return 0;
    }
    public virtual int SkillA()
    {
        return 0;
    }
}
