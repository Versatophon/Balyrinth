using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RoomConnectionsBehaviour : MonoBehaviour
{
    public GameObject m_WallFromNorth = null;
    public GameObject m_WallFromEast = null;
    public GameObject m_WallFromSouth = null;
    public GameObject m_WallFromWest = null;
    public GameObject m_Ceiling = null;

    public bool m_ConnectedFromNorth = false;
    public bool m_ConnectedFromEast = false;
    public bool m_ConnectedFromSouth = false;
    public bool m_ConnectedFromWest = false;

    public int m_Color = 0;


    // Start is called before the first frame update
    void Start()
    {
    }

    public void resetVisibility()
    {
        m_ConnectedFromNorth = false;
        m_ConnectedFromEast = false;
        m_ConnectedFromSouth = false;
        m_ConnectedFromWest = false;
    }

    public void Updatevisibility()
    {
        m_WallFromNorth?.SetActive(!m_ConnectedFromNorth);
        m_WallFromEast?.SetActive(!m_ConnectedFromEast);
        m_WallFromSouth?.SetActive(!m_ConnectedFromSouth);
        m_WallFromWest?.SetActive(!m_ConnectedFromWest);


        m_Ceiling?.SetActive(!(m_ConnectedFromNorth || m_ConnectedFromEast || m_ConnectedFromSouth || m_ConnectedFromWest));
    }

    // Update is called once per frame
    void Update()
    {
        //Updatevisibility();
    }
}
