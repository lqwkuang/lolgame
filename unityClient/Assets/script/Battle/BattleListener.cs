using Game.Model;
using Game.Net;
using ProtoMsg;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleListener : Singleton<BattleListener>
{
    //��ʼ������ ����ս����������Ϣ
    public void Init()
    {
        awaitHandle = new Queue<BattleUserInputS2C>();
        NetEvent.Instance.AddEventListener(1500, HandleBattleUserInputS2C);
        NetEvent.Instance.AddEventListener(1501, HandleBattleUserEquipS2C);
    }
    private void HandleBattleUserEquipS2C(BufferEntity entity)
    {
        BattleUserEquipS2C s2cMsg= ProtobufHelper.FromBytes<BattleUserEquipS2C>(entity.proto);
        BattleManager.Instance.playerCtrlDIC[s2cMsg.Equip.RolesID].playerbag.HandleEquip(s2cMsg);
    }

    Queue<BattleUserInputS2C> awaitHandle;
    //����洢�����¼��ķ���
    private void HandleBattleUserInputS2C(BufferEntity entity)
    {
        BattleUserInputS2C s2cMSG = ProtobufHelper.FromBytes<BattleUserInputS2C>(entity.proto);
        awaitHandle.Enqueue(s2cMSG);
    }

    //�ͷŷ��� �Ƴ�����������Ϣ
    public void Relese()
    {
        NetEvent.Instance.RemoveEventListener(1500, HandleBattleUserInputS2C);
        NetEvent.Instance.RemoveEventListener(1501, HandleBattleUserEquipS2C);
        awaitHandle.Clear();
    }

    //���������¼��ķ���
    public void PlayerFrame(Action<BattleUserInputS2C> action)
    {
        if(action!=null&&awaitHandle.Count>0)
        {
            action(awaitHandle.Dequeue());
        }
    }
}
