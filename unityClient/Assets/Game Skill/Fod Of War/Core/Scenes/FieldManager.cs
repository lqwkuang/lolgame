using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FogOfWar
{
    public abstract class FieldManager : MonoBehaviour
    {
        /// <summary> ��ȡ��Shaderʹ�õ������������() </summary>
        public abstract Vector4 GetWorldParams();

        /// <summary> ��ȡ��������0-1�������� </summary>
        public abstract Vector2 GetLocalPosInO1(Vector3 worldPos);

        /// <summary>
        /// �ж�ĳ���Ƿ��ܿ���ĳ��
        /// </summary>
        /// <param name="viewerWorldPos">�۲��ߵ���������</param>
        /// <param name="pixelUV">���۲�ĵ�</param>
        /// <returns></returns>
        public abstract bool IsVisible(Vector3 viewerWorldPos, Vector2 pixelUV);
    }
}
