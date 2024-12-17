using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dizzybuffer : basebuffer
{
    private PlayerFSM fsm;
    public override void Init(GameObject target, float Durationtime)
    {
        base.Init(target, Durationtime);
    }
    public override void buff()
    {
        base.buff();
        fsm = target.GetComponent<PlayerCtrl>().playerFSM;
        fsm.ToNext(FSMState.Dance);
    }
    public override void Destorbuff()
    {
        fsm.ToNext(FSMState.Idle);
        Destroy(this);
        base.Destorbuff();
    }

    
}
