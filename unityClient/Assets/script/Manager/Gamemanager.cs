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
        //�򿪵�¼����
        WindowManager.Instance.OpenWindow(WindowType.LoginWindow);
    }

    // Update is called once per frame
    void Update()
    {
        if(usocket!=null)
        {
            usocket.Handle();
        }
        //����UI��������Update
    }
    void DispatchNetEvent(BufferEntity buffer)
    {
        //���б��ķַ�
        NetEvent.Instance.Dispatch(buffer.messageID, buffer);
    }
}
