using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LabyrinthSpawner : MonoBehaviour, MazeGenerationListener
{
    public MazeGeneratorManager m_MazeGeneratorManager;

    public GameObject m_SquareRoomPrefab = null;
    public GameObject m_HexagonalRoomPrefab = null;

    GameObject mRoomToInstantiate = null;

    public Camera m_RTCam = null;

    public GameObject m_StartingPointPrefab = null;
    public GameObject m_EndingPointPrefab = null;

    GameObject m_StartingCP = null;
    GameObject m_EndingCP = null;

    public LineRenderer m_PathRenderer = null;
    CatmullRomSpline m_ReferenceSpline = null;

    public int m_PathSplineSubdivisions = 10;
    public float m_PathTension = 0.5f;

    List<GameObject> m_Rooms = new List<GameObject>();

    bool mNeedToCompute = false;

    // Start is called before the first frame update
    void Start()
    {
        m_ReferenceSpline = new CatmullRomSpline();
        m_MazeGeneratorManager?.SetGenerationListener(this);
        InitiateGeneration();
    }

    void updateRoomVisibility(int pRoomIndex, GameObject pRoom)
    {
        RoomConnectionsBehaviour lRoom = pRoom.GetComponent<RoomConnectionsBehaviour>();
        lRoom.resetVisibility();

        lRoom.transform.position = m_MazeGeneratorManager.GetObjectPosition(pRoomIndex);
        lRoom.m_ConnectionsActive = m_MazeGeneratorManager.GetConnectedDirections(pRoomIndex).ToList();

        lRoom.Updatevisibility();
    }

    // Update is called once per frame
    void Update()
    {
        int[] lUpdatedRooms = m_MazeGeneratorManager.UpdateGeneration();

        if (lUpdatedRooms.Length > 0)
        {
            while (lUpdatedRooms.Length > m_Rooms.Count)
            {
                m_Rooms.Add(GameObject.Instantiate(mRoomToInstantiate,
                                                   transform.TransformPoint(new Vector3(-10, 0, -10) * Balyrinth.Utilities.VIEW_SCALE),
                                                   Quaternion.identity, transform));
            }

            for (int i = 0; i < m_Rooms.Count; ++i)
            {
                if (i < lUpdatedRooms.Length)
                {
                    m_Rooms[i].SetActive(true);
                    m_Rooms[i].transform.position = m_MazeGeneratorManager.GetObjectPosition(lUpdatedRooms[i]);

                    m_Rooms[i].GetComponent<RoomConnectionsBehaviour>().m_ConnectionsActive = m_MazeGeneratorManager.GetConnectedDirections(lUpdatedRooms[i]).ToList();
                    m_Rooms[i].GetComponent<RoomConnectionsBehaviour>().Updatevisibility();
                }
                else
                {
                    m_Rooms[i].SetActive(false);
                }
            }

            m_RTCam.Render();
        }
    }

    public void InitiateGeneration()
    {
        if (mNeedToCompute)
        {
            return;
        }

        mNeedToCompute = true;

        if (m_PathRenderer != null)
        {
            m_PathRenderer.gameObject.SetActive(false);
        }

        foreach(GameObject lRoom in m_Rooms)
        {
            lRoom.transform.position = new Vector3(-10, -10, 0) * Balyrinth.Utilities.VIEW_SCALE;
            GameObject.DestroyImmediate(lRoom);
        }

        m_Rooms.Clear();

        switch (m_MazeGeneratorManager.m_Shape)
        {
            case Balyrinth.Utilities.LabyShape.Rectangle:
                mRoomToInstantiate = m_SquareRoomPrefab;
                break;
            case Balyrinth.Utilities.LabyShape.HoneyComb:
                mRoomToInstantiate = m_HexagonalRoomPrefab;
                break;
            case Balyrinth.Utilities.LabyShape.Sphere:
                mNeedToCompute = false;
                return;
                break;
            case Balyrinth.Utilities.LabyShape.Torus:
                mNeedToCompute = false;
                return;
                break;
        }

        m_Rooms.Add(GameObject.Instantiate(mRoomToInstantiate, 
                                           transform.TransformPoint(new Vector3(-10, 0, -10) * Balyrinth.Utilities.VIEW_SCALE),
                                           Quaternion.identity, transform));



        if (m_StartingCP != null)
        {
            GameObject.DestroyImmediate(m_StartingCP);
            m_StartingCP = null;
        }

        if (m_EndingCP != null)
        {
            GameObject.DestroyImmediate(m_EndingCP);
            m_EndingCP = null;
        }

        RenderTexture lTmpTexture = RenderTexture.active;
        RenderTexture.active = m_RTCam.targetTexture;
        GL.Clear(true, true, Color.clear);
        RenderTexture.active = lTmpTexture;

        {//Viewport Update
            AdaptViewportToContent lViewportAdapter = m_RTCam.GetComponent<AdaptViewportToContent>();

            switch (m_MazeGeneratorManager.m_Shape)
            {
                case Balyrinth.Utilities.LabyShape.Rectangle:
                    {
                        lViewportAdapter.m_xMin = -1 * Balyrinth.Utilities.VIEW_SCALE;
                        lViewportAdapter.m_xMax = lViewportAdapter.m_xMin + m_MazeGeneratorManager.m_NumberOfColumns * 2 * Balyrinth.Utilities.VIEW_SCALE;
                        lViewportAdapter.m_zMin = -5;
                        lViewportAdapter.m_zMax = 5;
                        lViewportAdapter.m_yMin = -1 * Balyrinth.Utilities.VIEW_SCALE;
                        lViewportAdapter.m_yMax = lViewportAdapter.m_yMin + m_MazeGeneratorManager.m_NumberOfRows * 2 * Balyrinth.Utilities.VIEW_SCALE;
                    }
                    break;
                case Balyrinth.Utilities.LabyShape.HoneyComb:
                    {
                        lViewportAdapter.m_xMin = (-Mathf.Sqrt(3) / 2) * Balyrinth.Utilities.VIEW_SCALE;
                        lViewportAdapter.m_xMax = lViewportAdapter.m_xMin + (m_MazeGeneratorManager.m_NumberOfColumns + 0.5f) * Mathf.Sqrt(3) * Balyrinth.Utilities.VIEW_SCALE;
                        lViewportAdapter.m_zMin = -5;
                        lViewportAdapter.m_zMax = 5;
                        lViewportAdapter.m_yMin = -1 * Balyrinth.Utilities.VIEW_SCALE;
                        lViewportAdapter.m_yMax = lViewportAdapter.m_yMin + (m_MazeGeneratorManager.m_NumberOfRows * 1.5f + 0.5f) * Balyrinth.Utilities.VIEW_SCALE;
                    }
                    break;
                case Balyrinth.Utilities.LabyShape.Sphere:
                    break;
                case Balyrinth.Utilities.LabyShape.Torus:
                    break;
            }

            lViewportAdapter.UpdateViewport();
        }

        m_MazeGeneratorManager.InitiateGeneration();

        if (!m_MazeGeneratorManager.m_ProgressiveGeneration)
        {
            float lStartupTime = Time.realtimeSinceStartup;

            foreach ( int lNodeIndex in m_MazeGeneratorManager.GetActiveNodes())
            {
                updateRoomVisibility(lNodeIndex, m_Rooms[0]);
                m_RTCam.Render();
            }

            mNeedToCompute = false;

            float lRenderTime = Time.realtimeSinceStartup;

            Debug.Log((lRenderTime - lStartupTime) + " s. for rendering generation !");
            Debug.Log((m_MazeGeneratorManager.m_NumberOfColumns * m_MazeGeneratorManager.m_NumberOfRows)/(lRenderTime - lStartupTime) + " Room/s. rendering rate !");
        }
    }

    public void GenerationDone()
    {
        if (m_StartingCP == null)
        {
            m_StartingCP = GameObject.Instantiate(m_StartingPointPrefab);
        }
        
        if (m_EndingCP == null)
        {
            m_EndingCP = GameObject.Instantiate(m_EndingPointPrefab);
        }

        m_StartingCP.transform.position = m_MazeGeneratorManager.GetObjectPosition(m_MazeGeneratorManager.GetStartingNodeIndex());
        m_EndingCP.transform.position = m_MazeGeneratorManager.GetObjectPosition(m_MazeGeneratorManager.GetEndingNodeIndex());

        if (m_PathRenderer != null)
        {
            int[] lMazePathIndices = m_MazeGeneratorManager.GetCompletePath();

            m_ReferenceSpline.m_CurvePoints.Clear();
            m_ReferenceSpline.alpha = m_PathTension;
            for (int i = 0; i < lMazePathIndices.Length; ++i)
            {
                Vector3 lPos = m_MazeGeneratorManager.GetObjectPosition(lMazePathIndices[i]);// mMazeGenerator.getPosition(lLabyrinthPath[i].mIndex);
                m_ReferenceSpline.m_CurvePoints.Add(new Vector3(lPos.x, 0, lPos.z));
            }

            Vector3[] lSplinePoints = m_ReferenceSpline.Generate(m_PathSplineSubdivisions);
            m_PathRenderer.positionCount = lSplinePoints.Length;
            m_PathRenderer.SetPositions(lSplinePoints);

            m_PathRenderer.gameObject.SetActive(true);

            m_RTCam.Render();
        }

        mNeedToCompute = false;
    }


    public void SetNumberOfColumns(string pNumberOfColumns)
    {
        m_MazeGeneratorManager.SetNumberOfColumns(pNumberOfColumns);
    }

    public void SetNumberOfRows(string pNumberOfRows)
    {
        m_MazeGeneratorManager.SetNumberOfRows(pNumberOfRows);
    }

    public void SetShapeType(int pShapeType)
    {
        m_MazeGeneratorManager.SetShapeType(pShapeType);
    }

    public void SetAlgorithm(int pAlgorithmIndex)
    {
        m_MazeGeneratorManager.SetAlgorithm(pAlgorithmIndex);
    }

    public void SetProgressiveGeneration(bool pProgressiveGeneration)
    {
        m_MazeGeneratorManager.SetProgressiveGeneration(pProgressiveGeneration);
    }

    public void SetCorridorPseudoLength(string pCorridorPseudoLength)
    {
        m_MazeGeneratorManager.SetCorridorPseudoLength(pCorridorPseudoLength);
    }

    public void SetNumberOfConnectionsPerSecond(string pNumberOfConnectionsPerSecond)
    {
        m_MazeGeneratorManager.SetNumberOfConnectionsPerSecond(pNumberOfConnectionsPerSecond);
    }
}
