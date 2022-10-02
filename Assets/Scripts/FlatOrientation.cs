using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class FlatOrientation : MonoBehaviour
{
    public Transform m_ReferenceTransform = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (m_ReferenceTransform != null)
        {
            float lFlatAngle = Mathf.Atan2(m_ReferenceTransform.right.z, m_ReferenceTransform.right.x);
            transform.rotation = Quaternion.AngleAxis(lFlatAngle * Mathf.Rad2Deg, Vector3.up);
        }
    }
}
