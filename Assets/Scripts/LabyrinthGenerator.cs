using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabyrinthGenerator : MonoBehaviour
{
    public enum LabyShape
    {
        Rectangle,
        HoneyComb,
        Torus,
        Sphere,
    }

    public LabyShape m_Shape = LabyShape.Rectangle;

    public int m_NumberOfColumns = 10;
    public int m_NumberOfRows = 10;

    public bool m_ProgressiveGeneration = false;

    public int m_AlgorithmIndex = 0;

    public int m_AlgorithmCorridorPseudoLength = 4;

    public float m_NumberOfConnectionsPerSeconds = 50f;

    public GameObject m_SquareRoomPrefab = null;
    public GameObject m_HexagonalRoomPrefab = null;

    public GameObject m_Player = null;

    Labyrinth mInternalRepresentation;
    MazeGenerator mMazeGenerator = null;

    float m_TimeCounter = 0f;

    //GameObject m_Room1 = null;
    //GameObject m_Room2 = null;

    List<GameObject> m_Rooms = new List<GameObject>();
    

    bool mNeedToCompute = false;

    // Start is called before the first frame update
    void Start()
    {
        GameObject lObjectToInstantiate = null;

        switch (m_Shape)
        {
            case LabyShape.Rectangle:
                lObjectToInstantiate = m_SquareRoomPrefab;
                break;
            case LabyShape.HoneyComb:
                lObjectToInstantiate = m_HexagonalRoomPrefab;
                if (m_AlgorithmIndex > 1)
                {
                    mNeedToCompute = false;
                    return;
                }
                break;
            case LabyShape.Sphere:
                mNeedToCompute = false;
                return;
                break;
            case LabyShape.Torus:
                mNeedToCompute = false;
                return;
                break;
        }

        m_Rooms.Add(GameObject.Instantiate(lObjectToInstantiate,
                   transform.TransformPoint(new Vector3(-10, -10, 0)),
                   Quaternion.identity, transform));
        m_Rooms.Add(GameObject.Instantiate(lObjectToInstantiate,
                   transform.TransformPoint(new Vector3(-10, -10, 0)),
                   Quaternion.identity, transform));
        m_Rooms.Add(GameObject.Instantiate(lObjectToInstantiate,
                   transform.TransformPoint(new Vector3(-10, -10, 0)),
                   Quaternion.identity, transform));
        m_Rooms.Add(GameObject.Instantiate(lObjectToInstantiate,
                   transform.TransformPoint(new Vector3(-10, -10, 0)),
                   Quaternion.identity, transform));
        m_Rooms.Add(GameObject.Instantiate(lObjectToInstantiate,
                   transform.TransformPoint(new Vector3(-10, -10, 0)),
                   Quaternion.identity, transform));
        m_Rooms.Add(GameObject.Instantiate(lObjectToInstantiate,
                   transform.TransformPoint(new Vector3(-10, -10, 0)),
                   Quaternion.identity, transform));
        m_Rooms.Add(GameObject.Instantiate(lObjectToInstantiate,
                   transform.TransformPoint(new Vector3(-10, -10, 0)),
                   Quaternion.identity, transform));

        InitiateGeneration();
    }

    void updateRoomVisibility(int pRoomIndex, GameObject pRoom)
    {
        switch (m_Shape)
        {
            case LabyShape.Rectangle:
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
            case LabyShape.HoneyComb:
                {
                    HexaRoomConnectionsBehaviour lRoom = pRoom.GetComponent<HexaRoomConnectionsBehaviour>();
                    lRoom.resetVisibility();
                    Node lNode = mInternalRepresentation.mNodes[pRoomIndex];

                    lRoom.transform.position = transform.TransformPoint(new Vector3(Mathf.Sqrt(3) * ((pRoomIndex % m_NumberOfColumns) + ((pRoomIndex / m_NumberOfColumns) % 2 == 0 ? 0 : 0.5f)), 0, 1.5f * (pRoomIndex / m_NumberOfColumns)));

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
            case LabyShape.Sphere:
                break;
            case LabyShape.Torus:
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //detects which room
        Vector3 lPlayerPosition = m_Player.transform.position;

        int lZPosition = (int)((lPlayerPosition.z+1f) / 1.5f);

        bool lEven = (lZPosition % 2) == 0;

        int lXPosition = (int)(((lPlayerPosition.x + (lEven ? (Mathf.Sqrt(3)/2) : 0)) / (Mathf.Sqrt(3))));

        

        //Debug.Log(lPlayerPosition);
        //Debug.Log(lXPosition + " " + lZPosition + " from " + lPlayerPosition);

        lZPosition = Mathf.Clamp(lZPosition, 0, m_NumberOfRows-1);
        lXPosition = Mathf.Clamp(lXPosition, 0, m_NumberOfColumns-1);


        int lRoomIndex = HoneycombShapeGenerator.getIndex(lXPosition, lZPosition, m_NumberOfColumns, m_NumberOfRows);

        float lMagnitude = (new Vector3(Mathf.Sqrt(3) * ((lRoomIndex % m_NumberOfColumns) + ((lRoomIndex / m_NumberOfColumns) % 2 == 0 ? 0 : 0.5f)), 0, 1.5f * (lRoomIndex / m_NumberOfColumns))-m_Player.transform.position).magnitude;

#if true
        for (int j = -1; j < 2; ++j)
        {
            for (int i = -1; i < 2; ++i)
            {
                int lLocalRoomIndex = lXPosition + i + (lZPosition+j) * m_NumberOfColumns;
                if (lLocalRoomIndex >= 0 && lLocalRoomIndex< m_NumberOfColumns* m_NumberOfRows)
                {
                    float lLocalMagnitude = (new Vector3(Mathf.Sqrt(3) * ((lLocalRoomIndex % m_NumberOfColumns) + ((lLocalRoomIndex / m_NumberOfColumns) % 2 == 0 ? 0 : 0.5f)), 0, 1.5f * (lLocalRoomIndex / m_NumberOfColumns)) - m_Player.transform.position).magnitude;

                    if (lLocalMagnitude < lMagnitude)
                    {
                        lMagnitude = lLocalMagnitude;
                        lRoomIndex = lLocalRoomIndex;
                    }
                }
            }
        }

#endif

        List<Node> lNodesTorender = new List<Node>();

        Node lNode = mInternalRepresentation.mNodes[lRoomIndex];

        lNodesTorender.Add(lNode);

        lNodesTorender.AddRange(lNode.mConnectedNeighbours);

        for ( int i = 0 ; i < m_Rooms.Count; ++i)
        {
            if ( i < lNodesTorender.Count)
            {
                m_Rooms[i].SetActive(true);

                updateRoomVisibility(lNodesTorender[i].mIndex, m_Rooms[i]);
            }
            else
            {
                m_Rooms[i].SetActive(false);
                m_Rooms[i].transform.position = new Vector3(-10, 0, -10);
            }
        }

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
        m_Shape = (LabyShape)pShapeType;
    }

    public void InitiateGeneration()
    {
        if (mNeedToCompute)
        {
            return;
        }

        mNeedToCompute = true;

        m_Player.transform.position = new Vector3(0, 1.5f, 0);
        m_Player.transform.rotation = Quaternion.identity;

        switch (m_Shape)
        {
            case LabyShape.Rectangle:
                mInternalRepresentation = SquareShapeGenerator.generate(m_NumberOfColumns, m_NumberOfRows);
                break;
            case LabyShape.HoneyComb:
                mInternalRepresentation = HoneycombShapeGenerator.generate(m_NumberOfColumns, m_NumberOfRows);
                break;
            case LabyShape.Sphere:
                break;
            case LabyShape.Torus:
                break;
        }

        mMazeGenerator = new MazeGenerator(mInternalRepresentation);

        if (!m_ProgressiveGeneration)
        {
            mMazeGenerator.generate();
            mNeedToCompute = false;
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

                //updateRoomVisibility(lNodeIndex1, m_Room1);
                //updateRoomVisibility(lNodeIndex2, m_Room2);
            }

            if (!mNeedToCompute)
            {
                mMazeGenerator = null;
            }
        }
    }
}
