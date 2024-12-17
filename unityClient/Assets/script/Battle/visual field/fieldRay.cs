using Game.Ctrl;
using Game.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//������Ұ�ļ�� �ж��Ƿ��ܹ���������
public class fieldRay : MonoSingleton<fieldRay>
{
    //��Ұ��λλ��
    public List<Transform> pos;
    //Ŀ��
    public List<Transform> target;
    //��ӪID
    public int teamID;

    public void Init()
    {
        pos = new List<Transform>();
        target = new List<Transform>();
        teamID = RoomCtrl.Instance.GetTeamID(PlayerModel.Instance.rolesInfo.RolesID);
        if(teamID==0)
        {
            //����A
            GameObject[] tempos = GameObject.FindGameObjectsWithTag("Atower");
            for (int i = 0; i < tempos.Length; i++)
            {
                pos.Add(tempos[i].transform);
            }
                
        }
        else if(teamID==1)
        {
            //����B
            GameObject[] tempos = GameObject.FindGameObjectsWithTag("Btower");
            for (int i = 0; i < tempos.Length; i++)
            {
                pos.Add(tempos[i].transform);
            }
        }
    }

    private void LateUpdate()
    {
       // RayCasttest();
    }
    //���Ӽ��Ŀ��
    public void Addtarget(Transform t)
    {
        target.Add(t);
    }
    //�����Ұ��
    public void Addpos(Transform transform )
    {
        pos.Add(transform);
    }
    //ɾ����Ұ��
    public void RemovePos(Transform transform)
    {
        pos.Remove(transform);
    }
    //����ܿ�����Щ����
    int[] countt = { 0,0,0,0,0} ;
    private void RayCasttest()
    {
        if (target == null || pos == null)
            return;
        int flag = 0;
        for(int i=0;i<target.Count;i++)
        {
            flag = 0;
            for(int j=0;j<pos.Count;j++)
            {
                if (Vector3.Distance(pos[j].position, target[i].position)<10)
                {
                    Debug.DrawLine(pos[j].position + new Vector3(0, 1, 0), target[i].position);
                    Ray ray = new Ray(pos[j].position+new Vector3(0, 1, 0), (target[i].position- pos[j].position).normalized);
                    if(Physics.Raycast(ray, out RaycastHit hitInfo,10,1<<LayerMask.NameToLayer("Wall")))
                    {
                        if (hitInfo.transform.name == target[i].name)
                        {
                            countt[i] = 0;
                            ChangeRneder(true, hitInfo.transform.gameObject);
                            flag = 1;
                            break;
                        }

                    }
                    else
                    {
                        
                        if(Physics.Raycast(ray, out RaycastHit hit))
                        {
                            Debug.Log($"hitcast{hit.transform.name}");
                        }
                    }
                }
            }
            if (flag==0)
            {
                if (countt[i] >= 20)
                {
                    countt[i] = 0;
                    ChangeRneder(false, target[i].gameObject);
                }
                else
                {
                    countt[i]++;
                }
            }
        }
    }
    int flag = 0;
    //�ı���ʾ״̬
    private void ChangeRneder(bool isRender,GameObject obj)
    {
        if (isRender)
        {
            Renderer[] ren = obj.GetComponentsInChildren<Renderer>();
            foreach(Renderer r in ren)
            {
                r.enabled = true;
            }
            obj.GetComponent<PlayerCtrl>().HUD.SetActive(true);
            obj.GetComponent<PlayerCtrl>().minmapIcon.SetActive(true);
        }
        else
        {
            Renderer[] ren = obj.GetComponentsInChildren<Renderer>();
            foreach (Renderer r in ren)
            {
                r.enabled = false;
            }
            obj.GetComponent<PlayerCtrl>().HUD.SetActive(false);
            obj.GetComponent<PlayerCtrl>().minmapIcon.SetActive(false);
        }
    }

}
