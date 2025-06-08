using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapRotator : MonoBehaviour
{

    public RectTransform m_MapToRotate = null;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (m_MapToRotate != null)
        {
            m_MapToRotate.rotation = Quaternion.Euler(0f, 0f, transform.rotation.eulerAngles.y);
        }
    }
}
