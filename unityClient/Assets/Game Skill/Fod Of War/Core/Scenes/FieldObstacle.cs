using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FogOfWar
{
    public abstract class FieldObstacle : MonoBehaviour
    {
        /// <summary>
        /// �ϰ������� 0�ǵ��棬1���ϰ��2�ǲ�
        /// </summary>
        public int indID = 0;
        [Header("ÿ���ݵı��")]
        public int GlassID;
        /// <summary> ĳ���Ƿ�λ���ϰ����� </summary>
        public abstract bool IsPointInObstacle(Vector3 worldPos);
    }
}
