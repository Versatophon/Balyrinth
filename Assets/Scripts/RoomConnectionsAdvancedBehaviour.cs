using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct MultiParametersObjectVisibility
{
    public GameObject m_GameObject;
    public List<bool> m_MaskOnOpenedDirection;
}

[ExecuteInEditMode]
public class RoomConnectionsAdvancedBehaviour : MonoBehaviour, RoomConnectionInterface
{
    public bool m_UpdateInEditMode = false;

    public List<GameObject> m_WallsObjects = new List<GameObject>();
    public List<GameObject> m_AntiwallsObjects = new List<GameObject>();
    public GameObject m_Ceiling = null;

    //public List<GameObject> m_MultiDirectionsObjectsWest = new List<GameObject>(); 
    //public List<GameObject> m_MultiDirectionsObjectsNorth = new List<GameObject>(); 
    //public List<GameObject> m_MultiDirectionsObjectsBottom = new List<GameObject>(); 
    //public List<GameObject> m_MultiDirectionsObjectsEast = new List<GameObject>(); 
    //public List<GameObject> m_MultiDirectionsObjectsSouth = new List<GameObject>(); 
    //public List<GameObject> m_MultiDirectionsObjectsTop = new List<GameObject>(); 

    public List<MultiParametersObjectVisibility> m_MultiParametersObjectVisibility = new List<MultiParametersObjectVisibility>();

    [SerializeField]
    private List<bool> m_ConnectionsActive = new List<bool>();

    // Start is called before the first frame update
    void Start()
    {
    }

    public void SetConnections(List<bool> pConnectionState)
    {
        m_ConnectionsActive = pConnectionState;
    }
    public void resetVisibility()
    {
        m_ConnectionsActive.ForEach(lValue => lValue = false);
    }

    public void Updatevisibility()
    {
        bool lCeilingVisibility = true;
        for (int i = 0; i < Mathf.Min(m_ConnectionsActive.Count, m_WallsObjects.Count); ++i)
        {
            if (m_WallsObjects[i] != null)
            {
                m_WallsObjects[i]?.SetActive(!m_ConnectionsActive[i]);
            }
            lCeilingVisibility &= !m_ConnectionsActive[i];
        }

        for (int i = 0; i < Mathf.Min(m_ConnectionsActive.Count, m_AntiwallsObjects.Count) ; ++i)
        {
            if (m_AntiwallsObjects[i] != null)
            {
                m_AntiwallsObjects[i]?.SetActive(m_ConnectionsActive[i]);
            }
        }

        foreach  (MultiParametersObjectVisibility lObject in m_MultiParametersObjectVisibility)
        {
            bool lIsActive = true;
            for (int j = 0; j < Mathf.Min(m_ConnectionsActive.Count, lObject.m_MaskOnOpenedDirection.Count); ++j)
            {
                if(m_ConnectionsActive[j] && lObject.m_MaskOnOpenedDirection[j])
                {
                    lIsActive = false;
                    break;
                }
            }
            lObject.m_GameObject.SetActive(lIsActive);
        }

        if (m_Ceiling != null)
        {
            m_Ceiling?.SetActive(lCeilingVisibility);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (m_UpdateInEditMode)
        {
            Updatevisibility();
        }
    }
}
