//#define USE_LEGACY_INPUT

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;




public class PlayerControlHyperMaze : MonoBehaviour
{
    
    public float m_Speed = 3;//m/s
    public float m_AngularSpeed = 120;//deg/s

    public bool m_FPSMoving = false;
    ///public bool m_ImInVR = false;
    public Transform m_ReferenceTransform = null;

    private Rigidbody m_Rigidbody;

    private Vector2 m_MoveInput = Vector2.zero;
    private Vector2 m_LookInput = Vector2.zero;

    private bool m_CleanMoving = false;
    private bool m_CleanLooking = false;

#if USE_LEGACY_INPUT
    List<UnityEngine.XR.InputDevice> mLeftInputDevices;
    List<UnityEngine.XR.InputDevice> mRightInputDevices;
#endif
    // Start is called before the first frame update
    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();

#if USE_LEGACY_INPUT
        //XR
        mLeftInputDevices = new List<UnityEngine.XR.InputDevice>();
        UnityEngine.XR.InputDevices.GetDevicesAtXRNode(UnityEngine.XR.XRNode.LeftHand, mLeftInputDevices);

        Debug.Log($"{mLeftInputDevices.Count} left devices discovered !");

        mRightInputDevices = new List<UnityEngine.XR.InputDevice>();
        UnityEngine.XR.InputDevices.GetDevicesAtXRNode(UnityEngine.XR.XRNode.RightHand, mRightInputDevices);

        Debug.Log($"{mRightInputDevices.Count} right devices discovered !");
#endif
    }

    // Update is called once per frame
    void Update()
    {
#if USE_LEGACY_INPUT
        if (mLeftInputDevices.Count > 0)
        {
            mLeftInputDevices[0].TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxis, out m_MovingInput);
        }

        if(mRightInputDevices.Count > 0)
        {
            mRightInputDevices[0].TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxis, out m_LookInput);
        }
#endif

        moveUsingInputs();
    }

    private void moveUsingInputs()
    {
        float lMoveDistance = m_Speed * Time.deltaTime;
        Vector3 lMoveVector = new Vector3();

        if (m_FPSMoving)
        {
            lMoveVector.z = m_MoveInput.y*lMoveDistance;
            lMoveVector.x = m_MoveInput.x*lMoveDistance;

            transform.Rotate(Vector3.up, m_LookInput.x * m_AngularSpeed * Time.deltaTime, Space.Self);
            transform.Rotate(Vector3.right, -m_LookInput.y * m_AngularSpeed * Time.deltaTime, Space.Self);
        }
        else
        {
            lMoveVector.z = m_MoveInput.y * lMoveDistance;
            lMoveVector.x = m_MoveInput.x * lMoveDistance;
        }

        Vector3 lFinalMoveVector;// = transform.TransformDirection(lMoveVector) * lMoveDistance;

        if (m_ReferenceTransform != null )
        {
            //float lMagnitude = lMoveVector.magnitude;
            //Vector3 lDirectionVector = m_ReferenceTransform.forward;
            //lDirectionVector.y = 0;
            lFinalMoveVector = m_ReferenceTransform.forward * lMoveVector.z;// + m_ReferenceTransform.right * lMoveVector.x;
        }
        else
        {
            lFinalMoveVector = transform.TransformDirection(lMoveVector) * lMoveDistance;
        }


        //if(m_ImInVR)
        //{
        //    transform.Translate(lFinalMoveVector, Space.World);
        //}
        //else
        {
            m_Rigidbody.transform.Translate(lFinalMoveVector, Space.World);
        }


        if ( m_CleanMoving )
        {
            m_CleanMoving = false;
            m_MoveInput = Vector2.zero;
        }


        if ( m_CleanLooking )
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

        if (pContext.control.device.layout == "Keyboard")
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

}
