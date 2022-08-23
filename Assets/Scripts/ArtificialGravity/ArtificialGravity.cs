using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtificialGravity : MonoBehaviour
{
    public Vector3 m_LocalDirectionSeekingForGravity = Vector3.down;

    public float m_AccelerationConstant = 9.81f;

    protected Vector3 m_MovingVector = Vector3.zero;
    protected Vector3 m_LastContactPoint = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        Vector3 lDirection = transform.TransformDirection(m_LocalDirectionSeekingForGravity);
        
        RaycastHit lHIt;

        float lRetractRecastDistance = 0.1f;
        if ( Physics.Raycast(transform.position - lRetractRecastDistance * lDirection, lDirection, out lHIt, 1.0f, 0b10000000) )
        {
            m_LastContactPoint = lHIt.point;
            //Debug.Log($"hit at {lHIt.point} with normal {lHIt.normal} on {lHIt.transform.name}");
            //Vector3 lGlobalforceApplied = (-lHIt.normal) * GetComponent<Rigidbody>().mass;

            {//Adjust body orientation

                Quaternion lCompensationRotation =  Quaternion.FromToRotation(lDirection, -lHIt.normal);
                //transform.rotation = Quaternion.Lerp(Quaternion.identity, lCompensationRotation, 0.1f) * transform.rotation;
                transform.rotation = lCompensationRotation * transform.rotation;
            }

            GetComponent<Rigidbody>().AddForce((-lHIt.normal) * m_AccelerationConstant, ForceMode.Acceleration);

            //Debug.Log($"{lHIt.distance}");

            if (m_MovingVector.sqrMagnitude > 0.01f && lHIt.distance < (lRetractRecastDistance*1.05f))
            {
                //lGlobalforceApplied += m_MovingDirection * GetComponent<Rigidbody>().mass;
                GetComponent<Rigidbody>().AddForce(m_MovingVector * GetComponent<Rigidbody>().mass, ForceMode.Impulse);
                //transform.position += m_MovingVector * Time.fixedDeltaTime;
            }
            //GetComponent<Rigidbody>().AddForce(lGlobalforceApplied, ForceMode.Impulse);

            //Make player flying in order to fluidify movements
            {
                //GetComponent<Rigidbody>().AddForce(transform.rotation * Vector3.up, ForceMode.Acceleration);
            }
        }
        else
        {
            if ( true )
            {//Adjust body orientation
                Vector3 lCompensatedDirection = m_LastContactPoint - transform.position;
                Quaternion lCompensationRotation = Quaternion.FromToRotation(lDirection, lCompensatedDirection);
                transform.rotation = lCompensationRotation * transform.rotation;
                GetComponent<Rigidbody>().AddForce(lCompensatedDirection * m_AccelerationConstant, ForceMode.Acceleration);
            }
        }
    }

    public void SetMovingVector(Vector3 pMovingVector)
    {
        m_MovingVector = transform.TransformDirection(pMovingVector);
    }
}
