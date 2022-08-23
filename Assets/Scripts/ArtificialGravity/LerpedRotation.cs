using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpedRotation : MonoBehaviour
{
    Quaternion m_LerpedOrientation;
    public Transform m_ReferenceTransform;
    // Start is called before the first frame update
    void Start()
    {
        m_LerpedOrientation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        m_LerpedOrientation = Quaternion.Lerp(m_LerpedOrientation, m_ReferenceTransform.rotation, 0.1f);
        transform.rotation = m_LerpedOrientation;
    }
}
