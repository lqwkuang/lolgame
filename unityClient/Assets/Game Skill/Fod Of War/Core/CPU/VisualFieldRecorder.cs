using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace FogOfWar.CPU 
{
    public class VisualFieldRecorder : MonoBehaviour
    {
        [Header("Visual Tex")]
        [Tooltip("纹理大小")]public Vector2Int m_TexSize = new Vector2Int(128, 128);
        [Tooltip("视野纹理 ")] [ReadOnly] public Texture2D m_VisualTex;
        [Tooltip("用来模糊的纹理大小")] public Vector2Int m_RTSize = new Vector2Int(512, 512);
        [Tooltip("用来模糊的纹理")] [ReadOnly] public RenderTexture m_VisualRT;
        [Tooltip("刷新率(每秒几次)")] public float m_RefreshRate = 10;

        [Header("Optimization")]
        [Tooltip("是否模糊")] public bool m_Blur = true;
        [Tooltip("模糊的材质")] public Material m_BlurMat;
        [Tooltip("模糊的偏移量")] public float m_BlurOffset = 1;
        [Tooltip("是否插值")] public bool m_Lerp = true;
        [Tooltip("插值纹理")] [ReadOnly] public Texture2D m_VisualTexForLerp;
        [Tooltip("插值速度")] public float m_LerpSpeed = 1;

        [Header("Test")]
        [Tooltip("区域")] public FieldManager m_Field;
        [Tooltip("应用材质")] public Material m_ApplyMaterial; //比如我们分别做了plane和后处理两种

        /// <summary> 刷新时间计时器 </summary>
        private float m_RefreshRateTimer = 0;
        /// <summary> 具有可视范围的物体 </summary>
        public HashSet<VisualFieldObject> m_Visualers = new HashSet<VisualFieldObject>();
        /// <summary> 清空纹理的数组 </summary>
        private Color[] m_ClearColor;

        private void OnEnable()
        {
            m_ClearColor = new Color[m_TexSize.x * m_TexSize.y];
            for (int i = 0; i < m_TexSize.x; i++)
            {
                for (int j = 0; j < m_TexSize.y; j++)
                {
                    m_ClearColor[i * m_TexSize.y + j] = Color.clear;
                }
            }

            m_VisualTex = new Texture2D(m_TexSize.x, m_TexSize.y, TextureFormat.RHalf, false, false);
            m_VisualTex.wrapMode = TextureWrapMode.Clamp;

            if (m_Blur)
            {
                m_VisualRT = RenderTexture.GetTemporary(m_RTSize.x, m_RTSize.y, 0, RenderTextureFormat.RHalf);
                m_ApplyMaterial.SetTexture("_MaskTex", m_VisualRT);
            }
            else
            {
                m_ApplyMaterial.SetTexture("_MaskTex", m_VisualTex);
            }

            if (m_Lerp)
            {
                m_VisualTexForLerp = new Texture2D(m_TexSize.x, m_TexSize.y, TextureFormat.RHalf, false, false);
                m_VisualTexForLerp.wrapMode = TextureWrapMode.Clamp;
            }
        }

        private void OnDisable()
        {
            Destroy(m_VisualTex);
            if (m_Blur)
                RenderTexture.ReleaseTemporary(m_VisualRT);
            if (m_Lerp)
                Destroy(m_VisualTexForLerp);
        }
        private void Start()
        {
            UpdateVisual();
            if (m_Lerp)
                Graphics.CopyTexture(m_VisualTex, m_VisualTexForLerp);
        }
        public void Init()
        {
            m_ClearColor = new Color[m_TexSize.x * m_TexSize.y];
            for (int i = 0; i < m_TexSize.x; i++)
            {
                for (int j = 0; j < m_TexSize.y; j++)
                {
                    m_ClearColor[i * m_TexSize.y + j] = Color.clear;
                }
            }

            m_VisualTex = new Texture2D(m_TexSize.x, m_TexSize.y, TextureFormat.RHalf, false, false);
            m_VisualTex.wrapMode = TextureWrapMode.Clamp;

            if (m_Blur)
            {
                m_VisualRT = RenderTexture.GetTemporary(m_RTSize.x, m_RTSize.y, 0, RenderTextureFormat.RHalf);
                m_ApplyMaterial.SetTexture("_MaskTex", m_VisualRT);
            }
            else
            {
                m_ApplyMaterial.SetTexture("_MaskTex", m_VisualTex);
            }

            if (m_Lerp)
            {
                m_VisualTexForLerp = new Texture2D(m_TexSize.x, m_TexSize.y, TextureFormat.RHalf, false, false);
                m_VisualTexForLerp.wrapMode = TextureWrapMode.Clamp;
            }
            UpdateVisual();
            if (m_Lerp)
                Graphics.CopyTexture(m_VisualTex, m_VisualTexForLerp);
        }
        private void Update()
        {
            // 解决抖动问题
            if (m_Lerp)
            {
                m_RefreshRateTimer += Time.deltaTime;
                if (m_RefreshRateTimer < 1 / m_RefreshRate)
                {
                    SmoothVisual();
                    return;
                }
                else
                {
                    m_RefreshRateTimer -= 1 / m_RefreshRate;
                }
                UpdateVisual();
                SmoothVisual();
            }
            else
            {
                UpdateVisual();
            }

            // 模糊
            if (m_Blur)
            {
                Graphics.Blit(m_Lerp ? m_VisualTexForLerp : m_VisualTex, m_VisualRT);
                RenderTexture temp = RenderTexture.GetTemporary(m_RTSize.x, m_RTSize.y, 0, RenderTextureFormat.RHalf);
                m_BlurMat.SetVector("_Offsets", new Vector2(0, m_BlurOffset / m_VisualRT.height));
                Graphics.Blit(m_VisualRT, temp, m_BlurMat);
                m_BlurMat.SetVector("_Offsets", new Vector2(m_BlurOffset / m_VisualRT.width, 0));
                Graphics.Blit(temp, m_VisualRT, m_BlurMat);
                RenderTexture.ReleaseTemporary(temp);
            }
        }

        /// <summary> 更新全部物体的可视范围 </summary>
        private void UpdateVisual() 
        {
            m_VisualTex.SetPixels(m_ClearColor);
            foreach (var visualer in m_Visualers)
                SetVisual(visualer);
            m_VisualTex.Apply();
        }

        /// <summary> 更新单个物体可视范围 </summary>
        private void SetVisual(VisualFieldObject visualer)
        {
            // 计算具有可视范围单位在场景上的相对位置
            Vector2 uv = m_Field.GetLocalPosInO1(visualer.transform.position);
            // 进而得到其在纹理上的像素坐标
            Vector2Int texcoord = new Vector2Int(Mathf.CeilToInt(uv.x * m_TexSize.x), Mathf.CeilToInt(uv.y * m_TexSize.y));

            // 更新可视的范围
            for (int r = -visualer.m_VisualFieldRadius; r <= visualer.m_VisualFieldRadius; r++)
            {
                for (int c = -visualer.m_VisualFieldRadius; c <= visualer.m_VisualFieldRadius; c++)
                {
                    Vector2Int currenTexcoord = new Vector2Int(texcoord.x + r, texcoord.y + c);
                    if (currenTexcoord.x >= 0 && currenTexcoord.x < m_TexSize.x && currenTexcoord.y >= 0 && currenTexcoord.y < m_TexSize.y)
                    {
                        Vector2 currentUV = new Vector2((float)currenTexcoord.x / m_TexSize.x, (float)currenTexcoord.y / m_TexSize.y);
                        if (visualer.IsInside(new Vector2Int(r, c)) && m_Field.IsVisible(visualer.transform.position, currentUV))
                            m_VisualTex.SetPixel(currenTexcoord.x, currenTexcoord.y, new Color(1, 0, 0, 0));
                    }
                }
            }
        }

        /// <summary> 平滑可视范围 </summary>
        private void SmoothVisual()
        {
            for (int r = 0; r < m_TexSize.x; r++)
            {
                for (int c = 0; c < m_TexSize.y; c++)
                {
                    Color bufferPixel = m_VisualTexForLerp.GetPixel(r, c);
                    Color targetPixel = m_VisualTex.GetPixel(r, c);
                    m_VisualTexForLerp.SetPixel(r, c, Color.Lerp(bufferPixel, targetPixel, m_LerpSpeed * Time.deltaTime));
                }
            }
            m_VisualTexForLerp.Apply();
        }
    }
}

