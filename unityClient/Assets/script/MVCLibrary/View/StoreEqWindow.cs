using Game.Ctrl;
using Game.Model;
using Game.Net;
using ProtoMsg;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Game.View
{
    //游戏商店的管理器
    public class StoreEqWindow : BaseWindow
    {
        public int select;//选择的装备的ID
        Playerbag playerbag;
        public StoreEqWindow()
        {
            selfType = WindowType.StoreWindow;
            scenesType = ScenesType.Battle;
            resident = true;
            resName = "UIPrefab/Battle/storeWindow";
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            isCanButEquip();
        }

        Text playermoney,buyText;//金钱 购买字样
        Image showEquipimage;
        Text showEquipText,showeqmoney;

        protected override void Awake()
        {
            base.Awake();
            select = -1;//初始化为-1表示还没有选择
            playermoney = transform.Find("money").GetComponent<Text>();
            showEquipimage = transform.Find("equip/equipdetail/showequip").GetComponent<Image>();
            showeqmoney = transform.Find("equip/equipdetail/showequip/text").GetComponent<Text>();
            buyText = transform.Find("button/buy/Text").GetComponent<Text>();
            showEquipText = transform.Find("equip/equipdetail/text").GetComponent<Text>();
            playerbag = RoomCtrl.Instance.GetSelfPlayerCtrl().gameObject.GetComponent<Playerbag>();
        }

        protected override void OnAddListener()
        {
            base.OnAddListener();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override void OnRemoveListener()
        {
            base.OnRemoveListener();
        }

        protected override void RegisterUIEvent()
        {
            base.RegisterUIEvent();
            for (int i = 0; i < buttonList.Length; i++)
            {
                switch (buttonList[i].name)
                {
                    case "duolandun":
                        buttonList[i].onClick.AddListener(delegate { ShowEqipment(0);});
                        break;
                    case "duolanjian":
                        buttonList[i].onClick.AddListener(delegate { ShowEqipment(1); });
                        break;
                    case "xiezi":
                        buttonList[i].onClick.AddListener(delegate { ShowEqipment(2); });
                        break;
                    case "cuiqu":
                        buttonList[i].onClick.AddListener(delegate { ShowEqipment(3); });
                        break;
                    case "zhenyan":
                        buttonList[i].onClick.AddListener(delegate { ShowEqipment(4); });
                        break;
                    case "sell":
                        break;
                    case "withdraw":
                        break;
                    case "buy":
                        buttonList[i].onClick.AddListener(BuyEquipment);
                        break;
                    case "close":
                        buttonList[i].onClick.AddListener(CloseStore);
                        break;
                    default:
                        break;
                }
            }

        }
        public void CloseStore()
        {
            Close(false);
        }
        public void BuyEquipment()
        {
            if (select == -1)
                return;
            playerbag.CostMoney(StoreEquiModel.Instance.GetEquip(select).Cost);
            BattleUserEquipC2S MsgC2S = new BattleUserEquipC2S();
            MsgC2S.RolesID = PlayerModel.Instance.rolesInfo.RolesID;
            MsgC2S.RoomID= PlayerModel.Instance.roomInfo.ID; 
            MsgC2S.ID = select;
            Bufferfactory.CreateAndSendpackage(1501, MsgC2S);
        }
        //显示选择装备的信息
        public void ShowEqipment(int Id)
        {
            select = Id;
            showEquipimage.sprite = StoreEquiModel.Instance.GetSprite(select);
            showEquipText.text= StoreEquiModel.Instance.GetEquip(select).Descroption;
            showeqmoney.text= StoreEquiModel.Instance.GetEquip(select).Cost.ToString();
        }
        //每秒检测能不能买选择的装备更新按钮 更新钱的显示
        private void isCanButEquip()
        {
            if (select == -1)
                return;
            playermoney.text = playerbag.GetplayerMoney().ToString();
            if (playerbag.GetplayerMoney() < StoreEquiModel.Instance.GetEquip(select).Cost || playerbag.GetEquipCount() >= 6)
            {
                buyText.transform.parent.GetComponent<Button>().enabled = false;
                buyText.color = new Color(0.4f, 0.4f, 0.4f, 1);
            }
            else
            {
                buyText.transform.parent.GetComponent<Button>().enabled = true;
                buyText.color = new Color(1, 1, 1, 1);
            }
        }
    }
}
