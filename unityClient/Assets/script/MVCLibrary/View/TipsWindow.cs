using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.View
{
    public class TipsWindow : BaseWindow
    {
        public TipsWindow()
        {
            scenesType = ScenesType.Login;
            resident = true;
            resName = "UIPrefab/Tips/TipsWindow";
            selfType = WindowType.TipsWindow;
        }
        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
        }

        Transform Tips01;
        Text Tips01Text;
        protected override void Awake()
        {
            base.Awake();
            Tips01 = transform.Find("Tips01");
            Tips01Text = transform.Find("Tips01/Tips01Text").GetComponent<Text>();
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
            for(int i=0;i<buttonList.Length;i++)
            {
                switch(buttonList[i].name)
                {
                    case "EnterBtn":
                        buttonList[i].onClick.AddListener(EnterBtnOnClick);
                        break;
                    case "CloseBtn":
                        buttonList[i].onClick.AddListener(CloseBtnOnClick);
                        break;
                    default:
                        break;
                }
            }
        }

        Action CloseBtnAction, EnterBtnAction;
        private void CloseBtnOnClick()
        {
            if(CloseBtnAction!=null)
            {
                CloseBtnAction();
                CloseBtnAction = null;
            }
            else
            {
                Close();
            }
        }

        private void EnterBtnOnClick()
        {
            if(EnterBtnAction!=null)
            {
                EnterBtnAction();
                EnterBtnAction = null;
            }
            else
            {
                Close();
            }
        }
        public void ShowTips(string text,Action enterBtnAction=null,Action closeBtnAction = null)
        {
            Tips01Text.text = text;
            EnterBtnAction = enterBtnAction;
            CloseBtnAction = closeBtnAction;
        }
    }
}


