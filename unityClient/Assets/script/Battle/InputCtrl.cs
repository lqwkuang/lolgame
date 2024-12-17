using Game.Model;
using Game.Net;
using Game.View;
using ProtoMsg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputCtrl : MonoBehaviour
{
    //只需要监听用户的按键 然后发送网络消息
    PlayerCtrl playerCtrl;
    SkillManager playerSkillmanager;
    public void Init(PlayerCtrl playerCtrl)
    {
        this.playerCtrl = playerCtrl;
        this.playerSkillmanager = playerCtrl.gameObject.GetComponent<SkillManager>(); 
    }
    public void  Update()
    {
        if(Input.GetKeyDown(KeyCode.Q)&&!playerSkillmanager.IsCooling(KeyCode.Q))
        {
            SendInputCMD(KeyCode.Q);
        }
        else if (Input.GetKeyDown(KeyCode.W) && !playerSkillmanager.IsCooling(KeyCode.W))
        {
            SendInputCMD(KeyCode.W);
        }
        else if (Input.GetKeyDown(KeyCode.E) && !playerSkillmanager.IsCooling(KeyCode.E))
        {
            SendInputCMD(KeyCode.E);
        }
        else if(Input.GetKeyDown(KeyCode.A) && !playerSkillmanager.IsCooling(KeyCode.A))
        {
            SendInputCMD(KeyCode.A);
        }
        else if (Input.GetKeyDown(KeyCode.D) && !playerSkillmanager.IsCooling(KeyCode.D))
        {
            SendInputCMD(KeyCode.D);
        }
        else if (Input.GetKeyDown(KeyCode.F) && !playerSkillmanager.IsCooling(KeyCode.F))
        {
            SendInputCMD(KeyCode.F);
        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
            SendInputCMD(KeyCode.B);
        }
        else if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            MouseDownEvent(KeyCode.Mouse0);
        }
        else if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            SendInputCMD(KeyCode.Mouse1);
            MouseDownEvent(KeyCode.Mouse1);
        }
        //回放的时候 看到用户的所有操作
        //把用户的所有输入发送给服务器 鼠标移动的位置
    }
    Ray ray;
    void SendInputCMD(KeyCode key)
    {
        BattleUserInputC2S c2sMSG = new BattleUserInputC2S();
        c2sMSG.RolesID = PlayerModel.Instance.rolesInfo.RolesID;
        c2sMSG.RoomID = PlayerModel.Instance.roomInfo.ID;

        c2sMSG.Key = key.GetHashCode();

        RaycastHit hit;

        ray= Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray,out hit,1000,1 << LayerMask.NameToLayer("Ground")))
        {
            c2sMSG.MousePosition = hit.point.ToV3Info();
            if(lockTransform!=null)
            {
                c2sMSG.LockID = lockobjectID;
                c2sMSG.LockTag = lockTransform.tag;
            }
        }
        Bufferfactory.CreateAndSendpackage(1500, c2sMSG);
    }
    Transform lockTransform;
    int lockobjectID;
    public void MouseDownEvent(KeyCode key)
    {
        RaycastHit hit;

        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            if(hit.transform.gameObject.CompareTag("Ground"))
            {
                
                
            }
            else if(hit.transform.CompareTag("Player"))
            {
                this.lockTransform = hit.transform;
                this.lockobjectID = hit.transform.GetComponent<PlayerCtrl>().playerInfo.RolesInfo.RolesID;
                //点击到人物
                //更新战斗面板
                BattleWindow battleWindow=(BattleWindow)WindowManager.Instance.GetWindow(WindowType.BattleWindow);
                battleWindow.ShowSelectObjectInfo(hit.transform.gameObject);
            }
        }
    }
}
