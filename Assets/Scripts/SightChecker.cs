using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SightChecker : MonoBehaviour
{
    public GameObject m_Viewpoint = null;
    public GameObject m_MinimalView = null;
    public GameObject m_MaximalView = null;
    public GameObject m_MinimalObject = null;
    public GameObject m_MaximalObject = null;


    public float m_MinimalViewingAngle = 0;
    public float m_MaximalViewingAngle = 0;

    public float m_MinimalObjectAngle = 0;
    public float m_MaximalObjectAngle = 0;

    public float m_MinimalOverlapAngle = 0;
    public float m_MaximalOverlapAngle = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (m_Viewpoint != null && m_MinimalView != null && m_MaximalView != null && m_MinimalObject != null && m_MaximalObject != null)
        {
            m_MinimalViewingAngle = Mathf.Rad2Deg * Mathf.Atan2(m_MinimalView.transform.position.z - m_Viewpoint.transform.position.z, m_MinimalView.transform.position.x - m_Viewpoint.transform.position.x);
            m_MaximalViewingAngle = Mathf.Rad2Deg * Mathf.Atan2(m_MaximalView.transform.position.z - m_Viewpoint.transform.position.z, m_MaximalView.transform.position.x - m_Viewpoint.transform.position.x);

            m_MinimalObjectAngle = Mathf.Rad2Deg * Mathf.Atan2(m_MinimalObject.transform.position.z - m_Viewpoint.transform.position.z, m_MinimalObject.transform.position.x - m_Viewpoint.transform.position.x);
            m_MaximalObjectAngle = Mathf.Rad2Deg * Mathf.Atan2(m_MaximalObject.transform.position.z - m_Viewpoint.transform.position.z, m_MaximalObject.transform.position.x - m_Viewpoint.transform.position.x);


            Balyrinth.Utilities.isInSight(m_MinimalViewingAngle, m_MaximalViewingAngle, m_MinimalObjectAngle, m_MaximalObjectAngle, out m_MinimalOverlapAngle, out m_MaximalOverlapAngle);

#if false
            m_MinimalViewingAngle = Mathf.Repeat(m_MinimalViewingAngle, 360);
            m_MaximalViewingAngle = Mathf.Repeat(m_MaximalViewingAngle, 360);

            m_MinimalObjectAngle = Mathf.Repeat(m_MinimalObjectAngle, 360);
            m_MaximalObjectAngle = Mathf.Repeat(m_MaximalObjectAngle, 360);

            if (m_MaximalViewingAngle < m_MinimalViewingAngle)
            {
                m_MaximalViewingAngle += 360;
            }

            if (m_MaximalObjectAngle < m_MinimalObjectAngle)
            {
                m_MaximalObjectAngle += 360;
            }

            if (m_MinimalViewingAngle < m_MinimalObjectAngle && m_MaximalViewingAngle < m_MinimalObjectAngle)
            {
                m_MinimalViewingAngle += 360;
                m_MaximalViewingAngle += 360;
            }

            if (m_MinimalObjectAngle < m_MinimalViewingAngle && m_MaximalObjectAngle < m_MinimalViewingAngle)
            {
                m_MinimalObjectAngle += 360;
                m_MaximalObjectAngle += 360;
            }

            m_MinimalOverlapAngle = Mathf.Max(m_MinimalViewingAngle, m_MinimalObjectAngle);
            m_MaximalOverlapAngle = Mathf.Min(m_MaximalViewingAngle, m_MaximalObjectAngle);

#endif
        }
    }


   

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(m_Viewpoint.transform.position, m_MinimalView.transform.position);
        Gizmos.DrawLine(m_Viewpoint.transform.position, m_MaximalView.transform.position);
        
        Gizmos.color = Color.red;
        Gizmos.DrawLine(m_Viewpoint.transform.position, m_MinimalObject.transform.position);
        Gizmos.DrawLine(m_Viewpoint.transform.position, m_MaximalObject.transform.position);

        if (m_MinimalOverlapAngle < m_MaximalOverlapAngle)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(m_Viewpoint.transform.position, m_Viewpoint.transform.position + new Vector3(Mathf.Cos(Mathf.Deg2Rad * m_MinimalOverlapAngle), 0, Mathf.Sin(Mathf.Deg2Rad * m_MinimalOverlapAngle)) * 2f);
            Gizmos.DrawLine(m_Viewpoint.transform.position, m_Viewpoint.transform.position + new Vector3(Mathf.Cos(Mathf.Deg2Rad * m_MaximalOverlapAngle), 0, Mathf.Sin(Mathf.Deg2Rad * m_MaximalOverlapAngle)) * 2f);
        }
    }        
}