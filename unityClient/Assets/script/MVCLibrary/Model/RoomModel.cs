using Google.Protobuf.Collections;
using ProtoMsg;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Model
{
    //保存房间数据
    public class RoomModel : Singleton<RoomModel>
    {
        public RepeatedField<PlayerInfo> playerInfos = new RepeatedField<PlayerInfo>();
        public Dictionary<int, GameObject> playerObjects = new Dictionary<int, GameObject>();
        public Dictionary<int, HeroAttributeEntity> heroCurrentAtt = new Dictionary<int, HeroAttributeEntity>();
        public Dictionary<int, HeroAttributeEntity> heroTotalAtt = new Dictionary<int, HeroAttributeEntity>();

        internal void SaveHeroAttribute(int rolesID, HeroAttributeEntity currentAttribute, HeroAttributeEntity totalAttribute)
        {
            heroCurrentAtt[rolesID] = currentAttribute;
            heroTotalAtt[rolesID] = totalAttribute;
        }

        public void Clear()
        {
            playerObjects.Clear();
            heroCurrentAtt.Clear();
            heroTotalAtt.Clear();
        }
    }
}

