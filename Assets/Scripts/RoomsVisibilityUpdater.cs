using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomsVisibilityUpdater : MonoBehaviour
{
    public GameObject m_PlayerViewer = null;

    public GameObject m_SquareRoomPrefab = null;
    public GameObject m_HexagonalRoomPrefab = null;
    public GameObject m_HypermazeRoomPrefab = null;

    private GameObject mObjectToInstantiate;

    public GameObject ObjectToInstantiate
    {
        get
        { 
            return mObjectToInstantiate;
        }
        
        set 
        { 
            mObjectToInstantiate = value;
            foreach (GameObject lGO in m_Rooms)
            {
                GameObject.Destroy(lGO);
            }
            m_Rooms.Clear();
        }
    }

    public MazeGenerator m_MazeGenerator = null;

    List<GameObject> m_Rooms = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        /*
        switch (m_MazeGenerator.mShapeGenerator.getLabyShape())
        {
            case Balyrinth.Utilities.LabyShape.Rectangle:
                mObjectToInstantiate = m_SquareRoomPrefab;
                break;
            case Balyrinth.Utilities.LabyShape.HoneyComb:
                mObjectToInstantiate = m_HexagonalRoomPrefab;
               
                break;
            case Balyrinth.Utilities.LabyShape.Sphere:

                break;
            case Balyrinth.Utilities.LabyShape.Torus:

                break;
        }
        */
    }

    List<int> mVisibleRoomsIndices = new List<int>();

    // Update is called once per frame
    void Update()
    {
        if (m_MazeGenerator != null)
        {
            int lNumDirections = m_MazeGenerator.mShapeGenerator.GetNumberOfDirections();

            //List<int> lVisibleRoomsIndices = new List<int>();
            mVisibleRoomsIndices.Clear();

            //Debug.Log("Update");

            int lCurrentRoomIndex = ResolveCurrentRoom();
            if (lCurrentRoomIndex != -1)
            {
                mVisibleRoomsIndices.Add(lCurrentRoomIndex);
                Vector3 lLeftViewVector = m_PlayerViewer.transform.TransformDirection(-Vector3.right);
                Vector3 lRightViewVector = m_PlayerViewer.transform.TransformDirection(Vector3.right);

                RecursiveSeeThrough(lCurrentRoomIndex, -1, lNumDirections, Mathf.Rad2Deg * Mathf.Atan2(-lLeftViewVector.z, lLeftViewVector.x), Mathf.Rad2Deg * Mathf.Atan2(-lRightViewVector.z, lRightViewVector.x));
            }

            while ( m_Rooms.Count < mVisibleRoomsIndices.Count )
            {
                m_Rooms.Add(GameObject.Instantiate(mObjectToInstantiate,
                  transform.TransformPoint(new Vector3(-10, -10, 0) * Balyrinth.Utilities.VIEW_SCALE),
                  Quaternion.identity, transform));
            }

            for (int i = 0; i < m_Rooms.Count; ++i)
            {
                if (i < mVisibleRoomsIndices.Count)
                {
                    m_Rooms[i].SetActive(true);

                    //TODO: Check how it performs
                    updateRoomVisibility(mVisibleRoomsIndices[i], m_Rooms[i]);
                }
                else
                {
                    m_Rooms[i].SetActive(false);
                    m_Rooms[i].transform.position = new Vector3(-10, 0, -10) * Balyrinth.Utilities.VIEW_SCALE;
                }
            }
        }
    }

    private void RecursiveSeeThrough(int pRoomIndex, int pFromDirection, int pNumDirections, float pMinAngle, float pMaxAngle)
    {
        //Debug.Log("Comming From " + pRoomIndex + " to " + pFromDirection);
        for (int i = 0; i < pNumDirections; ++i)
        {
            if(pFromDirection == -1 || i != m_MazeGenerator.mShapeGenerator.getOppositeDirection(pFromDirection))
            {
                int lNextCellIndex = m_MazeGenerator.mShapeGenerator.getNextCellIndex(pRoomIndex, i);

                if (m_MazeGenerator.areConnected(pRoomIndex, lNextCellIndex))
                {
                    float lMinimalComputedAngle;
                    float lMaximalComputedAngle;
                    Vector3 lLeftExtremity;
                    Vector3 lRightExtremity;

                    m_MazeGenerator.mShapeGenerator.getDoorExtremities(pRoomIndex, i, out lLeftExtremity, out lRightExtremity);

                    float lMinimalLocalAngle = Mathf.Rad2Deg * Mathf.Atan2(-(lLeftExtremity.z - m_PlayerViewer.transform.position.z), lLeftExtremity.x - m_PlayerViewer.transform.position.x);
                    float lMaximalLocalAngle = Mathf.Rad2Deg * Mathf.Atan2(-(lRightExtremity.z - m_PlayerViewer.transform.position.z), lRightExtremity.x - m_PlayerViewer.transform.position.x);

                    //Debug.Log($"Angles ({lMinimalLocalAngle}, {lMaximalLocalAngle})");

#if false
                    //checks if door is viewed from the back
                    if ( !Balyrinth.Utilities.isMinimalAngleIsDirect(lMinimalLocalAngle, lMaximalLocalAngle) )
                    {
                        Debug.Log("Unwanted Check is reached !");
                        //TODO: check if this code is reached
                        float lTmpValue = lMinimalLocalAngle;
                        lMinimalLocalAngle = lMaximalLocalAngle;
                        lMaximalLocalAngle = lTmpValue;
                    }
#endif

                    if (Balyrinth.Utilities.isMinimalAngleIsDirect(lMinimalLocalAngle, lMaximalLocalAngle))
                    {
                        if (Balyrinth.Utilities.isInSight(pMinAngle, pMaxAngle, lMinimalLocalAngle, lMaximalLocalAngle, out lMinimalComputedAngle, out lMaximalComputedAngle))
                        {
                            mVisibleRoomsIndices.Add(lNextCellIndex);
                            RecursiveSeeThrough(lNextCellIndex, i, pNumDirections, lMinimalComputedAngle, lMaximalComputedAngle);
                        }
                    }
                }
                
            }
        }
    }

    int ResolveCurrentRoom()
    {
        int lZPosition = 0;
        int lXPosition = 0;

        int lRoomIndex = -1;

        //ShapeGeneratorInterface lSGInterface = null;

        Vector3 lPlayerPosition = m_PlayerViewer.transform.position;

        switch (m_MazeGenerator.mShapeGenerator.getLabyShape())
        {
            case Balyrinth.Utilities.LabyShape.HoneyComb:
                {
                    //lSGInterface = new HoneycombShapeGenerator(m_NumberOfColumns, m_NumberOfRows);

                    lZPosition = (int)((lPlayerPosition.z + (1f * Balyrinth.Utilities.VIEW_SCALE)) / (1.5f * Balyrinth.Utilities.VIEW_SCALE));
                    
                    bool lEven = (lZPosition % 2) == 0;
                    lXPosition = (int)(((lPlayerPosition.x + (lEven ? (Mathf.Sqrt(3) / 2) * Balyrinth.Utilities.VIEW_SCALE : 0)) / (Mathf.Sqrt(3) * Balyrinth.Utilities.VIEW_SCALE)));

                    //Debug.Log(lPlayerPosition);
                    //Debug.Log(lXPosition + " " + lZPosition + " from " + lPlayerPosition);

                    int lNumberOfColumns = m_MazeGenerator.mShapeGenerator.getWidth();
                    int lNumberOfRows = m_MazeGenerator.mShapeGenerator.getHeight();

                    lZPosition = Mathf.Clamp(lZPosition, 0, m_MazeGenerator.mShapeGenerator.getHeight() - 1);
                    lXPosition = Mathf.Clamp(lXPosition, 0, m_MazeGenerator.mShapeGenerator.getWidth() - 1);

                    lRoomIndex = HoneycombShapeGenerator.getIndex(lXPosition, lZPosition, lNumberOfColumns, lNumberOfRows);

                    float lMagnitude = (new Vector3(Mathf.Sqrt(3) * ((lRoomIndex % lNumberOfColumns) + ((lRoomIndex / lNumberOfColumns) % 2 == 0 ? 0 : 0.5f)) * Balyrinth.Utilities.VIEW_SCALE,
                                                    0, 
                                                    1.5f * (lRoomIndex / lNumberOfColumns) * Balyrinth.Utilities.VIEW_SCALE) - lPlayerPosition).magnitude;

#if true
                    for (int j = -1; j < 2; ++j)
                    {
                        for (int i = -1; i < 2; ++i)
                        {
                            int lLocalRoomIndex = lXPosition + i + (lZPosition + j) * lNumberOfColumns;
                            if (lLocalRoomIndex >= 0 && lLocalRoomIndex < lNumberOfColumns * lNumberOfRows)
                            {
                                float lLocalMagnitude = (new Vector3(Mathf.Sqrt(3) * ((lLocalRoomIndex % lNumberOfColumns) + ((lLocalRoomIndex / lNumberOfColumns) % 2 == 0 ? 0 : 0.5f)) * Balyrinth.Utilities.VIEW_SCALE,
                                                                     0,
                                                                     1.5f * (lLocalRoomIndex / lNumberOfColumns) * Balyrinth.Utilities.VIEW_SCALE) - lPlayerPosition).magnitude;

                                if (lLocalMagnitude < lMagnitude)
                                {
                                    lMagnitude = lLocalMagnitude;
                                    lRoomIndex = lLocalRoomIndex;
                                }
                            }
                        }
                    }

#endif
                }
                break;
            case Balyrinth.Utilities.LabyShape.Rectangle:
                {
                    //lSGInterface = new SquareShapeGenerator(m_NumberOfColumns, m_NumberOfRows);

                    int lNumberOfColumns = m_MazeGenerator.mShapeGenerator.getWidth();
                    int lNumberOfRows = m_MazeGenerator.mShapeGenerator.getHeight();

                    //TODO: Check this part
                    lZPosition = (int)((lPlayerPosition.z + 1f * Balyrinth.Utilities.VIEW_SCALE) / (2f * Balyrinth.Utilities.VIEW_SCALE));
                    lXPosition = (int)((lPlayerPosition.x + 1f * Balyrinth.Utilities.VIEW_SCALE) / (2f * Balyrinth.Utilities.VIEW_SCALE));

                    lZPosition = Mathf.Clamp(lZPosition, 0, lNumberOfRows - 1);
                    lXPosition = Mathf.Clamp(lXPosition, 0, lNumberOfColumns - 1);

                    lRoomIndex = SquareShapeGenerator.getIndex(lXPosition, lZPosition, lNumberOfColumns, lNumberOfRows);
                }
                break;
            case Balyrinth.Utilities.LabyShape.Octogonized:
                {//TODO: Non Euclidian resolution need to be computed
                    //lSGInterface = new SquareShapeGenerator(m_NumberOfColumns, m_NumberOfRows);

                    int lNumberOfColumns = m_MazeGenerator.mShapeGenerator.getWidth();
                    int lNumberOfRows = m_MazeGenerator.mShapeGenerator.getHeight();

                    //TODO: Check this part
                    lZPosition = (int)((lPlayerPosition.z + 1f * Balyrinth.Utilities.VIEW_SCALE) / (2f * Balyrinth.Utilities.VIEW_SCALE));
                    lXPosition = (int)((lPlayerPosition.x + 1f * Balyrinth.Utilities.VIEW_SCALE) / (2f * Balyrinth.Utilities.VIEW_SCALE));

                    lZPosition = Mathf.Clamp(lZPosition, 0, lNumberOfRows - 1);
                    lXPosition = Mathf.Clamp(lXPosition, 0, lNumberOfColumns - 1);

                    lRoomIndex = SquareShapeGenerator.getIndex(lXPosition, lZPosition, lNumberOfColumns, lNumberOfRows);
                }
                break;
        }

        return lRoomIndex;
    }

    void updateRoomVisibility(int pRoomIndex, GameObject pRoom)
    {

        RoomConnectionInterface lRoom = pRoom.GetComponent<RoomConnectionInterface>();
        lRoom.resetVisibility();
        Node lNode = m_MazeGenerator.getNode(pRoomIndex);
        pRoom.transform.position = m_MazeGenerator.getPosition(pRoomIndex);

        List<bool> lConnections = new List<bool>();
        for (int i = 0; i < m_MazeGenerator.mShapeGenerator.GetNumberOfDirections(); ++i)
        {
            lConnections.Add(m_MazeGenerator.areConnected(pRoomIndex, m_MazeGenerator.mShapeGenerator.getNextCellIndex(pRoomIndex, i)));
            //lRoom.m_ConnectionsActive[i] = ;
        }

        lRoom.SetConnections(lConnections);
      
        lRoom.Updatevisibility();
    }
}
