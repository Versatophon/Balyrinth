using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabyrinthSpawner : MonoBehaviour
{
    //public enum LabyShape
    //{
    //    Rectangle,
    //    HoneyComb,
    //    Torus,
    //    Sphere,
    //}

    public Balyrinth.Utilities.LabyShape m_Shape = Balyrinth.Utilities.LabyShape.Rectangle;

    public int m_NumberOfColumns = 10;
    public int m_NumberOfRows = 10;

    public bool m_ProgressiveGeneration = false;

    public int m_AlgorithmIndex = 0;
    
    public int m_AlgorithmCorridorPseudoLength = 4;

    public float m_NumberOfConnectionsPerSeconds = 50f;

    public GameObject m_SquareRoomPrefab = null;
    public GameObject m_HexagonalRoomPrefab = null;

    public Camera m_RTCam = null;

    Labyrinth mInternalRepresentation;
    MazeGenerator mMazeGenerator = null;

    float m_TimeCounter = 0f;

    GameObject m_Room1 = null;
    GameObject m_Room2 = null;


    bool mNeedToCompute = false;

    // Start is called before the first frame update
    void Start()
    {
        InitiateGeneration();
    }

    void updateRoomVisibility(int pRoomIndex, GameObject pRoom)
    {
        RoomConnectionsBehaviour lRoom = pRoom.GetComponent<RoomConnectionsBehaviour>();
        lRoom.resetVisibility();
        Node lNode = mMazeGenerator.getNode(pRoomIndex);
        lRoom.transform.position = mMazeGenerator.getPosition(pRoomIndex);

        for (int i = 0; i < mMazeGenerator.mShapeGenerator.GetNumberOfDirections(); ++i)
        {
            lRoom.m_ConnectionsActive[i] = mMazeGenerator.areConnected(pRoomIndex, mMazeGenerator.mShapeGenerator.getNextCellIndex(pRoomIndex, i));
        }

        lRoom.Updatevisibility();

    }

    // Update is called once per frame
    void Update()
    {
        UpdateGeneration();
    }

    public void SetNumberOfColumns(string pNumberOfColumns)
    {
        int.TryParse(pNumberOfColumns, out m_NumberOfColumns);
    }

    public void SetNumberOfRows(string pNumberOfRows)
    {
        int.TryParse(pNumberOfRows, out m_NumberOfRows);
    }

    public void setShapeType(int pShapeType)
    {
        m_Shape = (Balyrinth.Utilities.LabyShape)pShapeType;
    }

    public void setAlgorithm(int pAlgorithmIndex)
    {
        m_AlgorithmIndex = pAlgorithmIndex;
    }

    public void setProgressiveGeneration(bool pProgressiveGeneration)
    {
        m_ProgressiveGeneration = pProgressiveGeneration;
    }
    
    public void setCorridorPseudoLength(string pCorridorPseudoLength)
    {
        int.TryParse(pCorridorPseudoLength, out m_AlgorithmCorridorPseudoLength);
    }
    
    public void setNumberOfConnectionsPerSecond(string pNumberOfConnectionsPerSecond)
    {
        float.TryParse(pNumberOfConnectionsPerSecond, out m_NumberOfConnectionsPerSeconds);
    }

    public void InitiateGeneration()
    {
        if (mNeedToCompute)
        {
            return;
        }

        mNeedToCompute = true;

        if (m_Room1 != null)
        {
            m_Room1.transform.position = new Vector3(-10, -10, 0) * Balyrinth.Utilities.VIEW_SCALE;
            GameObject.Destroy(m_Room1);
        }
        
        if (m_Room2 != null)
        {
            m_Room2.transform.position = new Vector3(-10, -10, 0) * Balyrinth.Utilities.VIEW_SCALE;
            GameObject.Destroy(m_Room2);
        }

        GameObject lObjectToInstantiate = null;

        switch (m_Shape)
        {
            case Balyrinth.Utilities.LabyShape.Rectangle:
                lObjectToInstantiate = m_SquareRoomPrefab;
                break;
            case Balyrinth.Utilities.LabyShape.HoneyComb:
                lObjectToInstantiate = m_HexagonalRoomPrefab;
                //if( m_AlgorithmIndex > 1)
                //{
                //    mNeedToCompute = false;
                //    return;
                //}
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

        m_Room1 = GameObject.Instantiate(lObjectToInstantiate,
                   transform.TransformPoint(new Vector3(-10, -10, 0) * Balyrinth.Utilities.VIEW_SCALE),
                   Quaternion.identity, transform);
        m_Room2 = GameObject.Instantiate(lObjectToInstantiate,
                    transform.TransformPoint(new Vector3(-10, -10, 0) * Balyrinth.Utilities.VIEW_SCALE),
                    Quaternion.identity, transform);

        RenderTexture lTmpTexture = RenderTexture.active;
        RenderTexture.active = m_RTCam.targetTexture;
        GL.Clear(true, true, Color.clear);
        RenderTexture.active = lTmpTexture;

        AdaptViewportToContent lViewportAdapter = m_RTCam.GetComponent<AdaptViewportToContent>();

        switch (m_Shape)
        {
            case Balyrinth.Utilities.LabyShape.Rectangle:
                {
                    ShapeGeneratorInterface lShapeGeneratorInterface = new SquareShapeGenerator(m_NumberOfColumns, m_NumberOfRows);
                    mInternalRepresentation = lShapeGeneratorInterface.generate(m_NumberOfColumns, m_NumberOfRows);
                    lViewportAdapter.m_xMin = -1 * Balyrinth.Utilities.VIEW_SCALE;
                    lViewportAdapter.m_xMax = lViewportAdapter.m_xMin + m_NumberOfColumns * 2 * Balyrinth.Utilities.VIEW_SCALE;
                    lViewportAdapter.m_zMin = -5;
                    lViewportAdapter.m_zMax = 5;
                    lViewportAdapter.m_yMin = -1 * Balyrinth.Utilities.VIEW_SCALE;
                    lViewportAdapter.m_yMax = lViewportAdapter.m_yMin + m_NumberOfRows * 2 * Balyrinth.Utilities.VIEW_SCALE;
                }
                break;
            case Balyrinth.Utilities.LabyShape.HoneyComb:
                {
                    ShapeGeneratorInterface lShapeGeneratorInterface = new HoneycombShapeGenerator(m_NumberOfColumns, m_NumberOfRows);
                    mInternalRepresentation = lShapeGeneratorInterface.generate(m_NumberOfColumns, m_NumberOfRows);
                    lViewportAdapter.m_xMin = (-Mathf.Sqrt(3) / 2) * Balyrinth.Utilities.VIEW_SCALE;
                    lViewportAdapter.m_xMax = lViewportAdapter.m_xMin + (m_NumberOfColumns + 0.5f) * Mathf.Sqrt(3) * Balyrinth.Utilities.VIEW_SCALE;
                    lViewportAdapter.m_zMin = -5;
                    lViewportAdapter.m_zMax = 5;
                    lViewportAdapter.m_yMin = -1 * Balyrinth.Utilities.VIEW_SCALE;
                    lViewportAdapter.m_yMax = lViewportAdapter.m_yMin + (m_NumberOfRows * 1.5f + 0.5f) * Balyrinth.Utilities.VIEW_SCALE;
                }
                break;
            case Balyrinth.Utilities.LabyShape.Sphere:
                break;
            case Balyrinth.Utilities.LabyShape.Torus:
                break;
        }

        lViewportAdapter.UpdateViewport();

        mMazeGenerator = new MazeGenerator(mInternalRepresentation, m_Shape, m_NumberOfColumns, m_NumberOfRows);

        if (!m_ProgressiveGeneration)
        {
            mMazeGenerator.generate(m_AlgorithmIndex, m_NumberOfColumns, m_NumberOfRows, m_AlgorithmCorridorPseudoLength);
            Debug.Log("Generation Performed !");

            float lStartupTime = Time.realtimeSinceStartup;

            foreach ( Node lNode in mInternalRepresentation.mNodes )
            {
                updateRoomVisibility(lNode.mIndex, m_Room1);
                m_RTCam.Render();
            }

            mNeedToCompute = false;

            float lRenderTime = Time.realtimeSinceStartup;

            Debug.Log((lRenderTime - lStartupTime) + " s. for rendering generation !");
            Debug.Log((m_NumberOfColumns*m_NumberOfRows)/(lRenderTime - lStartupTime) + " Room/s. rendering rate !");
        }

        m_TimeCounter = 0;
    }

    private void UpdateGeneration()
    {
        m_TimeCounter += Time.deltaTime;

        float lTimeBetweenUpdates = (1f / m_NumberOfConnectionsPerSeconds);

        if (m_ProgressiveGeneration && mNeedToCompute)
        {
            while (m_TimeCounter > lTimeBetweenUpdates && mNeedToCompute)
            {
                m_TimeCounter -= lTimeBetweenUpdates;

                int lNodeIndex1 = -1;
                int lNodeIndex2 = -1;

                if (m_AlgorithmIndex == 0)
                {
                    mNeedToCompute = !mMazeGenerator.generateStep(ref lNodeIndex1, ref lNodeIndex2);
                }
                else if (m_AlgorithmIndex == 1)
                {
                    mNeedToCompute = !mMazeGenerator.generateStep2(ref lNodeIndex1, ref lNodeIndex2);
                }
                else if (m_AlgorithmIndex == 2)
                {
                    mNeedToCompute = !mMazeGenerator.generateStep3(ref lNodeIndex1, ref lNodeIndex2, m_NumberOfColumns, m_NumberOfRows);
                }
                else if (m_AlgorithmIndex == 3)
                {
                    mNeedToCompute = !mMazeGenerator.generateStep4(ref lNodeIndex1, ref lNodeIndex2, m_NumberOfColumns, m_NumberOfRows, m_AlgorithmCorridorPseudoLength);
                }

                updateRoomVisibility(lNodeIndex1, m_Room1);
                updateRoomVisibility(lNodeIndex2, m_Room2);

                m_RTCam.Render();
            }

            if ( !mNeedToCompute )
            {
                mMazeGenerator = null;
                mNeedToCompute = false;
            }
        }
    }
}
