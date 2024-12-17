using Game.Model;
using Game.View;
using ProtoMsg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playerbag : MonoBehaviour
{
    //玩家的背包里面的装备
    private List<equipEntity> equipEntityMents;
    private PlayerCtrl playerCtrl;
    private BattleWindow battleWindow;
    private float playerMoney;
    private float timeval;
    private HeroAttributeEntity endAttribut;//最终属性
    private int HeroID;//英雄编号
    public void Init(PlayerCtrl playerCtrl)
    {
        this.playerCtrl = playerCtrl;
        equipEntityMents = new List<equipEntity>();
        playerMoney = 500;
        timeval = 0;
        endAttribut = new HeroAttributeEntity();
        HeroID = playerCtrl.playerInfo.HeroID;
        if(playerCtrl.isSelf==true)
        {
            battleWindow = (BattleWindow)WindowManager.Instance.GetWindow(WindowType.BattleWindow);
        }
    }
    private void FixedUpdate()
    {
        timeval += Time.deltaTime;
        if(timeval>=1)
        {
            timeval = 0;
            ++playerMoney;
        }
    }
    //处理网络消息
    public void HandleEquip(BattleUserEquipS2C s2cMSG)
    {
        equipEntityMents.Add(StoreEquiModel.Instance.GetEquip(s2cMSG.Equip.ID));
        //更新装备栏
        UpdateEquip();
        //更新属性
        UpdateAttrHero();
    }
    //更新装备栏 以及属性
    public void UpdateEquip()
    {
        if(playerCtrl.isSelf==false)
        {
            return;
        }
        for(int i=0;i<equipEntityMents.Count;i++)
        {
            switch(i)
            {
                case 0:
                    battleWindow.equip1.sprite = StoreEquiModel.Instance.GetSprite(equipEntityMents[i].ID);
                    battleWindow.equip1.gameObject.SetActive(true);
                    break;
                case 1:
                    battleWindow.equip2.sprite = StoreEquiModel.Instance.GetSprite(equipEntityMents[i].ID);
                    battleWindow.equip2.gameObject.SetActive(true);
                    break;
                case 2:
                    battleWindow.equip3.sprite = StoreEquiModel.Instance.GetSprite(equipEntityMents[i].ID);
                    battleWindow.equip3.gameObject.SetActive(true);
                    break;
                case 3:
                    battleWindow.equip4.sprite = StoreEquiModel.Instance.GetSprite(equipEntityMents[i].ID);
                    battleWindow.equip4.gameObject.SetActive(true);
                    break;
                case 4:
                    battleWindow.equip5.sprite = StoreEquiModel.Instance.GetSprite(equipEntityMents[i].ID);
                    battleWindow.equip5.gameObject.SetActive(true);
                    break;
                case 5:
                    battleWindow.equip6.sprite = StoreEquiModel.Instance.GetSprite(equipEntityMents[i].ID);
                    battleWindow.equip6.gameObject.SetActive(true);
                    break;
                default:
                    break;
            }       
        }
    }
    //更新属性
    public void UpdateAttrHero()
    {
        endAttribut = HeroAttributeConfig.GetInstance(HeroID);
        for (int i = 0; i < equipEntityMents.Count; i++)
        {
            //属性的计算
            endAttribut.HP += equipEntityMents[i].HP;
            endAttribut.MP += equipEntityMents[i].MP;
            endAttribut.Power += equipEntityMents[i].Power;
            endAttribut.Spells += equipEntityMents[i].Spells;
        }
        playerCtrl.totalAttribute = endAttribut;
        playerCtrl.currentAttribute = endAttribut;
        playerCtrl.HUDUpdate(false);
        playerCtrl.battleWindow.UpateAttrature();
    }
    //获取玩家有多少金钱
    public int GetplayerMoney()
    {
        return (int)playerMoney; 
    }
    //获取玩家有多少件装备
    public int GetEquipCount()
    {
        return equipEntityMents.Count;
    }
    public void CostMoney(int Cost)
    {
        playerMoney -= Cost;
    }
}
