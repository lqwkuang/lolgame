using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resmanager : Singleton<Resmanager>
{
    //����UI����
    public GameObject LoadUI(string path)
    {
        GameObject go= Resources.Load<GameObject>($"UIPrefab/{path}");
        if(go==null)
        {
            Debug.LogError($"û���ҵ�UI����{path}");
            return null;
        }
        GameObject obj = GameObject.Instantiate(go);
        return obj;
    }
    //����Բ��ͷ��ͼƬ
    public Sprite LoadRoundHead(string path)
    {
        return Resources.Load<Sprite>($"Image/Round/{path}");
    }
    //�����ٻ�ʦ����
    public Sprite LoadGeneralSkill(int skillID)
    {
        return Resources.Load<Sprite>($"Image/GeneralSkill/{skillID}");
    }
    //����Ӣ��ԭ��
    public Sprite LoadHeroTexture(int heroID)
    {
        return Resources.Load<Sprite>($"Image/HeroTexture/{heroID}");
    }
    //����Ӣ��
    public GameObject LoadHero(int heroID)
    {
        GameObject go=Resources.Load<GameObject>($"Hero/{heroID}/Model/{heroID}");
        return GameObject.Instantiate(go);
    }
    //����Ұ��
    public GameObject LoadCreep(int creepID)
    {
        GameObject go = Resources.Load<GameObject>($"Creeps/{creepID}creep");
        return GameObject.Instantiate(go);
    }
    //����HUD
    public GameObject LoadHUD()
    {
        GameObject go = Resources.Load<GameObject>($"HUD/HeroHead");
        return GameObject.Instantiate(go);
    }
    //������Ч��Դ
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
    //����װ��ͼƬ
    public Sprite LoadEquipIcon(int ID)
    {
        return Resources.Load<Sprite>($"Image/equip/{ID}");
    }
    //����С��ͼ��Դ
    public GameObject LoadMinmapIcon(string name)
    {
        return GameObject.Instantiate(Resources.Load<GameObject>($"Minmapicon/{name}"));
    }
}
