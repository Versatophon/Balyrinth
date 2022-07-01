using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabyrinthGenerator : MonoBehaviour
{
   

    public Balyrinth.Utilities.LabyShape m_Shape = Balyrinth.Utilities.LabyShape.Rectangle;

    public int m_NumberOfColumns = 10;
    public int m_NumberOfRows = 10;

    public bool m_ProgressiveGeneration = false;

    public int m_AlgorithmIndex = 0;

    public int m_AlgorithmCorridorPseudoLength = 4;

    public float m_NumberOfConnectionsPerSeconds = 50f;

    public GameObject m_SquareRoomPrefab = null;
    public GameObject m_HexagonalRoomPrefab = null;

    public GameObject m_Player = null;

    public RoomsVisibilityUpdater m_RoomsVisibilityUpdater = null;
    public AdaptViewportToContent m_MapCamAdapter = null;

    Labyrinth mInternalRepresentation;
    MazeGenerator mMazeGenerator = null;

    float m_TimeCounter = 0f;

    List<GameObject> m_Rooms = new List<GameObject>();
    

    bool mNeedToCompute = false;

    // Start is called before the first frame update
    void Start()
    {
        //GameObject lObjectToInstantiate = null;

        m_RoomsVisibilityUpdater = GetComponent<RoomsVisibilityUpdater>();

        switch (m_Shape)
        {
            case Balyrinth.Utilities.LabyShape.Rectangle:
                m_RoomsVisibilityUpdater.mObjectToInstantiate = m_SquareRoomPrefab;
                break;
            case Balyrinth.Utilities.LabyShape.HoneyComb:
                m_RoomsVisibilityUpdater.mObjectToInstantiate = m_HexagonalRoomPrefab;
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

        InitiateGeneration();

        if( m_MapCamAdapter != null)
        {
            switch (m_Shape)
            {
                case Balyrinth.Utilities.LabyShape.Rectangle:
                    {
                        ShapeGeneratorInterface lShapeGeneratorInterface = new SquareShapeGenerator(m_NumberOfColumns, m_NumberOfRows);
                        mInternalRepresentation = lShapeGeneratorInterface.generate(m_NumberOfColumns, m_NumberOfRows);
                        m_MapCamAdapter.m_xMin = -1 * Balyrinth.Utilities.VIEW_SCALE;
                        m_MapCamAdapter.m_xMax = m_MapCamAdapter.m_xMin + m_NumberOfColumns * 2 * Balyrinth.Utilities.VIEW_SCALE;
                        m_MapCamAdapter.m_zMin = -5;
                        m_MapCamAdapter.m_zMax = 5;
                        m_MapCamAdapter.m_yMin = -1 * Balyrinth.Utilities.VIEW_SCALE;
                        m_MapCamAdapter.m_yMax = m_MapCamAdapter.m_yMin + m_NumberOfRows * 2 * Balyrinth.Utilities.VIEW_SCALE;
                    }
                    break;
                case Balyrinth.Utilities.LabyShape.HoneyComb:
                    {
                        ShapeGeneratorInterface lShapeGeneratorInterface = new HoneycombShapeGenerator(m_NumberOfColumns, m_NumberOfRows);
                        mInternalRepresentation = lShapeGeneratorInterface.generate(m_NumberOfColumns, m_NumberOfRows);
                        m_MapCamAdapter.m_xMin = (-Mathf.Sqrt(3) / 2) * Balyrinth.Utilities.VIEW_SCALE;
                        m_MapCamAdapter.m_xMax = m_MapCamAdapter.m_xMin + (m_NumberOfColumns + 0.5f) * Mathf.Sqrt(3) * Balyrinth.Utilities.VIEW_SCALE;
                        m_MapCamAdapter.m_zMin = -5;
                        m_MapCamAdapter.m_zMax = 5;
                        m_MapCamAdapter.m_yMin = -1 * Balyrinth.Utilities.VIEW_SCALE;
                        m_MapCamAdapter.m_yMax = m_MapCamAdapter.m_yMin + (m_NumberOfRows * 1.5f + 0.5f) * Balyrinth.Utilities.VIEW_SCALE;
                    }
                    break;
                case Balyrinth.Utilities.LabyShape.Sphere:
                    break;
                case Balyrinth.Utilities.LabyShape.Torus:
                    break;
            }

            m_MapCamAdapter.UpdateViewport();
        }
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

    public void respawnInputCB(UnityEngine.InputSystem.InputAction.CallbackContext pContext)
    {
        if (pContext.phase == UnityEngine.InputSystem.InputActionPhase.Performed)
        {
            InitiateGeneration();
            //Debug.Log("Respawn called with CB context");
        }
    }


    public void InitiateGeneration()
    {
        //Debug.Log("Regenerate Maze !");

        if (mNeedToCompute)
        {
            return;
        }

        mNeedToCompute = true;

        
        m_Player.transform.position = new Vector3(0, 
                                                  (UnityEngine.XR.Management.XRGeneralSettings.Instance?.Manager?.activeLoader == null ? 1.5f: 0),
                                                  0);

        m_Player.transform.rotation = Quaternion.identity;

        switch (m_Shape)
        {
            case Balyrinth.Utilities.LabyShape.Rectangle:
                {
                    ShapeGeneratorInterface lShapeGeneratorInterface = new SquareShapeGenerator(m_NumberOfColumns, m_NumberOfRows);
                    mInternalRepresentation = lShapeGeneratorInterface.generate(m_NumberOfColumns, m_NumberOfRows);
                }
                break;
            case Balyrinth.Utilities.LabyShape.HoneyComb:
                {
                    ShapeGeneratorInterface lShapeGeneratorInterface = new HoneycombShapeGenerator(m_NumberOfColumns, m_NumberOfRows);
                    mInternalRepresentation = lShapeGeneratorInterface.generate(m_NumberOfColumns, m_NumberOfRows);
                }
                break;
            case Balyrinth.Utilities.LabyShape.Sphere:
                break;
            case Balyrinth.Utilities.LabyShape.Torus:
                break;
        }

        mMazeGenerator = new MazeGenerator(mInternalRepresentation, m_Shape, m_NumberOfColumns, m_NumberOfRows);

        if (!m_ProgressiveGeneration)
        {
            mMazeGenerator.generate(m_AlgorithmIndex, m_NumberOfColumns, m_NumberOfRows, m_AlgorithmCorridorPseudoLength);
            mNeedToCompute = false;
        }



        m_RoomsVisibilityUpdater.m_MazeGenerator = mMazeGenerator;

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
