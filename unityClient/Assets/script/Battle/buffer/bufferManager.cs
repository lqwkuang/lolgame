using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bufferManager : Singleton<bufferManager>
{
    //����buffer���б�
    private Dictionary<bufferType, basebuffer> allbuffer;
    //��ʼ��
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
{//ѣ��
    dizzy,
}
