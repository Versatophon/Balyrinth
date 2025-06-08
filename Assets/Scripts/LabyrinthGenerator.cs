using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LabyrinthGenerator : MonoBehaviour, MazeGenerationListener
{
    public MazeGeneratorManager m_MazeGeneratorManager;

    public GameObject m_SquareRoomPrefab = null;
    public GameObject m_HexagonalRoomPrefab = null;
    public GameObject m_OctogonalRoomSt0Prefab = null;
    public GameObject m_OctogonalRoomSt1Prefab = null;
    public GameObject m_HypermazeRoomPrefab = null;

    public ArtificialGravity m_Player = null;

    public Vector3 m_SpawnPointsOffset;
    public GameObject m_Starting = null;
    public GameObject m_Goal = null;

    public RoomsVisibilityUpdater m_RoomsVisibilityUpdater = null;
    public AdaptViewportToContent m_MapCamAdapter = null;

    public OrbitalManipulator m_OrbitalManipulator = null;

    public bool m_LoopGeneration = false;

    GameObject m_StartCP = null;
    GameObject m_GoalCP = null;

    List<GameObject> m_Rooms = new List<GameObject>();

    bool mNeedToCompute = false;

    // Start is called before the first frame update
    void Start()
    {
        //GameObject lObjectToInstantiate = null;

        m_RoomsVisibilityUpdater = GetComponent<RoomsVisibilityUpdater>();
        
        m_MazeGeneratorManager?.SetGenerationListener(this);
        InitiateGeneration();
    }

    void SelectPrefabToInstantiate()
    {
        switch (m_MazeGeneratorManager.m_Shape)
        {
            case Balyrinth.Utilities.LabyShape.Rectangle:
                m_RoomsVisibilityUpdater.ObjectToInstantiateSt0 = m_SquareRoomPrefab;
                m_RoomsVisibilityUpdater.ObjectToInstantiateSt1 = null;
                break;
            case Balyrinth.Utilities.LabyShape.HoneyComb:
                m_RoomsVisibilityUpdater.ObjectToInstantiateSt0 = m_HexagonalRoomPrefab;
                m_RoomsVisibilityUpdater.ObjectToInstantiateSt1 = null;
                break;
            case Balyrinth.Utilities.LabyShape.Octogonized:
                m_RoomsVisibilityUpdater.ObjectToInstantiateSt0 = m_OctogonalRoomSt0Prefab;
                m_RoomsVisibilityUpdater.ObjectToInstantiateSt1 = m_OctogonalRoomSt1Prefab;
                break;
            case Balyrinth.Utilities.LabyShape.Hypermaze:
                m_RoomsVisibilityUpdater.ObjectToInstantiateSt0 = m_HypermazeRoomPrefab;
                m_RoomsVisibilityUpdater.ObjectToInstantiateSt1 = null;
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
    }

    void RecomputeMapCamBounds()
    {
        if (m_MapCamAdapter != null)
        {
            switch (m_MazeGeneratorManager.m_Shape)
            {
                case Balyrinth.Utilities.LabyShape.Rectangle:
                    {
                        //ShapeGeneratorInterface lShapeGeneratorInterface = new SquareShapeGenerator(m_NumberOfColumns, m_NumberOfRows);
                        //mInternalRepresentation = lShapeGeneratorInterface.generate(m_NumberOfColumns, m_NumberOfRows);
                        m_MapCamAdapter.m_xMin = -1 * Balyrinth.Utilities.VIEW_SCALE;
                        m_MapCamAdapter.m_xMax = m_MapCamAdapter.m_xMin + m_MazeGeneratorManager.m_NumberOfColumns * 2 * Balyrinth.Utilities.VIEW_SCALE;
                        m_MapCamAdapter.m_zMin = -5;
                        m_MapCamAdapter.m_zMax = 5;
                        m_MapCamAdapter.m_yMin = -1 * Balyrinth.Utilities.VIEW_SCALE;
                        m_MapCamAdapter.m_yMax = m_MapCamAdapter.m_yMin + m_MazeGeneratorManager.m_NumberOfRows * 2 * Balyrinth.Utilities.VIEW_SCALE;
                    }
                    break;
                case Balyrinth.Utilities.LabyShape.HoneyComb:
                    {
                        //ShapeGeneratorInterface lShapeGeneratorInterface = new HoneycombShapeGenerator(m_NumberOfColumns, m_NumberOfRows);
                        //mInternalRepresentation = lShapeGeneratorInterface.generate(m_NumberOfColumns, m_NumberOfRows);
                        m_MapCamAdapter.m_xMin = (-Mathf.Sqrt(3) / 2) * Balyrinth.Utilities.VIEW_SCALE;
                        m_MapCamAdapter.m_xMax = m_MapCamAdapter.m_xMin + (m_MazeGeneratorManager.m_NumberOfColumns + 0.5f) * Mathf.Sqrt(3) * Balyrinth.Utilities.VIEW_SCALE;
                        m_MapCamAdapter.m_zMin = -5;
                        m_MapCamAdapter.m_zMax = 5;
                        m_MapCamAdapter.m_yMin = -1 * Balyrinth.Utilities.VIEW_SCALE;
                        m_MapCamAdapter.m_yMax = m_MapCamAdapter.m_yMin + (m_MazeGeneratorManager.m_NumberOfRows * 1.5f + 0.5f) * Balyrinth.Utilities.VIEW_SCALE;
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

    // Update is called once per frame
    void Update()
    {
        if(mNeedToCompute)
        {
            int[] lUpdatedNodes = m_MazeGeneratorManager.UpdateGeneration();

            foreach (int lNodeIndex in lUpdatedNodes)
            {
                GameObject lRoom = m_Rooms[lNodeIndex];
                RoomConnectionInterface lRoomConnections = lRoom.GetComponent<RoomConnectionInterface>();

                lRoom.SetActive(true);

                bool[] lLocalConnections = m_MazeGeneratorManager.GetConnectedDirections(lNodeIndex);
                lRoomConnections.SetConnections(lLocalConnections.ToList());

                lRoomConnections.Updatevisibility();
            }

        }
    }

    public void SetNumberOfColumns(string pNumberOfColumns)
    {
        m_MazeGeneratorManager.SetNumberOfColumns(pNumberOfColumns);
    }

    public void SetNumberOfRows(string pNumberOfRows)
    {
        m_MazeGeneratorManager.SetNumberOfRows(pNumberOfRows);
    }

    public void setShapeType(int pShapeType)
    {
        m_MazeGeneratorManager.SetShapeType(pShapeType);
    }

    public void respawnInputCB(UnityEngine.InputSystem.InputAction.CallbackContext pContext)
    {

        //TODO: Find a way to resolve as needed
#if false//VR
        if (pContext.phase == UnityEngine.InputSystem.InputActionPhase.Performed)
        {
            InitiateGeneration();
            //Debug.Log("Respawn called with CB context");
        }
#else
        InitiateGeneration();
#endif
    }

    void SpawnPlayer()
    {
        if (m_Player != null)
        {
            m_RoomsVisibilityUpdater.Init();
            m_Player.Teleport(m_MazeGeneratorManager.GetObjectPosition(m_MazeGeneratorManager.GetStartingNodeIndex()) + new Vector3(0, (UnityEngine.XR.Management.XRGeneralSettings.Instance?.Manager?.activeLoader == null ? 1.5f : 0), 0), Vector3.down);
            m_RoomsVisibilityUpdater.SetCheckpoints(m_MazeGeneratorManager.GetStartingNodeIndex(), m_StartCP, m_MazeGeneratorManager.GetEndingNodeIndex(), m_GoalCP, m_SpawnPointsOffset);
        }
    }

    public void InitiateGeneration()
    {
        if (mNeedToCompute)
        {
            return;
        }

        mNeedToCompute = true;

        SelectPrefabToInstantiate();

        m_MazeGeneratorManager.InitiateGeneration();

        //TODO: Find another way to handle this
        m_RoomsVisibilityUpdater.m_MazeGeneratorManager = m_MazeGeneratorManager;

        //m_TimeCounter = 0;


        if (m_MazeGeneratorManager.m_Shape == Balyrinth.Utilities.LabyShape.Hypermaze)
        {
            foreach (GameObject lGo in m_Rooms)
            {
                GameObject.Destroy(lGo);
            }

            m_Rooms.Clear();

            Vector3 lMinPosition = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 lMaxPosition = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            for (int i = 0; i < m_MazeGeneratorManager.GetNumberOfNodes(); ++i)
            {
                Vector3 lPosition = m_MazeGeneratorManager.GetObjectPosition(i);
                GameObject lRoom = GameObject.Instantiate(m_HypermazeRoomPrefab, lPosition, Quaternion.identity, transform);
                m_Rooms.Add(lRoom);
            
                lRoom.SetActive(false);
                
                if( lMaxPosition.x < lPosition.x )
                {
                    lMaxPosition.x = lPosition.x;
                }

                if (lMaxPosition.y < lPosition.y)
                {
                    lMaxPosition.y = lPosition.y;
                }

                if (lMaxPosition.z < lPosition.z)
                {
                    lMaxPosition.z = lPosition.z;
                }

                if (lMinPosition.x > lPosition.x)
                {
                    lMinPosition.x = lPosition.x;
                }

                if (lMinPosition.y > lPosition.y)
                {
                    lMinPosition.y = lPosition.y;
                }

                if (lMinPosition.z > lPosition.z)
                {
                    lMinPosition.z = lPosition.z;
                }


                //RoomConnectionsBehaviour lRoomConnectionsBehaviour = lRoom.GetComponent<RoomConnectionsBehaviour>();
                //
                //bool[] lLocalConnections = m_MazeGeneratorManager.GetConnectedDirections(i);
                //
                //lRoomConnectionsBehaviour.m_ConnectionsActive = lLocalConnections.ToList();
                //
                //lRoomConnectionsBehaviour.Updatevisibility();
            }

            if(m_OrbitalManipulator != null)
            {
                m_OrbitalManipulator.transform.position = (lMinPosition + lMaxPosition) * 0.5f;

                m_OrbitalManipulator.SetCameraStartupDistance((lMaxPosition - lMinPosition).magnitude);
            }

        }
    }

    public void GenerationDone()
    {
        if (m_StartCP == null)
        {
            m_StartCP = GameObject.Instantiate(m_Starting);
        }

        if (m_GoalCP == null)
        {
            m_GoalCP = GameObject.Instantiate(m_Goal);
        }

        m_StartCP.transform.position = m_MazeGeneratorManager.GetObjectPosition(m_MazeGeneratorManager.GetStartingNodeIndex()) + m_SpawnPointsOffset;
        m_GoalCP.transform.position = m_MazeGeneratorManager.GetObjectPosition(m_MazeGeneratorManager.GetEndingNodeIndex()) + m_SpawnPointsOffset;

        m_StartCP.SetActive(false);
        m_GoalCP.SetActive(false);

        mNeedToCompute = false;

        SpawnPlayer();

        if ( m_LoopGeneration )
        {
            InitiateGeneration();
        }
    }
}
