using ProtoMsg;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillManager : MonoBehaviour
{
    PlayerCtrl playerCtrl;
    PlayerInfo playerInfo;
    //����������Ϣ
    Dictionary<KeyCode, int> skillID = new Dictionary<KeyCode, int>();
    //������ȴ����Ϣ
    Dictionary<KeyCode, float> coolingConfig = new Dictionary<KeyCode, float>();
    public void Init(PlayerCtrl playerCtrl)
    {
        this.playerCtrl = playerCtrl;
        playerInfo = playerCtrl.playerInfo;

        //����������Ϣ
        HeroSkillEntity skill= HeroSkillConfig.GetInstance(playerInfo.HeroID);
        skillID[KeyCode.Q] = skill.Q_ID;
        skillID[KeyCode.W] = skill.W_ID;
        skillID[KeyCode.E] = skill.E_ID;
        skillID[KeyCode.R] = skill.R_ID;
        skillID[KeyCode.A] = skill.A_ID;

        //d f
        skillID[KeyCode.D] = playerInfo.SkillA;
        skillID[KeyCode.F] = playerInfo.SkillB;
        skillID[KeyCode.B] = 4;

        //������ȴ��Ϣ
        coolingConfig[KeyCode.Q] = AllSkillConfig.Get(skill.Q_ID).CoolingTime;
        coolingConfig[KeyCode.W] = AllSkillConfig.Get(skill.W_ID).CoolingTime;
        coolingConfig[KeyCode.E] = AllSkillConfig.Get(skill.E_ID).CoolingTime;
        coolingConfig[KeyCode.R] = AllSkillConfig.Get(skill.R_ID).CoolingTime;
        coolingConfig[KeyCode.A] = 0.5f;

        //d f
        coolingConfig[KeyCode.D] = 180;
        coolingConfig[KeyCode.F] = 180;
        coolingConfig[KeyCode.B] = 4;//�س�ʱ��

        //���һ�ΰ��µ�ʱ��
    }
    //���¼��ܵ�ʱ��
    Dictionary<KeyCode, float> keyDownTime = new Dictionary<KeyCode, float>();
    public void DoCooling(KeyCode key,Action<float> action)
    {
        keyDownTime[key] = Time.time;
        if(action!=null)
        {
            action(keyDownTime[key]);
        }
    }
    //ʣ�����ȴ��ʱ��
    public float SurplusTime(KeyCode key)
    {
        float preTime=0;
        if(keyDownTime.ContainsKey(key)==true)
        {
            //
            preTime = keyDownTime[key];
        }
        float time = coolingConfig[key]- Time.time + preTime;
        if(time<0)
        {
            time = 0;
        }
        return time;
    }
    //�Ƿ���ȴ���
    public bool IsCooling(KeyCode key)
    {
        return SurplusTime(key)>0;
    }
    //�õ�����������Ϣ
    public int GetID(KeyCode key)
    {
        return skillID[key];
    }
    //ʣ��ʱ��İٷֱ�
    public float SurplusTimeprecent(KeyCode key)
    {
        return SurplusTime(key) / coolingConfig[key];
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
