using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class basebuffer : MonoBehaviour
{
    //����ʱ��
    public float DurationTime;
    //��ʱ
    public float timeval;
    //���ö���
    public GameObject target;

    public virtual void Init(GameObject target,float Durationtime)
    {
        this.target = target;
        this.DurationTime = Durationtime;
        timeval = 0;
        buff();
    }
    // Update is called once per frame
    void Update()
    {
        timeval += Time.deltaTime;
        if(timeval>=DurationTime)
        {
            Destorbuff();
        }
    }
    //�����buffЧ��
    public virtual void buff()
    {

    }
    //���buffer
    public virtual void Destorbuff()
    {

    }
}
