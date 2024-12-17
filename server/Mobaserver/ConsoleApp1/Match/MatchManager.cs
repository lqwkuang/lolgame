using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mobaserver.Room;

namespace Mobaserver.Match
{
    class MatchManager:Singleton<MatchManager>
    {
        //2v2 1v1
        public int Number = 1;
        //存放所有的队伍
        ConcurrentDictionary<int, MatchEntity> pool = new ConcurrentDictionary<int, MatchEntity>();

        public bool Add(MatchEntity matchEntity)
        {
           if(pool.TryAdd(matchEntity.TeamID, matchEntity))
           {
                //判断是否满足开启一个房间
                if(pool.Count>=Number*2)
                {
                    //匹配完成的事件
                    MatchCompleteEvent();
                }
                return true;
           }
           else
           {
                Debug.Log($"加入匹配池失败{matchEntity.TeamID}");
                return false;
           }
        }
        //移出匹配池
        public bool Remove(MatchEntity matchEntity)
        {
            MatchEntity match;
            return pool.TryRemove(matchEntity.TeamID, out match);
        }
        //匹配完成
        private void MatchCompleteEvent()
        {
            Debug.Log($"匹配池完成");
            List<MatchEntity> teamA = new List<MatchEntity>();
            List<MatchEntity> teamB = new List<MatchEntity>();
            for(int i=0;i<Number*2;i++)
            {
                MatchEntity entity = pool.ElementAt(i).Value;
                if(teamA.Count<Number)
                {
                    teamA.Add(entity);
                }
                else
                {
                    teamB.Add(entity);
                }
            }
            //把这两个队伍传递给房间
            RoomManager.Instance.Add(teamA, teamB);
        }
    }
}
