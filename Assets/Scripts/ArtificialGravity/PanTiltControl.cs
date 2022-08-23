using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PanTiltControl : MonoBehaviour
{
    public float m_Pan = 0;
    public float m_Tilt = 0;


    public float m_MinTilt = -45;
    public float m_MaxTilt = 45;


    public float m_MouseSensitivity = 0.2f;

    public float m_PlayerSpeed = 1;
    public float m_AngularSpeed = 120;//deg/s
    //public Rigidbody m_PlayerRigidbody = null;
    public ArtificialGravity m_ArtificialGravity = null;


    private Vector2 m_MoveInput = Vector2.zero;
    private Vector2 m_LookInput = Vector2.zero;

    private bool m_CleanMoving = false;
    private bool m_CleanLooking = false;

    private bool m_ActiveLooking = false;

    private float m_CurrentPlayerSpeed = 0; 

    //Vector3 mPreviousMousePosition = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Vector3 lCurrentMousePosition = Input.mousePosition;
        //Vector3 lDeltaMousePosition = lCurrentMousePosition - mPreviousMousePosition;


        //if (Input.GetKey(KeyCode.Mouse0))
        //{
        //    m_Tilt -= lDeltaMousePosition.y * m_MouseSensitivity;
        //    m_Pan += lDeltaMousePosition.x * m_MouseSensitivity;
        //}

        

        //mPreviousMousePosition = lCurrentMousePosition;

        
    }

    private void FixedUpdate()
    {
        moveUsingInputs();

        m_Tilt = Mathf.Clamp(m_Tilt, m_MinTilt, m_MaxTilt);
        m_Pan = Mathf.Repeat(m_Pan, 360);

        transform.localRotation = Quaternion.Euler(m_Tilt, m_Pan, 0);



        //if (Input.GetKey(KeyCode.Mouse1))
        {

            m_ArtificialGravity.SetMovingVector(Quaternion.Euler(0, m_Pan, 0) * (Vector3.forward +0.2f *Vector3.up) * m_CurrentPlayerSpeed);
            //m_PlayerRigidbody.AddForce(m_PlayerRigidbody.transform.TransformDirection(Quaternion.Euler(0,m_Pan,0)*Vector3.forward) * m_PlayerSpeed * m_PlayerRigidbody.mass, ForceMode.Impulse);

            //Debug.Log($"moving on");

        }
        //else
        {
            //m_ArtificialGravity.SetMovingVector(Vector3.zero);
        }
    }

    private void moveUsingInputs()
    {
        float lMoveDistance = m_PlayerSpeed * Time.deltaTime;
        Vector3 lMoveVector = new Vector3();

        //if (m_FPSMoving)
        {
            lMoveVector.z = m_MoveInput.y * lMoveDistance;
            lMoveVector.x = m_MoveInput.x * lMoveDistance;
            if (m_ActiveLooking)
            {
                m_Pan += m_LookInput.x * m_AngularSpeed * Time.deltaTime;
                m_Tilt += -m_LookInput.y * m_AngularSpeed * Time.deltaTime;
                //transform.Rotate(Vector3.up, m_LookInput.x * m_AngularSpeed * Time.deltaTime, Space.Self);
                //transform.Rotate(Vector3.right, -m_LookInput.y * m_AngularSpeed * Time.deltaTime, Space.Self);
            }
        }
        //else
        {
            //lMoveVector.z = m_MoveInput.y * lMoveDistance;
            //lMoveVector.x = m_MoveInput.x * lMoveDistance;
        }

        m_CurrentPlayerSpeed = lMoveVector.z;

        //Vector3 lFinalMoveVector;// = transform.TransformDirection(lMoveVector) * lMoveDistance;

        //if (m_ReferenceTransform != null)
        {
            //float lMagnitude = lMoveVector.magnitude;
            //Vector3 lDirectionVector = m_ReferenceTransform.forward;
            //lDirectionVector.y = 0;
            //lFinalMoveVector = m_ReferenceTransform.forward * lMoveVector.z;// + m_ReferenceTransform.right * lMoveVector.x;
        }
        //else
        {
            //lFinalMoveVector = transform.TransformDirection(lMoveVector) * lMoveDistance;
        }


        //if(m_ImInVR)
        //{
        //    transform.Translate(lFinalMoveVector, Space.World);
        //}
        //else
        {
            //m_Rigidbody.transform.Translate(lFinalMoveVector, Space.World);
        }


        if (m_CleanMoving)
        {
            m_CleanMoving = false;
            m_MoveInput = Vector2.zero;
        }


        if (m_CleanLooking)
        {
            m_CleanLooking = false;
            m_LookInput = Vector2.zero;
        }

#if false//for VR
        //Put values To Zero
        m_MovingInput = Vector2.zero;
        m_LookInput = Vector2.zero;
#endif
    }


    public void moveInputs(InputAction.CallbackContext pContext)
    {
#if false//for VR
        if (pContext.phase == InputActionPhase.Performed)
        {
            m_MovingInput = pContext.action.ReadValue<Vector2>();// new Vector2(pContext.action.ReadValue<Vector2>().x , UnityEngine.UIElements.Experimental.Easing.InQuad(pContext.action.ReadValue<Vector2>().y)*Mathf.Sign(pContext.action.ReadValue<Vector2>().y));
            //Debug.Log($"Moving input change : {m_MovingInput} at frame {Time.frameCount} with ID {pContext.action.id}");
        }
        //Debug.Log($"Moving input change : {m_MovingInput} at frame {Time.frameCount} with ID {pContext.action.id}");
#else
        //Debug.Log($"Moving input change : {pContext.valueType} with value {pContext.action.ReadValue<Vector2>()} at frame {Time.frameCount} with ID {pContext.action.id} and phased as {pContext.phase}");

        if (pContext.control.device.layout == "Keyboard" || pContext.control.device.layout == "Mouse")
        {
            m_MoveInput = pContext.action.ReadValue<Vector2>();
        }
        else
        {
            if (pContext.phase == InputActionPhase.Performed)
            {
                m_MoveInput = pContext.action.ReadValue<Vector2>();
                m_CleanMoving = true;
            }
        }
#endif
    }

    public void lookInputs(InputAction.CallbackContext pContext)
    {
#if false//for VR
        if (pContext.phase == InputActionPhase.Performed)
        {
            m_LookInput = pContext.action.ReadValue<Vector2>();
            //Debug.Log($"Looking input change : {m_LookInput} at frame {Time.frameCount} with ID {pContext.action.id}");
        }
        else
        {
            //Debug.Log($"Context Phase : {pContext.}");
        }
        //Debug.Log($"Looking input change : {pContext.action.ReadValue<Vector2>()} at frame {Time.frameCount} with ID {pContext.action.id}");
#else
        //Debug.Log($"Looking input change : {pContext.valueType} with value {pContext.action.ReadValue<Vector2>()} at frame {Time.frameCount} with ID {pContext.action.id} and phased as {pContext.phase}");
        //Debug.Log($"Looking input change : {m_LookInput} at frame {Time.frameCount} with ID {pContext.action.id}");

        if (pContext.control.device.layout == "Keyboard")
        {
            m_LookInput = pContext.action.ReadValue<Vector2>();
        }
        else
        {
            if (pContext.phase == InputActionPhase.Performed)
            {
                m_LookInput = pContext.action.ReadValue<Vector2>();
                m_CleanLooking = true;
            }
        }

#endif
    }

    public void activeLookInput(UnityEngine.InputSystem.InputAction.CallbackContext pContext)
    {
        switch (pContext.phase)
        {
            case InputActionPhase.Canceled:
                m_ActiveLooking = false;
                break;
            case InputActionPhase.Started:
                m_ActiveLooking = true;
                break;
        }
    }
}
