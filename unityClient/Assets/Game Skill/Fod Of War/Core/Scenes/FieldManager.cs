using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FogOfWar
{
    public abstract class FieldManager : MonoBehaviour
    {
        /// <summary> 获取供Shader使用的世界坐标参数() </summary>
        public abstract Vector4 GetWorldParams();

        /// <summary> 获取相对区域的0-1本地坐标 </summary>
        public abstract Vector2 GetLocalPosInO1(Vector3 worldPos);

        /// <summary>
        /// 判断某点是否能看到某点
        /// </summary>
        /// <param name="viewerWorldPos">观察者的世界坐标</param>
        /// <param name="pixelUV">被观察的点</param>
        /// <returns></returns>
        public abstract bool IsVisible(Vector3 viewerWorldPos, Vector2 pixelUV);
    }
}
