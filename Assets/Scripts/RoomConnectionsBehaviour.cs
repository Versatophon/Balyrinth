using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RoomConnectionsBehaviour : MonoBehaviour
{
    public bool m_UpdateInEditMode = false;

    public List<GameObject> m_WallsObjects = new List<GameObject>();
    public GameObject m_Ceiling = null;

    public List<bool> m_ConnectionsActive = new List<bool>();

    // Start is called before the first frame update
    void Start()
    {
    }

    public void resetVisibility()
    {
        m_ConnectionsActive.ForEach(lValue => lValue = false);
    }

    public void Updatevisibility()
    {
        bool lCeilingVisibility = true;
        for (int i = 0; i < m_ConnectionsActive.Count; ++i)
        {
            m_WallsObjects[i]?.SetActive(!m_ConnectionsActive[i]);
            lCeilingVisibility &= !m_ConnectionsActive[i];
        }

        m_Ceiling?.SetActive(lCeilingVisibility);
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
