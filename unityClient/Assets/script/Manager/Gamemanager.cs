using Game.Net;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gamemanager : MonoBehaviour
{
    public static Usocket usocket;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        usocket = new Usocket(DispatchNetEvent);
        //打开登录界面
        WindowManager.Instance.OpenWindow(WindowType.LoginWindow);
    }

    // Update is called once per frame
    void Update()
    {
        if(usocket!=null)
        {
            usocket.Handle();
        }
        //调用UI管理器的Update
    }
    void DispatchNetEvent(BufferEntity buffer)
    {
        //进行报文分发
        NetEvent.Instance.Dispatch(buffer.messageID, buffer);
    }
}
