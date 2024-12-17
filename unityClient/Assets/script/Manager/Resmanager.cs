using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resmanager : Singleton<Resmanager>
{
    //加载UI窗体
    public GameObject LoadUI(string path)
    {
        GameObject go= Resources.Load<GameObject>($"UIPrefab/{path}");
        if(go==null)
        {
            Debug.LogError($"没有找到UI窗体{path}");
            return null;
        }
        GameObject obj = GameObject.Instantiate(go);
        return obj;
    }
    //加载圆形头像图片
    public Sprite LoadRoundHead(string path)
    {
        return Resources.Load<Sprite>($"Image/Round/{path}");
    }
    //加载召唤师技能
    public Sprite LoadGeneralSkill(int skillID)
    {
        return Resources.Load<Sprite>($"Image/GeneralSkill/{skillID}");
    }
    //加载英雄原画
    public Sprite LoadHeroTexture(int heroID)
    {
        return Resources.Load<Sprite>($"Image/HeroTexture/{heroID}");
    }
    //加载英雄
    public GameObject LoadHero(int heroID)
    {
        GameObject go=Resources.Load<GameObject>($"Hero/{heroID}/Model/{heroID}");
        return GameObject.Instantiate(go);
    }
    //加载野怪
    public GameObject LoadCreep(int creepID)
    {
        GameObject go = Resources.Load<GameObject>($"Creeps/{creepID}creep");
        return GameObject.Instantiate(go);
    }
    //加载HUD
    public GameObject LoadHUD()
    {
        GameObject go = Resources.Load<GameObject>($"HUD/HeroHead");
        return GameObject.Instantiate(go);
    }
    //加载特效资源
    public GameObject LoadEffect(int heroID,string skillName)
    {
        GameObject go = Resources.Load<GameObject>($"Hero/{heroID}/Effect/{skillName}");
        return go;
    }

    public Sprite LoadHeroSkill(int heroID,string skillName)
    {
        return Resources.Load<Sprite>($"Hero/{heroID}/UI_Skill/{skillName}");
    }

    internal Sprite LoadHeadIcon(int heroID)
    {
        return Resources.Load<Sprite>($"Hero/{heroID}/UI_Head/{heroID}");
    }
    //加载装备图片
    public Sprite LoadEquipIcon(int ID)
    {
        return Resources.Load<Sprite>($"Image/equip/{ID}");
    }
    //加载小地图资源
    public GameObject LoadMinmapIcon(string name)
    {
        return GameObject.Instantiate(Resources.Load<GameObject>($"Minmapicon/{name}"));
    }
}
