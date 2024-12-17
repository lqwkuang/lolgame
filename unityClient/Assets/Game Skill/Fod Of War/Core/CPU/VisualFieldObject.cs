using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FogOfWar.CPU
{
    public class VisualFieldObject : MonoBehaviour
    {
        /// <summary> 可视区域半径 </summary>
        public int m_VisualFieldRadius = 10;
        /// <summary> 是否在可视范围内 </summary>
        private bool[,] m_Inside;
        /// <summary> 可视区域记录器 </summary>
        private VisualFieldRecorder recorder;

        /// <summary> 是否在可视范围内 </summary>
        public bool IsInside(Vector2Int coord)
        {
            return m_Inside[coord.x + m_VisualFieldRadius, coord.y + m_VisualFieldRadius];
        }

        private void OnEnable()
        {
            recorder = (VisualFieldRecorder)FindObjectOfType(typeof(VisualFieldRecorder));
            recorder.m_Visualers.Add(this);

            UpdateInside();
        }

        private void OnDisable()
        {
            recorder.m_Visualers.Remove(this);
        }

        private void UpdateInside()
        {
            m_Inside = new bool[m_VisualFieldRadius * 2 + 1, m_VisualFieldRadius * 2 + 1];
            float max = m_VisualFieldRadius * m_VisualFieldRadius;
            for (int r = -m_VisualFieldRadius; r <= m_VisualFieldRadius; r++)
            {
                for (int c = -m_VisualFieldRadius; c <= m_VisualFieldRadius; c++)
                {
                    m_Inside[r + m_VisualFieldRadius, c + m_VisualFieldRadius] = (Mathf.Abs(r * r) + Mathf.Abs(c * c)) < max;
                }
            }
        }
    }
}