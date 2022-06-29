using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    
    public float m_Speed = 3;//m/s
    public float m_AngularSpeed = 120;//deg/s

    public bool m_FPSMoving = false;

    private Rigidbody m_Rigidbody;

    private Vector2 m_MovingInput = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        moveUsingInputs();
    }

    private void moveUsingInputs()
    {
        float lMoveDistance = m_Speed * Time.deltaTime;
        Vector3 lMoveVector = new Vector3();

        if (m_FPSMoving)
        {
            lMoveVector.z = m_MovingInput.y;

            transform.Rotate(m_MovingInput.x * m_AngularSpeed * Time.deltaTime * Vector3.up);
        }
        else
        {
            lMoveVector.z = m_MovingInput.y;
            lMoveVector.x = m_MovingInput.x;
        }

        Vector3 lFinalMoveVector = transform.TransformDirection(lMoveVector.normalized) * lMoveDistance;

        m_Rigidbody.transform.Translate(lFinalMoveVector, Space.World);
    }

    public void moveInputs(InputAction.CallbackContext pContext)
    {
        m_MovingInput = pContext.action.ReadValue<Vector2>();
    }
}
