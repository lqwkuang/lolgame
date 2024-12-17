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
        [Tooltip("�����С")]public Vector2Int m_TexSize = new Vector2Int(128, 128);
        [Tooltip("��Ұ���� ")] [ReadOnly] public Texture2D m_VisualTex;
        [Tooltip("����ģ���������С")] public Vector2Int m_RTSize = new Vector2Int(512, 512);
        [Tooltip("����ģ��������")] [ReadOnly] public RenderTexture m_VisualRT;
        [Tooltip("ˢ����(ÿ�뼸��)")] public float m_RefreshRate = 10;

        [Header("Optimization")]
        [Tooltip("�Ƿ�ģ��")] public bool m_Blur = true;
        [Tooltip("ģ���Ĳ���")] public Material m_BlurMat;
        [Tooltip("ģ����ƫ����")] public float m_BlurOffset = 1;
        [Tooltip("�Ƿ��ֵ")] public bool m_Lerp = true;
        [Tooltip("��ֵ����")] [ReadOnly] public Texture2D m_VisualTexForLerp;
        [Tooltip("��ֵ�ٶ�")] public float m_LerpSpeed = 1;

        [Header("Test")]
        [Tooltip("����")] public FieldManager m_Field;
        [Tooltip("Ӧ�ò���")] public Material m_ApplyMaterial; //�������Ƿֱ�����plane�ͺ�������

        /// <summary> ˢ��ʱ���ʱ�� </summary>
        private float m_RefreshRateTimer = 0;
        /// <summary> ���п��ӷ�Χ������ </summary>
        public HashSet<VisualFieldObject> m_Visualers = new HashSet<VisualFieldObject>();
        /// <summary> ������������ </summary>
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
            // �����������
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

            // ģ��
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

        /// <summary> ����ȫ������Ŀ��ӷ�Χ </summary>
        private void UpdateVisual() 
        {
            m_VisualTex.SetPixels(m_ClearColor);
            foreach (var visualer in m_Visualers)
                SetVisual(visualer);
            m_VisualTex.Apply();
        }

        /// <summary> ���µ���������ӷ�Χ </summary>
        private void SetVisual(VisualFieldObject visualer)
        {
            // ������п��ӷ�Χ��λ�ڳ����ϵ����λ��
            Vector2 uv = m_Field.GetLocalPosInO1(visualer.transform.position);
            // �����õ����������ϵ���������
            Vector2Int texcoord = new Vector2Int(Mathf.CeilToInt(uv.x * m_TexSize.x), Mathf.CeilToInt(uv.y * m_TexSize.y));

            // ���¿��ӵķ�Χ
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

        /// <summary> ƽ�����ӷ�Χ </summary>
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

