using Game.Model;
using Game.View;
using ProtoMsg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playerbag : MonoBehaviour
{
    //��ҵı��������װ��
    private List<equipEntity> equipEntityMents;
    private PlayerCtrl playerCtrl;
    private BattleWindow battleWindow;
    private float playerMoney;
    private float timeval;
    private HeroAttributeEntity endAttribut;//��������
    private int HeroID;//Ӣ�۱��
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
    //����������Ϣ
    public void HandleEquip(BattleUserEquipS2C s2cMSG)
    {
        equipEntityMents.Add(StoreEquiModel.Instance.GetEquip(s2cMSG.Equip.ID));
        //����װ����
        UpdateEquip();
        //��������
        UpdateAttrHero();
    }
    //����װ���� �Լ�����
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
    //��������
    public void UpdateAttrHero()
    {
        endAttribut = HeroAttributeConfig.GetInstance(HeroID);
        for (int i = 0; i < equipEntityMents.Count; i++)
        {
            //���Եļ���
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
    //��ȡ����ж��ٽ�Ǯ
    public int GetplayerMoney()
    {
        return (int)playerMoney; 
    }
    //��ȡ����ж��ټ�װ��
    public int GetEquipCount()
    {
        return equipEntityMents.Count;
    }
    public void CostMoney(int Cost)
    {
        playerMoney -= Cost;
    }
}
