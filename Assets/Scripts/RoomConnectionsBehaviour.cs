using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RoomConnectionsBehaviour : MonoBehaviour, RoomConnectionInterface
{
    public bool m_UpdateInEditMode = false;

    public List<GameObject> m_WallsObjects = new List<GameObject>();
    public List<GameObject> m_AntiwallsObjects = new List<GameObject>();
    public GameObject m_Ceiling = null;

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
