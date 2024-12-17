using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FogOfWar
{
    public abstract class FieldObstacle : MonoBehaviour
    {
        /// <summary>
        /// 障碍物类型 0是地面，1是障碍物，2是草
        /// </summary>
        public int indID = 0;
        [Header("每个草的编号")]
        public int GlassID;
        /// <summary> 某点是否位于障碍物内 </summary>
        public abstract bool IsPointInObstacle(Vector3 worldPos);
    }
}
