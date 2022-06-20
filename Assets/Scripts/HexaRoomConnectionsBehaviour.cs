using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexaRoomConnectionsBehaviour : MonoBehaviour
{
    public GameObject m_WallFromNorthWest = null;
    public GameObject m_WallFromNorthEast = null;
    public GameObject m_WallFromEast = null;
    public GameObject m_WallFromSouthEast = null;
    public GameObject m_WallFromSouthWest = null;
    public GameObject m_WallFromWest = null;
    public GameObject m_Ceiling = null;

    public bool m_ConnectedFromNorthWest = false;
    public bool m_ConnectedFromNorthEast = false;
    public bool m_ConnectedFromEast = false;
    public bool m_ConnectedFromSouthEast = false;
    public bool m_ConnectedFromSouthWest = false;
    public bool m_ConnectedFromWest = false;

    public int m_Color = 0;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void resetVisibility()
    {
        m_ConnectedFromNorthWest = false;
        m_ConnectedFromNorthEast = false;
        m_ConnectedFromEast = false;
        m_ConnectedFromSouthEast = false;
        m_ConnectedFromSouthWest = false;
        m_ConnectedFromWest = false;
    }

    public void Updatevisibility()
    {
        m_WallFromNorthWest?.SetActive(!m_ConnectedFromNorthWest);
        m_WallFromNorthEast?.SetActive(!m_ConnectedFromNorthEast);
        m_WallFromEast?.SetActive(!m_ConnectedFromEast);
        m_WallFromSouthEast?.SetActive(!m_ConnectedFromSouthEast);
        m_WallFromSouthWest?.SetActive(!m_ConnectedFromSouthWest);
        m_WallFromWest?.SetActive(!m_ConnectedFromWest);


        m_Ceiling?.SetActive(!(m_ConnectedFromNorthWest || m_ConnectedFromNorthEast || m_ConnectedFromEast || m_ConnectedFromSouthEast || m_ConnectedFromSouthWest || m_ConnectedFromWest));
    }
}
