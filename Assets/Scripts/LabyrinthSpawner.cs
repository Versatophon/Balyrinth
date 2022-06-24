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

    public LabyrinthGenerator.LabyShape m_Shape = LabyrinthGenerator.LabyShape.Rectangle;

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
        switch (m_Shape)
        {
            case LabyrinthGenerator.LabyShape.Rectangle:
                {
                    RoomConnectionsBehaviour lRoom = pRoom.GetComponent<RoomConnectionsBehaviour>();
                    lRoom.resetVisibility();
                    Node lNode = mInternalRepresentation.mNodes[pRoomIndex];
                    lRoom.transform.position = transform.TransformPoint(new Vector3(2 * (pRoomIndex % m_NumberOfColumns), 0, 2 * (pRoomIndex / m_NumberOfColumns)));


                    foreach (Node lNeighbourNode in lNode.mConnectedNeighbours)
                    {
                        if (pRoomIndex == (lNeighbourNode.mIndex - 1))
                        {
                            lRoom.m_ConnectedFromEast = true;
                        }

                        if (pRoomIndex == (lNeighbourNode.mIndex + 1))
                        {
                            lRoom.m_ConnectedFromWest = true;
                        }

                        if (pRoomIndex == (lNeighbourNode.mIndex + m_NumberOfColumns))
                        {
                            lRoom.m_ConnectedFromSouth = true;
                        }

                        if (pRoomIndex == (lNeighbourNode.mIndex - m_NumberOfColumns))
                        {
                            lRoom.m_ConnectedFromNorth = true;
                        }
                    }
                    lRoom.Updatevisibility();
                }
                break;
            case LabyrinthGenerator.LabyShape.HoneyComb:
                {
                    HexaRoomConnectionsBehaviour lRoom = pRoom.GetComponent<HexaRoomConnectionsBehaviour>();
                    lRoom.resetVisibility();
                    Node lNode = mInternalRepresentation.mNodes[pRoomIndex];

                    lRoom.transform.position = transform.TransformPoint(new Vector3(Mathf.Sqrt(3) * ((pRoomIndex % m_NumberOfColumns) + ((pRoomIndex / m_NumberOfColumns)%2 == 0 ? 0:0.5f)), 0, 1.5f * (pRoomIndex / m_NumberOfColumns)));

                    

                    foreach (Node lNeighbourNode in lNode.mConnectedNeighbours)
                    {
                        int lFirstRoomIndex = pRoomIndex; 
                        int lSecondRoomIndex = lNeighbourNode.mIndex;

                        if (lSecondRoomIndex == HoneycombShapeGenerator.getEast(lFirstRoomIndex % m_NumberOfColumns, lFirstRoomIndex / m_NumberOfColumns, m_NumberOfColumns, m_NumberOfRows))
                        {
                            lRoom.m_ConnectedFromEast = true;
                        }

                        if (lSecondRoomIndex == HoneycombShapeGenerator.getWest(lFirstRoomIndex % m_NumberOfColumns, lFirstRoomIndex / m_NumberOfColumns, m_NumberOfColumns, m_NumberOfRows))
                        {
                            lRoom.m_ConnectedFromWest = true;
                        }

                        if (lSecondRoomIndex == HoneycombShapeGenerator.getNorthEast(lFirstRoomIndex % m_NumberOfColumns, lFirstRoomIndex / m_NumberOfColumns, m_NumberOfColumns, m_NumberOfRows))
                        {
                            lRoom.m_ConnectedFromNorthEast = true;
                        }

                        if (lSecondRoomIndex == HoneycombShapeGenerator.getNorthWest(lFirstRoomIndex % m_NumberOfColumns, lFirstRoomIndex / m_NumberOfColumns, m_NumberOfColumns, m_NumberOfRows))
                        {
                            lRoom.m_ConnectedFromNorthWest = true;
                        }

                        if (lSecondRoomIndex == HoneycombShapeGenerator.getSouthEast(lFirstRoomIndex % m_NumberOfColumns, lFirstRoomIndex / m_NumberOfColumns, m_NumberOfColumns, m_NumberOfRows))
                        {
                            lRoom.m_ConnectedFromSouthEast = true;
                        }

                        if (lSecondRoomIndex == HoneycombShapeGenerator.getSouthWest(lFirstRoomIndex % m_NumberOfColumns, lFirstRoomIndex / m_NumberOfColumns, m_NumberOfColumns, m_NumberOfRows))
                        {
                            lRoom.m_ConnectedFromSouthWest = true;
                        }
                    }

                    lRoom.Updatevisibility();

                }
                break;
            case LabyrinthGenerator.LabyShape.Sphere:
                break;
            case LabyrinthGenerator.LabyShape.Torus:
                break;
        }

        

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
        m_Shape = (LabyrinthGenerator.LabyShape)pShapeType;
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
            m_Room1.transform.position = new Vector3(-10, -10, 0);
            GameObject.Destroy(m_Room1);
        }
        
        if (m_Room2 != null)
        {
            m_Room2.transform.position = new Vector3(-10, -10, 0);
            GameObject.Destroy(m_Room2);
        }

        GameObject lObjectToInstantiate = null;

        switch (m_Shape)
        {
            case LabyrinthGenerator.LabyShape.Rectangle:
                lObjectToInstantiate = m_SquareRoomPrefab;
                break;
            case LabyrinthGenerator.LabyShape.HoneyComb:
                lObjectToInstantiate = m_HexagonalRoomPrefab;
                //if( m_AlgorithmIndex > 1)
                //{
                //    mNeedToCompute = false;
                //    return;
                //}
                break;
            case LabyrinthGenerator.LabyShape.Sphere:
                mNeedToCompute = false;
                return;
                break;
            case LabyrinthGenerator.LabyShape.Torus:
                mNeedToCompute = false;
                return;
                break;
        }

        m_Room1 = GameObject.Instantiate(lObjectToInstantiate,
                   transform.TransformPoint(new Vector3(-10, -10, 0)),
                   Quaternion.identity, transform);
        m_Room2 = GameObject.Instantiate(lObjectToInstantiate,
                    transform.TransformPoint(new Vector3(-10, -10, 0)),
                    Quaternion.identity, transform);

        RenderTexture lTmpTexture = RenderTexture.active;
        RenderTexture.active = m_RTCam.targetTexture;
        GL.Clear(true, true, Color.clear);
        RenderTexture.active = lTmpTexture;

        AdaptViewportToContent lViewportAdapter = m_RTCam.GetComponent<AdaptViewportToContent>();

        switch (m_Shape)
        {
            case LabyrinthGenerator.LabyShape.Rectangle:
                {
                    ShapeGeneratorInterface lShapeGeneratorInterface = new SquareShapeGenerator(m_NumberOfColumns, m_NumberOfRows);
                    mInternalRepresentation = lShapeGeneratorInterface.generate(m_NumberOfColumns, m_NumberOfRows);
                    lViewportAdapter.m_xMin = -1;
                    lViewportAdapter.m_xMax = lViewportAdapter.m_xMin + m_NumberOfColumns * 2;
                    lViewportAdapter.m_zMin = -5;
                    lViewportAdapter.m_zMax = 5;
                    lViewportAdapter.m_yMin = -1;
                    lViewportAdapter.m_yMax = lViewportAdapter.m_yMin + m_NumberOfRows * 2;
                }
                break;
            case LabyrinthGenerator.LabyShape.HoneyComb:
                {
                    ShapeGeneratorInterface lShapeGeneratorInterface = new HoneycombShapeGenerator(m_NumberOfColumns, m_NumberOfRows);
                    mInternalRepresentation = lShapeGeneratorInterface.generate(m_NumberOfColumns, m_NumberOfRows);
                    lViewportAdapter.m_xMin = -Mathf.Sqrt(3) / 2;
                    lViewportAdapter.m_xMax = lViewportAdapter.m_xMin + (m_NumberOfColumns + 0.5f) * Mathf.Sqrt(3);
                    lViewportAdapter.m_zMin = -5;
                    lViewportAdapter.m_zMax = 5;
                    lViewportAdapter.m_yMin = -1;
                    lViewportAdapter.m_yMax = lViewportAdapter.m_yMin + m_NumberOfRows * 1.5f + 0.5f;
                }
                break;
            case LabyrinthGenerator.LabyShape.Sphere:
                break;
            case LabyrinthGenerator.LabyShape.Torus:
                break;
        }

        mMazeGenerator = new MazeGenerator(mInternalRepresentation, m_Shape, m_NumberOfColumns, m_NumberOfRows);

        if (!m_ProgressiveGeneration)
        {
            mMazeGenerator.generate();
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
