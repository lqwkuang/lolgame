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
    class RoomModule : GameModuleBase<RoomModule>
    {
        public override void AddListener()
        {
            base.AddListener();
            NetEvent.Instance.AddEventListener(1400, HandleRoomSelectHeroC2S);
            NetEvent.Instance.AddEventListener(1401, HandleRoomSelectHeroSkillC2S);
            NetEvent.Instance.AddEventListener(1404, HandleRoomSendMsgC2S);
            NetEvent.Instance.AddEventListener(1405, HandleRoomLockHeroC2S);
            NetEvent.Instance.AddEventListener(1406, HandleRoomLoadProgressC2S);
        }
        //发送了加载进度
        private void HandleRoomLoadProgressC2S(BufferEntity entity)
        {
            RoomLoadProgressC2S c2sMSG = ProtobufHelper.FromBytes<RoomLoadProgressC2S>(entity.proto);
            RoomLoadProgressS2C s2cMSG = new RoomLoadProgressS2C();
            s2cMSG.IsBattleStart = false;

            PlayerEntity playerEntity = PlayerManager.GetPlayerEntityFromSession(entity.session);
           
            bool rescult= playerEntity.roomEntity.UpdateLoadProgress(playerEntity.rolesInfo.RolesID, c2sMSG.LoadProgress);
            if(rescult)
            {
                //所有玩家都已经加载完成了
            }
            else
            {
                playerEntity.roomEntity.GetLoadProgress(ref s2cMSG);
                Bufferfactory.CreqateAndSendPackage(entity, s2cMSG);
            }

            
        }
        //锁定英雄
        private void HandleRoomLockHeroC2S(BufferEntity entity)
        {
            RoomLockHeroC2S c2sMSG = ProtobufHelper.FromBytes<RoomLockHeroC2S>(entity.proto);
            RoomLockHeroS2C s2cMSG = new RoomLockHeroS2C();
            s2cMSG.HeroID = c2sMSG.HeroID;
            PlayerEntity playerEntity = PlayerManager.GetPlayerEntityFromSession(entity.session);
            s2cMSG.RolesID = playerEntity.rolesInfo.RolesID;

            //缓存角色技能
            playerEntity.roomEntity.LockHero(s2cMSG.RolesID, s2cMSG.HeroID);
            playerEntity.roomEntity.Broadcast(entity.messageID, s2cMSG);

        }


        //发送聊天信息
        private void HandleRoomSendMsgC2S(BufferEntity entity)
        {
            RoomSendMsgC2S c2sMSG = ProtobufHelper.FromBytes<RoomSendMsgC2S>(entity.proto);
            RoomSendMsgS2C s2cMSG = new RoomSendMsgS2C();
           
            PlayerEntity playerEntity = PlayerManager.GetPlayerEntityFromSession(entity.session);
            s2cMSG.RolesID = playerEntity.rolesInfo.RolesID;
            s2cMSG.Text = c2sMSG.Text;

            //只想广播给同阵营的玩家
            //playerEntity.roomEntity.Broadcast(playerEntity.TeamID, entity.messageID, s2cMSG);

            playerEntity.roomEntity.Broadcast(entity.messageID, s2cMSG);
        }
        //选择英雄
        private void HandleRoomSelectHeroSkillC2S(BufferEntity entity)
        {
            RoomSelectHeroSkillC2S c2sMSG = ProtobufHelper.FromBytes<RoomSelectHeroSkillC2S>(entity.proto);
            RoomSelectHeroSkillS2C s2cMSG = new RoomSelectHeroSkillS2C();
            s2cMSG.SkillID = c2sMSG.SkillID;
            s2cMSG.GridID= c2sMSG.GridID;
            PlayerEntity playerEntity = PlayerManager.GetPlayerEntityFromSession(entity.session);
            s2cMSG.RolesID = playerEntity.rolesInfo.RolesID;

            //缓存角色技能
            playerEntity.roomEntity.UpdateSKill(s2cMSG.RolesID, s2cMSG.SkillID, s2cMSG.GridID);
            playerEntity.roomEntity.Broadcast(entity.messageID, s2cMSG);
        }

        //用户选择英雄
        private void HandleRoomSelectHeroC2S(BufferEntity entity)
        {
            RoomSelectHeroC2S c2sMSG = ProtobufHelper.FromBytes<RoomSelectHeroC2S>(entity.proto);
            RoomSelectHeroS2C s2cMSG = new RoomSelectHeroS2C();
            s2cMSG.HeroID = c2sMSG.HeroID;
            PlayerEntity playerEntity= PlayerManager.GetPlayerEntityFromSession(entity.session);
            s2cMSG.RolesID = playerEntity.rolesInfo.RolesID;

            playerEntity.roomEntity.Broadcast(entity.messageID, s2cMSG);
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
            NetEvent.Instance.RemoveEventListener(1400, HandleRoomSelectHeroC2S);
            NetEvent.Instance.RemoveEventListener(1401, HandleRoomSelectHeroSkillC2S);
            NetEvent.Instance.RemoveEventListener(1404, HandleRoomSendMsgC2S);
            NetEvent.Instance.RemoveEventListener(1405, HandleRoomLockHeroC2S);
            NetEvent.Instance.RemoveEventListener(1406, HandleRoomLoadProgressC2S);
        }
    }
}
