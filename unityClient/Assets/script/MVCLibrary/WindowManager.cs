using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.View;
using System;

/// <summary>
/// 窗体类型
/// </summary>
public enum WindowType
{
    LoginWindow,
    RolesWindow,
    LobbyWindow,
    RoomWindow,
    BattleWindow,
    StoreWindow,
    TipsWindow,//提示窗口
}


/// <summary>
/// 场景类型,目的:提供根据场景类型进行预加载
/// </summary>
public enum ScenesType
{
    None,
    Login,
    Battle,
}
public class WindowManager : MonoSingleton<WindowManager>
{
    Dictionary<WindowType, BaseWindow> windowDIC = new Dictionary<WindowType, BaseWindow>();
    //构造函数 初始化
    public WindowManager() {
        
        windowDIC.Add(WindowType.LoginWindow, new LoginWindow());
        windowDIC.Add(WindowType.TipsWindow, new TipsWindow());
        windowDIC.Add(WindowType.RolesWindow, new RolesWindow());
        windowDIC.Add(WindowType.LobbyWindow, new LobbyWindow());
        windowDIC.Add(WindowType.RoomWindow, new RoomWindow());
        windowDIC.Add(WindowType.BattleWindow, new BattleWindow());
        windowDIC.Add(WindowType.StoreWindow, new StoreEqWindow());
    }

    public void Update()
    {
        foreach (var window in windowDIC.Values)
        {
            if (window.IsVisible())
            {
                window.Update(Time.deltaTime);
            }
        }
            
    }
    //打开窗口
    public BaseWindow OpenWindow(WindowType type) {
        BaseWindow window;
        if (windowDIC.TryGetValue(type, out window))
        {
            window.Open();
            return window;
        }
        else
        {
            Debug.LogError($"Open Error:{type}");
            return null;
        }
    }
    public BaseWindow GetWindow(WindowType type)
    {
        BaseWindow window;
        if (windowDIC.TryGetValue(type, out window))
        {
            return window;
        }
        else
        {
            return null;
        }
    }
    //关闭窗口
    public void CloseWindow(WindowType type) {
        BaseWindow window;
        if (windowDIC.TryGetValue(type, out window))
        {
            window.Close();
        }
        else
        {
            Debug.LogError($"Open Error:{type}");
        }
    }

    //预加载
    public void PreLoadWindow(ScenesType type)
    {
        foreach (var item in windowDIC.Values)
        {
            if (item.GetScenesType()==type)
            {
                item.PreLoad();
            }
        }
    }

    //隐藏掉某个类型的所有窗口
    public void HideAllWindow(ScenesType type,bool isDestroy=false) {

        foreach (var item in windowDIC.Values)
        {
            if (item.GetScenesType() == type)
            {
                item.Close(isDestroy);
            }
        }
    }
    //显示提示窗体
    public void ShowTips(string text, Action enterBtnAction=null, Action closeBtnAction=null)
    {
        TipsWindow tipsWindow = (TipsWindow)Instance.OpenWindow(WindowType.TipsWindow);
        tipsWindow.ShowTips(text, enterBtnAction, closeBtnAction);
    }
}
