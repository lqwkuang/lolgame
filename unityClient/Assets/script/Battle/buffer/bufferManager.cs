using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bufferManager : Singleton<bufferManager>
{
    //所有buffer的列表
    private Dictionary<bufferType, basebuffer> allbuffer;
    //初始化
    public void Init()
    {
        allbuffer = new Dictionary<bufferType, basebuffer>();
        allbuffer[bufferType.dizzy] = new dizzybuffer();
    }
    public basebuffer Getbuffer(bufferType bufferType)
    {
        return allbuffer[bufferType.dizzy];
    }
}
public enum bufferType
{//眩晕
    dizzy,
}
