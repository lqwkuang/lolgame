using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace FogOfWar.Demo 
{
    public enum GridType
    {
        Null,
        Ground,
        Obstacle,
        Glass,
    }

    public class FieldManagerTest : FieldManager
    {
        [Header("Real Field")]
        [Tooltip("��ͼʵ�ʴ�С")] public Vector3 m_MapSize;
        [Tooltip("��ͼ��Χ��ƫ����(�������0-1��ֵ)")] public Vector3 m_MinOffset;

        [Header("Obstacle Map")]
        [Tooltip("�ڵ�������Ĵ�С")] public Vector2Int m_ObstacleMapResolution = new Vector2Int(128, 128);   
        [Tooltip("�ڵ�������")] public Texture2D m_MapTex;

        [Header("Visible Test")]
        [Tooltip("�Ƿ�ʹ���ڵ�������")] public bool m_UseMapTex;
        [Tooltip("�Ƿ���ʾ��������")] public bool m_DrawGrid;
        [Tooltip("�Ƿ������Ӽ��")] public bool m_VisibleTest;
        [Tooltip("�۲���")] public Transform m_Viewer;
        [Tooltip("���۲�ĵ�")] public Transform m_PixelPoint;

        /// <summary> �������ӵ����� </summary>
        public GridType[,] grids;
        /// <summary> �������ӵĴ�С </summary>
        private Vector3 gridSize;
        /// <summary> ���е��ϰ��� </summary>
        private FieldObstacle[] obstacles;
        /// <summary> �������͸��ӵ����� </summary>
        private List<Vector2Int> groundCoords;
        private Glassdata glassdata1;

        private void OnEnable()
        {
            LoadMap();
        }
        public override Vector4 GetWorldParams()
        {
            return new Vector4(transform.position.x + m_MinOffset.x, transform.position.z + m_MinOffset.z, m_MapSize.x, m_MapSize.z);
        }

        public override Vector2 GetLocalPosInO1(Vector3 worldPos)
        {
            float x = (worldPos.x - (transform.position.x + m_MinOffset.x)) / m_MapSize.x;
            float z = (worldPos.z - (transform.position.z + m_MinOffset.z)) / m_MapSize.z;
            return new Vector2(x, z);
        }

        public override bool IsVisible(Vector3 viewerWorldPos, Vector2 pixelUV)
        {
            if (!m_VisibleTest)
                return true;

            // Ŀǰ���뷨�Ǵӵ�ǰ����۲��߷����⣬���Ƿ���gridΪ�ϰ�������
            Vector2Int viewerGridCoord = GetGridCoord(viewerWorldPos);
            Vector2Int pixelGridCoord = GetGridCoord(pixelUV);

            // ����һ����ֱ�߼��
            int minX = Mathf.Min(viewerGridCoord.x, pixelGridCoord.x);
            int maxX = Mathf.Max(viewerGridCoord.x, pixelGridCoord.x);
            int minZ = Mathf.Min(viewerGridCoord.y, pixelGridCoord.y);
            int maxZ = Mathf.Max(viewerGridCoord.y, pixelGridCoord.y);
            for (int x = minX; x < maxX; x++) //��ˮƽ�ٴ�ֱ
            {
                if (grids[x, minZ] == GridType.Obstacle|| grids[x, minZ] == GridType.Glass)
                    return false;
            }
            for (int z = minZ; z < maxZ; z++)
            {
                if (grids[maxX, z] == GridType.Obstacle|| grids[maxX, z] == GridType.Glass)
                    return false;
            }
            for (int z = minZ; z < maxZ; z++) //�ȴ�ֱ��ˮƽ
            {
                if (grids[minX, z] == GridType.Obstacle|| grids[minX, z] == GridType.Glass)
                    return false;
            }
            for (int x = minX; x < maxX; x++)
            {
                if (grids[x, maxZ] == GridType.Obstacle|| grids[x, maxZ] == GridType.Glass)
                    return false;
            }

            // ����������б�߼��

            // �����������߼��

            return true;
        }

        /// <summary> ����ת��ͼ�������� </summary>
        private void LoadMap()
        {
            grids = new GridType[m_MapTex.width, m_MapTex.height];
            gridSize = new Vector3(m_MapSize.x / m_MapTex.width, m_MapSize.y, m_MapSize.z / m_MapTex.height);
            groundCoords = new List<Vector2Int>();

            for (int x = 0; x < m_MapTex.width; x++)
            {
                for (int z = 0; z < m_MapTex.height; z++)
                {
                    if(m_MapTex.GetPixel(x, z).r > 1 - 1e-5&& m_MapTex.GetPixel(x, z).g > 1 - 1e-5)
                    {
                        grids[x, z] = GridType.Glass;
                       
                    }
                    else if(m_MapTex.GetPixel(x, z).r > 1 - 1e-5)
                    {
                        grids[x, z] = GridType.Obstacle;
                    }
                    else
                    {
                        grids[x, z] = GridType.Ground;
                    }
                    //grids[x, z] = m_MapTex.GetPixel(x, z).r > 1 - 1e-5 ? GridType.Obstacle : GridType.Ground;
                    if (grids[x, z] == GridType.Ground)
                        groundCoords.Add(new Vector2Int(x, z));
                }
            }
        }

        /// <summary> ��ȡ��ͼ�������һ��δ���ϰ����ڵĵ� </summary>
        public Vector3 GetRandomPos(float height)
        {
            int index = UnityEngine.Random.Range(0, groundCoords.Count);
            return GetWorldPosByCoord(groundCoords[index], height);
        }

        /// <summary> ��ȡ����ռ���ĳ���ڵ�ͼ�ϵ�Grid���� </summary>
        public Vector2Int GetGridCoord(Vector3 worldPos)
        {
            Vector2 uvCoord = GetLocalPosInO1(worldPos);
            return GetGridCoord(uvCoord);
        }

        /// <summary> ��ȡUV�ռ���ĳ���ȡ��ͼ�ϵ�Grid���� </summary>
        public Vector2Int GetGridCoord(Vector2 uv)
        {
            int x = Mathf.Clamp(Mathf.CeilToInt(m_ObstacleMapResolution.x * uv.x), 0, m_ObstacleMapResolution.x - 1);
            int z = Mathf.Clamp(Mathf.CeilToInt(m_ObstacleMapResolution.y * uv.y), 0, m_ObstacleMapResolution.y - 1);
            return new Vector2Int(x, z);
        }
        #region Tool
        //[Button("Generate Grids")]
        /// <summary> ���ɵ�ͼ�������飺����-1���ϰ���-2 �ݴ�-3</summary>
        private void GenerateMap()
        {
            glassdata1 = new Glassdata();
            grids = new GridType[m_ObstacleMapResolution.x, m_ObstacleMapResolution.y];
            gridSize = new Vector3(m_MapSize.x / m_ObstacleMapResolution.x, m_MapSize.y, m_MapSize.z / m_ObstacleMapResolution.y);
            groundCoords = new List<Vector2Int>();
            obstacles = FindObjectsOfType<FieldObstacle>();

            for (int x = 0; x < m_ObstacleMapResolution.x; x++)
            {
                for (int z = 0; z < m_ObstacleMapResolution.y; z++)
                {
                    float minX = transform.position.x + m_MinOffset.x + x * gridSize.x;
                    float minZ = transform.position.z + m_MinOffset.z + z * gridSize.z;
                    for (int i = 0; i < obstacles.Length; i++)
                    {
                        if (grids[x, z] == GridType.Obstacle || grids[x,z]==GridType.Glass)
                            continue;
                        if(IsGridInObstacle(obstacles[i], new Vector3(minX, 0, minZ)))
                        {
                            if (obstacles[i].indID==1)
                            {
                                grids[x, z] = GridType.Obstacle;
                            }
                            else if (obstacles[i].indID==2)
                            {
                                grids[x, z] = GridType.Glass;
                                glassdata1.x.Add(x);
                                glassdata1.z.Add(z);
                                glassdata1.ID.Add(obstacles[i].GlassID);
                            }
                        }
                        else
                        {
                            grids[x, z] = GridType.Ground;
                        }
                       // grids[x, z] = IsGridInObstacle(obstacles[i], new Vector3(minX, 0, minZ)) ? GridType.Obstacle : GridType.Ground;
                        if (grids[x, z] == GridType.Ground)
                            groundCoords.Add(new Vector2Int(x, z));
                    }

                    // ������������ȥ��ˮ֮���......
                }
            }
        }

        //[Button("Bake Into Texture")]
        /// <summary> �ѵ�ͼ��������決������ </summary>
        public void GenerateMapTexture()
        {
            GenerateMap();
            Texture2D mapTex = new Texture2D(m_ObstacleMapResolution.x, m_ObstacleMapResolution.y, TextureFormat.RHalf, false, false);
            for (int x = 0; x < m_ObstacleMapResolution.x; x++)
            {
                for (int z = 0; z < m_ObstacleMapResolution.y; z++)
                {
                    if(grids[x, z] == GridType.Obstacle)
                    {
                        mapTex.SetPixel(x, z, new Color(1, 0, 0, 0));
                    }
                    else if(grids[x, z] == GridType.Glass)
                    {
                        mapTex.SetPixel(x, z, new Color(1, 0,0,0));
                    }
                    else
                    {
                        mapTex.SetPixel(x, z, new Color(0, 0, 0, 0));
                    }
                    //mapTex.SetPixel(x, z, grids[x, z] == GridType.Obstacle ? new Color(1, 0, 0, 0) : new Color(0, 0, 0, 0));
                }
            }

            string directoryPath = Application.dataPath + "/Resources/Images/FieldMap/";
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);
            string json = JsonUtility.ToJson(glassdata1);
            string jsonpth=Path.Combine(Application.persistentDataPath,"Glassdata");
            File.WriteAllText(jsonpth,json);
            File.WriteAllBytes(directoryPath + gameObject.name + ".png", mapTex.EncodeToPNG());
           // AssetDatabase.Refresh();
        }

        /// <summary> �жϵ�ǰGrid�Ƿ�Ϊ�ϰ��� </summary>
        private bool IsGridInObstacle(FieldObstacle obstacle, Vector3 minWorldPos)
        {
            // ֱ���жϵ�ǰGrid���ĸ����Ƿ����ϰ�����
            int counter = 0;
            counter = obstacle.IsPointInObstacle(new Vector3(minWorldPos.x, 0, minWorldPos.z)) ? counter + 1 : counter;
            counter = obstacle.IsPointInObstacle(new Vector3(minWorldPos.x + gridSize.x, 0, minWorldPos.z)) ? counter + 1 : counter;
            counter = obstacle.IsPointInObstacle(new Vector3(minWorldPos.x, 0, minWorldPos.z + gridSize.z)) ? counter + 1 : counter;
            counter = obstacle.IsPointInObstacle(new Vector3(minWorldPos.x + gridSize.x, 0, minWorldPos.z + gridSize.z)) ? counter + 1 : counter;
            return counter >= 3;// ���������Ϊ���������������ϰ����ڲ������ϰ�����
        }

        private void OnValidate()
        {
            if (m_MapTex == null || !m_UseMapTex)
                GenerateMap();
            else
                LoadMap();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(transform.position + m_MinOffset, 1);
            Gizmos.DrawWireCube(transform.position, m_MapSize);

            if (m_DrawGrid)
            {
                for (int x = 0; x < m_ObstacleMapResolution.x; x++)
                {
                    for (int z = 0; z < m_ObstacleMapResolution.y; z++)
                    {
                        Vector3 pos = GetWorldPosByCoord(new Vector2Int(x, z), transform.position.y + m_MinOffset.y);
                        if (grids[x, z] == GridType.Obstacle)
                            Gizmos.color = Color.red;
                        else
                            Gizmos.color = Color.white;
                        Gizmos.DrawWireCube(pos, gridSize);
                    }
                }
            }

            if (m_VisibleTest)
            {
                /*Gizmos.color = Color.green;
                Gizmos.DrawSphere(m_Viewer.position, 1);
                Gizmos.color = IsVisible(m_Viewer.position, GetLocalPosInO1(m_PixelPoint.position)) ? Color.blue : Color.red;
                Gizmos.DrawSphere(m_PixelPoint.position, 1);*/
            }
        }

        /// <summary> ��ȡĳgrid�����ĵ��������� </summary>
        private Vector3 GetWorldPosByCoord(Vector2Int coord, float height = 0)
        {
            return new Vector3(
                        transform.position.x + m_MinOffset.x + (coord.x + 0.5f) * gridSize.x,
                        height,
                        transform.position.z + m_MinOffset.z + (coord.y + 0.5f) * gridSize.z);
        }
        #endregion
    }
}


