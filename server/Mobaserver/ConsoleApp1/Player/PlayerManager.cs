using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mobaserver.Player
{
    public class PlayerManager
    {
        //数据结构
        //角色ID和角色实体
        public static ConcurrentDictionary<int, PlayerEntity> playerList=new ConcurrentDictionary<int, PlayerEntity>();
        public static ConcurrentDictionary<int, PlayerEntity> playerSession=new ConcurrentDictionary<int, PlayerEntity>();

        //缓存 用户信息
        public static void Add(int session,int userid,PlayerEntity player)
        {
            playerList.TryAdd(userid, player);
            playerSession.TryAdd(session, player);
        }

        public static bool RemoveFromSession(int session)
        {
            PlayerEntity player;
            return playerSession.TryRemove(session, out player);
        }

        public static bool RemoveFromRolesID(int rolesID)
        {
            PlayerEntity player;
            return playerSession.TryRemove(rolesID, out player);
        }

        public static PlayerEntity GetPlayerEntityFromSession(int session)
        {
            PlayerEntity player;
            if(playerSession.TryGetValue(session, out player))
            {
                return player;
            }
            else
            {
                return null;
            }
        }

        public static PlayerEntity GetPlayerEntityFromRoles(int rolesid)
        {
            PlayerEntity player;
            if (playerList.TryGetValue(rolesid, out player))
            {
                return player;
            }
            else
            {
                return null;
            }
        }
    }
}
