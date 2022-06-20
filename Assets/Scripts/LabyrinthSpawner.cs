using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabyrinthSpawner : MonoBehaviour
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

    //public float m_TimeBetweenUpdates = 0.01f;
    public float m_NumberOfConnectionsPerSeconds = 50f;

    public GameObject m_RoomPrefab = null;

    public Camera m_RTCam = null;

    //List<RoomConnectionsBehaviour> m_RoomsList = new List<RoomConnectionsBehaviour>();

    Labyrinth mInternalRepresentation;
    MazeGenerator mMazeGenerator = null;


    Vector3 mMazeScale = new Vector3();
    Vector3 mMazeOffset = new Vector3();

    //int m_CurrentColor = 1;

    float m_TimeCounter = 0f;

    //public List<> m_PotentialConnections;

    GameObject m_Room1;
    GameObject m_Room2;


    bool mNeedToCompute = true;

    // Start is called before the first frame update
    void Start()
    {
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


        //TODO: Find a way to calculate BB
        mMazeScale.x = 3.56f / (m_NumberOfColumns*2);//x2 because the prefab is 2 units width
        mMazeScale.z = 2f / (m_NumberOfRows*2);//x2 because the prefab is 2 units width

        mMazeScale.x = 3.56f / (m_NumberOfColumns*1.78f);//x2 because the prefab is 2 units width
        mMazeScale.z = 2f / (m_NumberOfRows*1.5f);//x2 because the prefab is 2 units width

        //mMazeScale.y = 1f;

        mMazeScale.x = mMazeScale.y = mMazeScale.z = Mathf.Min(mMazeScale.x, mMazeScale.z);

        //For Rectangular Shape
        mMazeOffset.x = - (0.5f * m_NumberOfColumns - 0.5f) * mMazeScale.x;
        mMazeOffset.z = - (0.5f * m_NumberOfRows - 0.5f) * mMazeScale.z;

        //Hexa Shape
        mMazeOffset.x = - (0.5f * (m_NumberOfColumns-1) ) * mMazeScale.x;
        mMazeOffset.z = - (0.5f * (m_NumberOfRows-1) ) * mMazeScale.z;
        mMazeOffset.y = 0;


        transform.localScale = mMazeScale;
        //transform.position = mMazeOffset;

        if ( !m_ProgressiveGeneration )
        {
            mMazeGenerator.generate();
        }

        m_Room1 = GameObject.Instantiate(m_RoomPrefab,
                    transform.TransformPoint(new Vector3(-10, -10, 0)),
                    Quaternion.identity, transform);//.GetComponent<RoomConnectionsBehaviour>();
        m_Room2 = GameObject.Instantiate(m_RoomPrefab,
                    transform.TransformPoint(new Vector3(-10, -10, 0)),
                    Quaternion.identity, transform);//.GetComponent<RoomConnectionsBehaviour>();
        //lRoom.name = "Room (" + i + ";" + j + ")";

#if false
        for (int j = 0; j < m_NumberOfRows; ++j)
        {
            for (int i = 0; i < m_NumberOfColumns; ++i)
            {
                RoomConnectionsBehaviour lRoom = GameObject.Instantiate(m_RoomPrefab,
                    transform.TransformPoint(  new Vector3(2 * i, 0, 2 * j) ), 
                    Quaternion.identity, transform).GetComponent<RoomConnectionsBehaviour>();
                lRoom.name = "Room (" + i + ";" + j + ")";
                m_RoomsList.Add(lRoom);


                foreach (Node lNeighbour in  mInternalRepresentation.mNodes[j* m_NumberOfColumns+i].mConnectedNeighbours)
                {
                    if (lNeighbour.mIndex == (mInternalRepresentation.mNodes[j * m_NumberOfColumns + i].mIndex - 1))
                    {
                        lRoom.m_ConnectedFromWest = true;
                    }

                    if (lNeighbour.mIndex == (mInternalRepresentation.mNodes[j * m_NumberOfColumns + i].mIndex + 1))
                    {
                        lRoom.m_ConnectedFromEast = true;
                    }

                    if (lNeighbour.mIndex == (mInternalRepresentation.mNodes[j * m_NumberOfColumns + i].mIndex - m_NumberOfColumns))
                    {
                        lRoom.m_ConnectedFromSouth = true;
                    }

                    if (lNeighbour.mIndex == (mInternalRepresentation.mNodes[j * m_NumberOfColumns + i].mIndex + m_NumberOfColumns))
                    {
                        lRoom.m_ConnectedFromNorth = true;
                    }
                }

                lRoom.Updatevisibility();
            }
        }
#endif
    }


#if false
    void breakWallBetweenNodes(int pNodeIndex1, int pNodeIndex2)
    {
        if (pNodeIndex1 == (pNodeIndex2 - 1))
        {
            m_RoomsList[pNodeIndex1].GetComponent<RoomConnectionsBehaviour>().m_ConnectedFromEast = true;
            m_RoomsList[pNodeIndex2].GetComponent<RoomConnectionsBehaviour>().m_ConnectedFromWest = true;
        }

        if (pNodeIndex1 == (pNodeIndex2 + 1))
        {
            m_RoomsList[pNodeIndex1].GetComponent<RoomConnectionsBehaviour>().m_ConnectedFromWest = true;
            m_RoomsList[pNodeIndex2].GetComponent<RoomConnectionsBehaviour>().m_ConnectedFromEast = true;
        }

        if (pNodeIndex1 == (pNodeIndex2 + m_NumberOfColumns))
        {
            m_RoomsList[pNodeIndex1].GetComponent<RoomConnectionsBehaviour>().m_ConnectedFromSouth = true;
            m_RoomsList[pNodeIndex2].GetComponent<RoomConnectionsBehaviour>().m_ConnectedFromNorth = true;
        }

        if (pNodeIndex1 == (pNodeIndex2 - m_NumberOfColumns))
        {
            m_RoomsList[pNodeIndex1].GetComponent<RoomConnectionsBehaviour>().m_ConnectedFromNorth = true;
            m_RoomsList[pNodeIndex2].GetComponent<RoomConnectionsBehaviour>().m_ConnectedFromSouth = true;
        }

        m_RoomsList[pNodeIndex1].Updatevisibility();
        m_RoomsList[pNodeIndex2].Updatevisibility();
    }
#endif

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

                    lRoom.transform.position = transform.TransformPoint(new Vector3(1.7f * ((pRoomIndex % m_NumberOfColumns) + ((pRoomIndex / m_NumberOfColumns)%2 == 0 ? 0:0.5f)), 0, 1.48f * (pRoomIndex / m_NumberOfColumns)));

                    

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
                else if( m_AlgorithmIndex == 1 )
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
                //breakWallBetweenNodes(lNodeIndex1, lNodeIndex2);
            }
        }
    }
}
