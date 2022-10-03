using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//TODO : Offset world here according non-euclidian space
//TODO : Make a better current room resolution (especially in hexagonal pattern) - IDEA: Highlight current room floor


public class RoomsVisibilityUpdater : MonoBehaviour
{
    public GameObject m_PlayerViewer = null;

    //public GameObject m_SquareRoomPrefab = null;
    //public GameObject m_HexagonalRoomPrefab = null;
    //public GameObject m_HypermazeRoomPrefab = null;
    //public GameObject m_OctogonalRoomPrefab = null;


    //Debug vars
    public bool m_DisplayDebug = false;
    public GameObject m_LeftSpot = null;
    public GameObject m_RightSpot = null; 
    public GameObject m_FirstDoorLeftSpot = null;
    public GameObject m_FirstDoorRightSpot = null;

    private GameObject mObjectToInstantiate;

    private bool m_FirstDoorLocated = false;

    private Vector3 mCumulatedOffset = Vector3.zero;
    private int mCurrentRoomIndex = -1;

    private int m_BeginIndex = -1;
    private GameObject m_MazeBegin = null;
    private int m_EndIndex = -1;
    private GameObject m_MazeEnd = null;
    private Vector3 m_CPOffset = Vector3.zero;

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

    public MazeGeneratorManager m_MazeGeneratorManager = null;

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
    List<Vector3> mRoomCorrectedPosition = new List<Vector3>();

    // Update is called once per frame
    void Update()
    {
        if (m_MazeGeneratorManager != null)
        {
            ShapeGeneratorInterface lShapeGenerator = m_MazeGeneratorManager.GetMazeGenerator().mShapeGenerator;
            int lNumDirections = lShapeGenerator.NumberOfDirections;

            //List<int> lVisibleRoomsIndices = new List<int>();
            mVisibleRoomsIndices.Clear();
            mRoomCorrectedPosition.Clear();
            //Debug.Log("Update");

            int lCurrentRoomIndex = ResolveCurrentRoom();
            if (lCurrentRoomIndex != -1)
            {
                mVisibleRoomsIndices.Add(lCurrentRoomIndex);
                mRoomCorrectedPosition.Add(lShapeGenerator.getRoomPosition(lCurrentRoomIndex) + mCumulatedOffset);

                Vector3 lLeftViewVector = m_PlayerViewer.transform.TransformDirection(-Vector3.right + Vector3.forward);
                //Vector3 lLeftViewVector = m_PlayerViewer.transform.TransformDirection(-Vector3.right);
                Vector3 lRightViewVector = m_PlayerViewer.transform.TransformDirection(Vector3.right + Vector3.forward);
                //Vector3 lRightViewVector = m_PlayerViewer.transform.TransformDirection(Vector3.right);

                if(m_DisplayDebug && m_LeftSpot != null && m_RightSpot != null )
                {
                    m_LeftSpot.transform.position = m_PlayerViewer.transform.position + lLeftViewVector;
                    m_RightSpot.transform.position = m_PlayerViewer.transform.position + lRightViewVector;
                }

                m_FirstDoorLocated = false;

                RecursiveSeeThrough(lCurrentRoomIndex, lShapeGenerator.getRoomPosition(lCurrentRoomIndex) + mCumulatedOffset, -1, lNumDirections, Mathf.Atan2(lRightViewVector.z, lRightViewVector.x), Mathf.Atan2(lLeftViewVector.z, lLeftViewVector.x));
            }

            while ( m_Rooms.Count < mVisibleRoomsIndices.Count )
            {
                m_Rooms.Add(GameObject.Instantiate(mObjectToInstantiate,
                  transform.TransformPoint(new Vector3(-10, -10, 0) * Balyrinth.Utilities.VIEW_SCALE),
                  Quaternion.identity, transform));
            }

            if ( m_MazeBegin != null && m_MazeEnd != null )
            {
                m_MazeBegin.SetActive(false);
                m_MazeEnd.SetActive(false);
            }

            //TODO: need to compute all visu updates here
            for (int i = 0; i < m_Rooms.Count; ++i)
            {
                if (i < mVisibleRoomsIndices.Count)
                {
                    m_Rooms[i].SetActive(true);

                    if (m_MazeBegin != null && m_MazeEnd != null)
                    {
                        if(m_BeginIndex == mVisibleRoomsIndices[i])
                        {
                            m_MazeBegin.transform.position = mRoomCorrectedPosition[i] + m_CPOffset;
                            m_MazeBegin.SetActive(true);
                        }

                        if(m_EndIndex == mVisibleRoomsIndices[i])
                        {
                            m_MazeEnd.transform.position = mRoomCorrectedPosition[i] + m_CPOffset;
                            m_MazeEnd.SetActive(true);
                        }
                    }

                    //TODO: Check how it performs
                    updateRoomVisibility(i);// /*mVisibleRoomsIndices[i],*/ m_Rooms[i], mRoomCorrectedPosition);
                }
                else
                {
                    m_Rooms[i].SetActive(false);
                    m_Rooms[i].transform.position = new Vector3(-10, 0, -10) * Balyrinth.Utilities.VIEW_SCALE;
                }
            }
        }
    }

    public void Init()
    {
        mCumulatedOffset = Vector3.zero;
        mCurrentRoomIndex = -1;
    }

    public void SetCheckpoints(int pBeginIndex, GameObject pBegin, int pEndIndex, GameObject pEnd, Vector3 pCPOffset)
    {
        m_MazeBegin = pBegin;
        m_MazeEnd = pEnd;

        m_BeginIndex = pBeginIndex;
        m_EndIndex = pEndIndex;

        m_CPOffset = pCPOffset;
    }

    private void RecursiveSeeThrough(int pRoomIndex, Vector3 pComputedRoomPosition, int pFromDirection, int pNumDirections, float pMinAngleRadians, float pMaxAngleRadians)
    {
        MazeGenerator lMazeGenerator = m_MazeGeneratorManager.GetMazeGenerator();
        ShapeGeneratorInterface lShapeGenerator = lMazeGenerator.mShapeGenerator;

        //Debug.Log("Comming From " + pRoomIndex + " to " + pFromDirection);
        for (int i = 0; i < pNumDirections; ++i)
        {
            if(pFromDirection == -1 || i != lShapeGenerator.getOppositeDirection(pFromDirection))
            {
                int lNextCellIndex = lShapeGenerator.getNextCellIndex(pRoomIndex, i);

                if (lMazeGenerator.areConnected(pRoomIndex, lNextCellIndex))
                {
                    float lMinimalComputedAngleRadians;
                    float lMaximalComputedAngleRadians;
                    Vector3 lLeftExtremity;
                    Vector3 lRightExtremity;

                    lShapeGenerator.getDoorLocalExtremities(i, out lLeftExtremity, out lRightExtremity);

                    lLeftExtremity += pComputedRoomPosition;
                    lRightExtremity += pComputedRoomPosition;

                    if (m_DisplayDebug && !m_FirstDoorLocated && m_FirstDoorLeftSpot != null && m_FirstDoorRightSpot != null)
                    {
                        m_FirstDoorLeftSpot.transform.position = lLeftExtremity;
                        m_FirstDoorRightSpot.transform.position = lRightExtremity;
                        m_FirstDoorLocated = true;
                    }

                    float lMinimalLocalAngleRadians = Mathf.Atan2((lRightExtremity.z - m_PlayerViewer.transform.position.z), lRightExtremity.x - m_PlayerViewer.transform.position.x);
                    float lMaximalLocalAngleRadians = Mathf.Atan2((lLeftExtremity.z - m_PlayerViewer.transform.position.z), lLeftExtremity.x - m_PlayerViewer.transform.position.x);
                    //m_LeftSpot

                    //Debug.Log($"Angles ({Mathf.Rad2Deg * lMinimalLocalAngle}, {Mathf.Rad2Deg * lMaximalLocalAngle})");

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
                    bool lHasBeenAdded = false;
                    if (Balyrinth.Utilities.isMinimalAngleRadiansIsDirect(lMinimalLocalAngleRadians, lMaximalLocalAngleRadians))
                    {
                        if (Balyrinth.Utilities.isInSightRadians(pMinAngleRadians, pMaxAngleRadians, lMinimalLocalAngleRadians, lMaximalLocalAngleRadians, out lMinimalComputedAngleRadians, out lMaximalComputedAngleRadians))
                        {
                            mVisibleRoomsIndices.Add(lNextCellIndex);
                            mRoomCorrectedPosition.Add(pComputedRoomPosition + lShapeGenerator.getDirectionOffset(i));

                            RecursiveSeeThrough(lNextCellIndex, pComputedRoomPosition + lShapeGenerator.getDirectionOffset(i), i, pNumDirections, lMinimalComputedAngleRadians, lMaximalComputedAngleRadians);
                            lHasBeenAdded = true;
                        }
                    }

                    if( pFromDirection == -1 && !lHasBeenAdded )
                    {
                        mVisibleRoomsIndices.Add(lNextCellIndex);
                        mRoomCorrectedPosition.Add(pComputedRoomPosition + lShapeGenerator.getDirectionOffset(i));
                    }

                }
                
            }
        }
    }

    int ResolveCurrentRoom()
    {
       // int lZPosition = 0;
       // int lXPosition = 0;


        if(mCurrentRoomIndex != -1)
        {

        }

        int lRoomIndex = -1;

        //ShapeGeneratorInterface lSGInterface = null;

        Vector3 lPlayerPosition = m_PlayerViewer.transform.position;

#if false
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
            case Balyrinth.Utilities.LabyShape.Octogonized:
                {
                    lRoomIndex = m_MazeGenerator.mShapeGenerator.getRoomIndex(lPlayerPosition);

#if false
                    //lSGInterface = new SquareShapeGenerator(m_NumberOfColumns, m_NumberOfRows);

                    int lNumberOfColumns = m_MazeGenerator.mShapeGenerator.getWidth();
                    int lNumberOfRows = m_MazeGenerator.mShapeGenerator.getHeight();

                    //TODO: Check this part
                    lZPosition = (int)((lPlayerPosition.z + 1f * Balyrinth.Utilities.VIEW_SCALE) / (2f * Balyrinth.Utilities.VIEW_SCALE));
                    lXPosition = (int)((lPlayerPosition.x + 1f * Balyrinth.Utilities.VIEW_SCALE) / (2f * Balyrinth.Utilities.VIEW_SCALE));

                    lZPosition = Mathf.Clamp(lZPosition, 0, lNumberOfRows - 1);
                    lXPosition = Mathf.Clamp(lXPosition, 0, lNumberOfColumns - 1);

                    lRoomIndex = m_MazeGenerator.mShapeGenerator.getIndex(lXPosition, lZPosition, lNumberOfColumns, lNumberOfRows);
#endif
                }
                break;
        }
#endif
        Vector3 lOffsetToAdd = Vector3.zero;
        lRoomIndex = m_MazeGeneratorManager.GetMazeGenerator().mShapeGenerator.getRoomIndexNonEuclidian(lPlayerPosition - mCumulatedOffset, out lOffsetToAdd);
        mCumulatedOffset += lOffsetToAdd;

        return lRoomIndex;
    }

    void updateRoomVisibility(int pVisibleIndex)//(int  pRoomIndex, GameObject pRoom)
    {
        MazeGenerator lMazeGenerator = m_MazeGeneratorManager.GetMazeGenerator();

        GameObject lFoundRoum = m_Rooms[pVisibleIndex];
        RoomConnectionInterface lRoom = lFoundRoum.GetComponent<RoomConnectionInterface>();
        lRoom.resetVisibility();
        lRoom.SetTPCollidersActive(pVisibleIndex == 0);
        lRoom.SetHighlighted(pVisibleIndex == 0);
        //Node lNode = m_MazeGenerator.getNode(pRoomIndex);
        lFoundRoum.transform.position = mRoomCorrectedPosition[pVisibleIndex];//m_MazeGenerator.getPosition(pRoomIndex);

        List<bool> lConnections = new List<bool>();
        for (int i = 0; i < lMazeGenerator.mShapeGenerator.NumberOfDirections; ++i)
        {
            lConnections.Add(lMazeGenerator.areConnected(mVisibleRoomsIndices[pVisibleIndex], lMazeGenerator.mShapeGenerator.getNextCellIndex(mVisibleRoomsIndices[pVisibleIndex], i)));
            //lRoom.m_ConnectionsActive[i] = ;
        }

        lRoom.SetConnections(lConnections);
      
        lRoom.Updatevisibility();
    }
}
