using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class basebuffer : MonoBehaviour
{
    //持续时间
    public float DurationTime;
    //计时
    public float timeval;
    //作用对象
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
    //具体的buff效果
    public virtual void buff()
    {

    }
    //解除buffer
    public virtual void Destorbuff()
    {

    }
}
