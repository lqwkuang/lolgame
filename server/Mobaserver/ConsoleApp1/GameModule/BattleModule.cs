using Mobaserver.Net;
using Mobaserver.Room;
using ProtoMsg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mobaserver.GameModule
{
    class BattleModule : GameModuleBase<BattleModule>
    {
        public override void AddListener()
        {
            base.AddListener();
            NetEvent.Instance.AddEventListener(1500, HandleBattleUserInputC2S);
            NetEvent.Instance.AddEventListener(1501, HandleBattleEquipC2S);
        }
        //处理购买装备的行为
        private void HandleBattleEquipC2S(BufferEntity entity)
        {
            BattleUserEquipC2S c2sMSG = ProtobufHelper.FromBytes<BattleUserEquipC2S>(entity.proto);
            RoomEntity roomEntity = RoomManager.Instance.Get(c2sMSG.RoomID);
            if (roomEntity != null)
            {
                roomEntity.HandleBattleEquipC2S(c2sMSG);
            }
        }

        //处理用户传输过来的输入
        private void HandleBattleUserInputC2S(BufferEntity entity)
        {
            BattleUserInputC2S c2sMSG= ProtobufHelper.FromBytes<BattleUserInputC2S>(entity.proto);
            RoomEntity roomEntity= RoomManager.Instance.Get(c2sMSG.RoomID);
            if (roomEntity!=null)
            {
                roomEntity.HandleBattleUserInputC2S(c2sMSG);
            }
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
            NetEvent.Instance.RemoveEventListener(1500, HandleBattleUserInputC2S);
            NetEvent.Instance.RemoveEventListener(1501, HandleBattleEquipC2S);
        }
    }
}
