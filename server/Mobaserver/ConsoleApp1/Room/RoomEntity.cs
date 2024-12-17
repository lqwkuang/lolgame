using Google.Protobuf;
using Mobaserver.Net;
using Mobaserver.Player;
using ProtoMsg;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mobaserver.Room
{
    public class RoomEntity
    {
        //房间ID 选择英雄的时间配置 房间的信息 房间中的玩家列表 客户端列表 锁定次数
        //加载进度 是否所有用户加载完成
        public int roomID;
        public int selectHeroTime = 10000;
        public RoomInfo roomInfo;
        ConcurrentDictionary<int, PlayerInfo> playerList = new ConcurrentDictionary<int, PlayerInfo>();
        ConcurrentDictionary<int, UClient> clientList = new ConcurrentDictionary<int, UClient>();
        public int lockCount;
        //每个玩家的加载进度
        ConcurrentDictionary<int, int> playerProgress = new ConcurrentDictionary<int, int>();

        bool isLoadComplete = false;
        //接口：初始化 缓存加载进度 获取加载进度 锁定英雄 选择了召唤师技能 玩家信息的初始化
        //广播给房间的所有人 广播给自己队伍的玩家 销毁时
        public RoomEntity(RoomInfo roomInfo)
        {
            this.roomID = roomInfo.ID;
            this.roomInfo = roomInfo;
            Init();
        }
        //角色的初始化
        void PlayerInit()
        {
            for (int i=0;i<roomInfo.TeamA.Count;i++)
            {
                PlayerInfo playerInfo = new PlayerInfo();
                playerInfo.RolesInfo = roomInfo.TeamA[i];
                //默认值
                playerInfo.SkillA = 103;
                playerInfo.SkillB = 106;
                playerInfo.HeroID = 0;//表示未选择
                playerInfo.TeamID = 0;
                playerInfo.PosID = i;
                playerList.TryAdd(playerInfo.RolesInfo.RolesID, playerInfo);


                UClient client = GameManager.uSocket.GetClient(PlayerManager.GetPlayerEntityFromRoles(playerInfo.RolesInfo.RolesID).session);
                //缓存每一个客户端
                clientList.TryAdd(playerInfo.RolesInfo.RolesID, client);

                //加载进度
                playerProgress.TryAdd(playerInfo.RolesInfo.RolesID, 0);
            }
            for (int i = 0; i < roomInfo.TeamB.Count; i++)
            {
                PlayerInfo playerInfo = new PlayerInfo();
                playerInfo.RolesInfo = roomInfo.TeamB[i];
                //默认值
                playerInfo.SkillA = 103;
                playerInfo.SkillB = 106;
                playerInfo.HeroID = 0;//表示未选择
                playerInfo.TeamID = 1;
                playerInfo.PosID = i+5;
                playerList.TryAdd(playerInfo.RolesInfo.RolesID, playerInfo);

                UClient client = GameManager.uSocket.GetClient(PlayerManager.GetPlayerEntityFromRoles(playerInfo.RolesInfo.RolesID).session);
                //缓存每一个客户端
                clientList.TryAdd(playerInfo.RolesInfo.RolesID, client);

                //加载进度
                playerProgress.TryAdd(playerInfo.RolesInfo.RolesID, 0);
            }
        }
        //整个房间的初始化
        private async void Init()
        {
            PlayerInit();
            //选择英雄的时间
            await Task.Delay(selectHeroTime);
            //是不是所有玩家都锁定了英雄
            if(lockCount==(roomInfo.TeamA.Count+roomInfo.TeamB.Count))
            {
                //所有人都锁定了选择英雄
                //可以加载战斗
                RoomToBattleS2C s2cMSG = new RoomToBattleS2C();

                foreach(var rolesID in playerList.Keys)
                {
                    UClient client = GameManager.uSocket.GetClient(PlayerManager.GetPlayerEntityFromRoles(rolesID).session);
                    //缓存每一个客户端
                    clientList.TryAdd(rolesID, client);

                    s2cMSG.PlayerList.Add(playerList[rolesID]);
                }
                Broadcast(1407, s2cMSG);
            }
            else
            {
                //解散房间
                RoomCloseS2C roomCloseS2C = new RoomCloseS2C();
                Broadcast(1403, roomCloseS2C);
                //通知房间管理器 释放掉这个房间
                RoomManager.Instance.Remove(roomID);
            }
        }

        #region 广播的接口 

        
        public void Broadcast(int messageID, IMessage s2cMSG)
        {
            foreach(var client in clientList.Values)
            {
                Bufferfactory.CreqateAndSendPackage(client, messageID, s2cMSG);
            }
        }
        public void Broadcast(int teamID,int messageID, IMessage s2cMSG)
        {
           if(teamID==0)
            {
                //A队伍
                for(int i=0;i<roomInfo.TeamA.Count;i++)
                {
                    UClient client;
                    if(clientList.TryGetValue(roomInfo.TeamA[i].RolesID, out client))
                    {
                        Bufferfactory.CreqateAndSendPackage(client, messageID, s2cMSG);
                    }
                }
            }
           else
            {
                //B队伍
                for (int i = 0; i < roomInfo.TeamB.Count; i++)
                {
                    UClient client;
                    if (clientList.TryGetValue(roomInfo.TeamB[i].RolesID, out client))
                    {
                        Bufferfactory.CreqateAndSendPackage(client, messageID, s2cMSG);
                    }
                }

            }
        }
        #endregion

        //锁定英雄

        public void LockHero(int rolesID,int heroID)
        {
            lockCount++;
            playerList[rolesID].HeroID = heroID;
        }
        //更新召唤师技能
        public void UpdateSKill(int rolesID,int skillID,int gridID)
        {
            if(gridID==0)
            {
                playerList[rolesID].SkillA = skillID;
            }
            else
            {
                playerList[rolesID].SkillB = skillID; 
            }
        }

        //更新所有用户的进度
        public bool UpdateLoadProgress(int rolesID,int progress)
        {
            if(isLoadComplete==true)
            {
                return true; 
            }
            playerProgress[rolesID] = progress;
            if(isLoadComplete==false)
            {
                foreach(var value in playerProgress.Values)
                {
                    //100 实际加载进度 90客户端异步加载的场景 0-0.9
                    if(value<100)
                    {
                        isLoadComplete = false;
                        return false;
                    }

                }
                isLoadComplete = true;
                //告诉所有客户端 都加载完成了
                RoomLoadProgressS2C s2cMSG = new RoomLoadProgressS2C();
                s2cMSG.IsBattleStart = true;
                foreach(var item in playerProgress.Keys)
                {
                    s2cMSG.RolesID.Add(item);
                    s2cMSG.LoadProgress.Add(playerProgress[item]);
                }
                Broadcast(1406, s2cMSG);
            }
            return true;
        }

        //获取所有用户的加载进度
        public void GetLoadProgress(ref RoomLoadProgressS2C s2cMSG)
        {
            foreach (var item in playerProgress.Keys)
            {
                s2cMSG.RolesID.Add(item);
                s2cMSG.LoadProgress.Add(playerProgress[item]);
            }
        }
        //房间关闭销毁
        public void Close()
        {

        }
        //处理用户的输入
        internal void HandleBattleUserInputC2S(BattleUserInputC2S c2sMSG)
        {
            BattleUserInputS2C s2cMSG = new BattleUserInputS2C();
            s2cMSG.CMD = c2sMSG;
            Broadcast(1500, s2cMSG);
            //定一个间隔时间 66ms广播一次
        }
        //处理用户购买装备
        internal void HandleBattleEquipC2S(BattleUserEquipC2S c2sMSG)
        {
            Debug.Log("处理装备");
            BattleUserEquipS2C s2cMSG = new BattleUserEquipS2C();
            s2cMSG.Equip = c2sMSG;
            Debug.Log("广播买装备");
            Broadcast(1501, s2cMSG);
        }
    }
}
