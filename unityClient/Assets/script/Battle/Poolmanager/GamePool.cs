using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GamePool:Singleton<GamePool> 
{
    private Dictionary<string, Stack<GameObject>> objpool;
    private Dictionary<string, GameObject> objGame;
    private int Captain = 2;
    private int Maxtain = 1000;
    private GameObject ALLpoolGame;
    public void Init()
    {
        objpool = new Dictionary<string, Stack<GameObject>>();
        objGame = new Dictionary<string, GameObject>();
        ALLpoolGame = new GameObject();
    }
    public void Addobj(string name,GameObject obj)
    {
        if (objpool.ContainsKey(name))
            return;
        Stack<GameObject> sta = new Stack<GameObject>();
        objGame[name] = obj;
        for(int i=0;i<Captain;i++)
        {
            GameObject t= GameObject.Instantiate(obj);
            t.SetActive(false);
            t.transform.SetParent(ALLpoolGame.transform);
            sta.Push(t);
        }
        objpool[name] = sta;
    }
    public GameObject GetGameobject(string name)
    {
        Stack<GameObject> stack = objpool[name];
        if(stack.Count>0)
        {
            return stack.Pop();
        }
        else
        {
            int pre = Captain;
            GameObject t = objGame[name];
            if (Captain * 2 > Maxtain)
            {
                Captain = Maxtain;
            }
            else
            {
                Captain *= 2;
            }
            for(int i=pre;i<Captain;i++)
            {
                GameObject temp = GameObject.Instantiate(t);
                temp.SetActive(false);
                temp.transform.SetParent(ALLpoolGame.transform);
                stack.Push(temp);
            }
            if (stack.Count > 0)
                return stack.Pop();
            else
                return null;
        }
    }
    public void Recely(string name,GameObject obj)
    {
        obj.SetActive(false);
        Stack<GameObject> stac = objpool[name];
        stac.Push(obj);
    }
    public async void Recely(string name, GameObject obj,float t)
    {
        await Task.Delay((int)t * 1000);
        obj.SetActive(false);
        Stack<GameObject> stac = objpool[name];
        stac.Push(obj);
    }
}
/*
 英雄的特效子典键值为 英雄编号+Q/Qhit
 
 */