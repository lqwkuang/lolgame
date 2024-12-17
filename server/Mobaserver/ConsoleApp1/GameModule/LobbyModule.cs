using Mobaserver.Match;
using Mobaserver.Net;
using Mobaserver.Player;
using ProtoMsg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mobaserver.GameModule
{
    class LobbyModule : GameModuleBase<LobbyModule>
    {
        public override void AddListener()
        {
            base.AddListener();
            NetEvent.Instance.AddEventListener(1300, HandleLobbyToMatchC2S);
            NetEvent.Instance.AddEventListener(1302, HandleLobbyQuitMatchC2S);
        }
        //退出匹配
        private void HandleLobbyQuitMatchC2S(BufferEntity entity)
        {
            LobbyQuitMatchC2S c2sMSG = ProtobufHelper.FromBytes<LobbyQuitMatchC2S>(entity.proto);
            LobbyQuitMatchS2C s2cMSG = new LobbyQuitMatchS2C();
            PlayerEntity player = PlayerManager.GetPlayerEntityFromSession(entity.session);
            if(player !=null)
            {
                bool result = MatchManager.Instance.Remove(player.matchEntity);
                if(result)
                {
                    player.matchEntity = null;
                    s2cMSG.Result = 0;//退出成功

                }
                else
                {
                    s2cMSG.Result = 1;//不在匹配状态
                }
            }

            Bufferfactory.CreqateAndSendPackage(entity, s2cMSG);
        }
        //进入匹配
        private void HandleLobbyToMatchC2S(BufferEntity entity)
        {
            LobbyToMatchC2S c2sMSG = ProtobufHelper.FromBytes<LobbyToMatchC2S>(entity.proto);
            LobbyToMatchS2C s2cMSG = new LobbyToMatchS2C();
            s2cMSG.Result = 0;

            MatchEntity matchEntity = new MatchEntity();
            PlayerEntity player= PlayerManager.GetPlayerEntityFromSession(entity.session);
            //缓存匹配信息
            player.matchEntity = matchEntity;

            matchEntity.TeamID = player.rolesInfo.RolesID;
            matchEntity.player = player;

            Bufferfactory.CreqateAndSendPackage(entity, s2cMSG);

            //让角色进入匹配状态
            MatchManager.Instance.Add(matchEntity);
        }

        public override void Init()
        {
            base.Init();
        }

        public override void Release()
        {
            base.Release();
        }

        public override void RemoveListener()
        {
            base.RemoveListener();
            NetEvent.Instance.RemoveEventListener(1300, HandleLobbyToMatchC2S);
            NetEvent.Instance.RemoveEventListener(1302, HandleLobbyQuitMatchC2S);
        }
    }
}
