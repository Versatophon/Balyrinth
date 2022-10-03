using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonEuclidianTPBehaviour : MonoBehaviour
{
    ArtificialGravity m_Player;
    public Vector3 m_TPOffset;
    // Start is called before the first frame update
    void Start()
    {
        //TODO: Find a better way to achieve this
        m_Player = GameObject.FindObjectOfType<ArtificialGravity>();   
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider pCollider)
    {
        //Debug.Log($"{pCollider} triggered");


        if (m_Player != null && pCollider.gameObject == m_Player.gameObject)
        {
            Vector3 lLocalPloayerMovingDirection = transform.TransformDirection(m_Player.getMovingDirection());

            if( true || lLocalPloayerMovingDirection.z > 0)
            {
                m_Player.transform.position += m_TPOffset;
                Debug.Log($"{m_Player} TP by {m_TPOffset}");
            }
            else
            { 
                Debug.Log($"{m_Player} No TP cause {lLocalPloayerMovingDirection}");
            }

            //Debug.Log($"Triggered by {m_Player}");
        }
    }
}
