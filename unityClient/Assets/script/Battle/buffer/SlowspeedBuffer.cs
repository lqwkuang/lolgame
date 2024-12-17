using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowspeedBuffer : basebuffer
{
    private float range;
    public override void Init(GameObject target, float Durationtime)
    {
        base.Init(target, Durationtime);
    }
    //设置减速百分比
    public void setRange(float range)
    {
        this.range = range;
    }
    public override void buff()
    {
        target.GetComponent<PlayerCtrl>().currentAttribute.MoveSpeed *= range;
    }

    public override void Destorbuff()
    {
        target.GetComponent<PlayerCtrl>().currentAttribute.MoveSpeed /= range;
        Destroy(this);
    }

    
}
