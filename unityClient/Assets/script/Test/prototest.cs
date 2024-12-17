using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProtoMsg;
using Game.Net;

public class prototest : MonoBehaviour
{
    public Usocket usocket;
    // Start is called before the first frame update
    void Start()
    {
        usocket = new Usocket(DispatchNetEvent);
        TestSend();
    }

    // Update is called once per frame
    void Update()
    {
        if(usocket!=null)
        {
            usocket.Handle();
        }
        if(Input.GetKeyDown(KeyCode.A))
        {
            TestSend();
        }
        
    }
    private static void TestSend()
    {
        UserInfo userInfo = new UserInfo();
        userInfo.Account = "88888";
        userInfo.Password = "123456";
        UserRegisterC2S userRegisterC2S = new UserRegisterC2S();
        userRegisterC2S.UserInfo = userInfo;
        BufferEntity bufferEntity = Bufferfactory.CreateAndSendpackage(1001, userInfo);

        //反序列化
        UserRegisterC2S userRegisterC1 = ProtobufHelper.FromBytes<UserRegisterC2S>(bufferEntity.proto);
    }
    void DispatchNetEvent(BufferEntity buffer)
    {
        //进行报文分发
    }
}
