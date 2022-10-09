using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RoomConnectionsBehaviour : MonoBehaviour, RoomConnectionInterface
{
    public bool m_UpdateInEditMode = false;

    //[Range(0, 255)]
    //public int m_RenderStencil;

    public List<GameObject> m_WallsObjects = new List<GameObject>();
    public List<GameObject> m_AntiwallsObjects = new List<GameObject>();
    public List<GameObject> m_AntiwallsObjectsWhenHighlighted = new List<GameObject>();

    public List<MeshRenderer> m_ObjectsToUpdateMaterials = new List<MeshRenderer>();

    public GameObject m_Ceiling = null;
    public GameObject m_Floor = null;

    public GameObject m_TPColliders = null;

    [SerializeField]
    private List<bool> m_ConnectionsActive = new List<bool>();

    public bool m_Highlighted = false;
    public Material m_StandardMaterial = null;
    public Material m_HighligthedMaterial = null;



    // Start is called before the first frame update
    void Start()
    {
    }

    public void SetConnections(List<bool> pConnectionState)
    {
        m_ConnectionsActive = pConnectionState;
    }

    public void SetTPCollidersActive(bool pActive)
    {
        if (m_TPColliders != null )
        {
            m_TPColliders.SetActive(pActive);
        }
    }

    public void SetHighlighted(bool pHighlighted)
    {
        m_Highlighted = pHighlighted;
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

        if (m_Floor != null)
        {
            MeshRenderer lMeshRenderer = m_Floor.GetComponentInChildren<MeshRenderer>();
            
            if( lMeshRenderer != null && m_HighligthedMaterial != null && m_StandardMaterial != null)
            {
                lMeshRenderer.material = m_Highlighted ? m_HighligthedMaterial : m_StandardMaterial;
            }
        }

        for (int i = 0; i < Mathf.Min(m_ConnectionsActive.Count, m_AntiwallsObjectsWhenHighlighted.Count); ++i)
        {
            if (m_AntiwallsObjectsWhenHighlighted[i] != null)
            {
                m_AntiwallsObjectsWhenHighlighted[i]?.SetActive(m_Highlighted && m_ConnectionsActive[i]);
            }
        }

        //foreach(MeshRenderer lMR in m_ObjectsToUpdateMaterials)
        //{
        //    lMR.material.SetInteger("_StencilID", m_RenderStencil);
        //}
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
