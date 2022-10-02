using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtificialGravity : MonoBehaviour
{
    public Vector3 m_LocalDirectionSeekingForGravity = Vector3.down;

    public float m_AccelerationConstant = 9.81f;
    public bool m_CalculateGravityVectorFromGroundSurface = true;

    protected Vector3 m_MovingVector = Vector3.zero;
    protected Vector3 m_LastContactPoint = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        m_LastContactPoint = transform.position + m_LocalDirectionSeekingForGravity;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Teleport(Vector3 pPosition, Vector3 pGravity)
    {
        transform.position = pPosition;
        m_LocalDirectionSeekingForGravity = pGravity;
        m_LastContactPoint = transform.position + m_LocalDirectionSeekingForGravity;
    }

    private void FixedUpdate()
    {
        Vector3 lDirection = transform.TransformDirection(m_LocalDirectionSeekingForGravity);
        
        RaycastHit lHIt;

        float lRetractRecastDistance = 0.1f;

        if (m_CalculateGravityVectorFromGroundSurface)
        {
            if (Physics.Raycast(transform.position - lRetractRecastDistance * lDirection, lDirection, out lHIt, 1.0f, 0b10000000))
            {
                m_LastContactPoint = lHIt.point;

                {//Adjust body orientation

                    Quaternion lCompensationRotation = Quaternion.FromToRotation(lDirection, -lHIt.normal);
                    transform.rotation = lCompensationRotation * transform.rotation;
                }

                GetComponent<Rigidbody>().AddForce((-lHIt.normal) * m_AccelerationConstant, ForceMode.Acceleration);

                if (m_MovingVector.sqrMagnitude > 0.01f && lHIt.distance < (lRetractRecastDistance * 1.05f))
                {
                    GetComponent<Rigidbody>().AddForce(m_MovingVector * GetComponent<Rigidbody>().mass, ForceMode.Impulse);
                }
            }
            else
            {
                if (true)
                {//Adjust body orientation
                    Vector3 lCompensatedDirection = m_LastContactPoint - transform.position;
                    Quaternion lCompensationRotation = Quaternion.FromToRotation(lDirection, lCompensatedDirection);
                    transform.rotation = lCompensationRotation * transform.rotation;
                    GetComponent<Rigidbody>().AddForce(lCompensatedDirection * m_AccelerationConstant, ForceMode.Acceleration);
                }
            }
        }
        else
        {
            //Set Gravity
            GetComponent<Rigidbody>().AddForce(m_LocalDirectionSeekingForGravity * m_AccelerationConstant, ForceMode.Acceleration);
            //Set Moving
            GetComponent<Rigidbody>().AddForce(m_MovingVector * GetComponent<Rigidbody>().mass, ForceMode.Impulse);
        }
    }

    public void SetMovingVector(Vector3 pMovingVector)
    {
        m_MovingVector = transform.TransformDirection(pMovingVector);
    }
}
