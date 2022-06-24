using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{

    public float m_Speed = 3;//m/s
    public float m_AngularSpeed = 120;//deg/s

    public bool m_FPSMoving = false;

    private Rigidbody m_Rigidbody; 

    // Start is called before the first frame update
    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        float lMoveDistance = m_Speed * Time.deltaTime;
        Vector3 lMoveVector = new Vector3();

        if (m_FPSMoving)
        {

            if (Input.GetKey(KeyCode.UpArrow))
            {
                lMoveVector += transform.forward;
            }

            if (Input.GetKey(KeyCode.DownArrow))
            {
                lMoveVector -= transform.forward;
            }

            if (Input.GetKey(KeyCode.RightArrow))
            {
                transform.Rotate( m_AngularSpeed * Time.deltaTime * Vector3.up);
            }

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                transform.Rotate(-m_AngularSpeed * Time.deltaTime * Vector3.up);
            }
        }
        else
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                lMoveVector.z += 1;
            }

            if (Input.GetKey(KeyCode.DownArrow))
            {
                lMoveVector.z -= 1;
            }

            if (Input.GetKey(KeyCode.RightArrow))
            {
                lMoveVector.x += 1;
            }

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                lMoveVector.x -= 1;
            }
        }

        

        Vector3 lFinalMoveVector = lMoveVector.normalized * lMoveDistance;

        m_Rigidbody.transform.Translate(lFinalMoveVector, Space.World);

        //m_Rigidbody.AddForce(lFinalForceVector, ForceMode.);   
    }
}
