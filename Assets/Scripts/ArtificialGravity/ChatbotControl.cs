using System.Collections;
using System.Collections.Generic;
using TMPro;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Text;

public class ChatbotControl : MonoBehaviour
{
    //public MazeGeneratorManager m_MazeGeneratorManager = null;
    public LabyrinthGenerator m_LabyrinthGenerator = null;
    public ArtificialGravity m_ArtificialGravity = null;

    public Transform m_FrontDirectionObject = null;
    public Transform m_BackDirectionObject = null;

    private int mCurrentDirection = 0;

    private bool mAnimatingRotation = false;
    private bool mAnimatingPosition = false;

    private float mRotationFeedDirection = 1f;
    private float mRotationFeedRate = 90f;

    private float mRotationGlobalOffset = -90f;

    private float mExpectedRotation = 0f;
    private float mCurrentRotation = 0f;

    private Vector3 mExpectedPosition = Vector3.zero;
    private float mTranslationFeedRate = 1.5f;

    //Socket
    Socket mUdp = null;

    void InitServer()
    {
        //IPHostEntry lHost = Dns.Resolve(Dns.GetHostName());
        //IPAddress lIp = lHost.AddressList[0];
        IPEndPoint lEndPoint = new IPEndPoint(IPAddress.Parse("192.168.1.41"), 9042);

        mUdp = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        mUdp.Bind(lEndPoint);

        Debug.Log("Udp Server started");
    }

    void UninitServer()
    {
        if (mUdp != null)
        {
            mUdp.Dispose();
            mUdp = null;
            Debug.Log("Udp Server stoped");
        }
    }

    void UpdateFromUdp()
    {
        if (mUdp != null && mUdp.Available != 0)
        {
            byte[] packet = new byte[32];
            EndPoint sender = new IPEndPoint(IPAddress.Any, 9042);

            int rec = mUdp.ReceiveFrom(packet, ref sender);
            string direction = Encoding.Default.GetString(packet).TrimEnd('\0');

            switch (direction)
            {
                case "UP":
                    InternalMoveForward();
                    break;
                case "DN":
                    InternalMoveBackward();
                    break;
                case "LT":
                    InternalTurnLeft();
                    break;
                case "RT":
                    InternalTurnRight();
                    break;
                default:
                    Debug.Log($"Receive {direction}");
                    break;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    private void OnEnable()
    {
        InitServer();
    }

    private void OnDisable()
    {
        UninitServer();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateFromUdp();

        if (mAnimatingRotation)
        {
            mCurrentRotation += Time.deltaTime * mRotationFeedRate * mRotationFeedDirection;
            //Debug.Log($"{mCurrentRotation} - {mExpectedRotation}");

            if(mRotationFeedDirection > 0f && mCurrentRotation > mExpectedRotation)
            {
                mCurrentRotation = mExpectedRotation;
                mAnimatingRotation = false;

                mCurrentRotation = Mathf.Repeat(mCurrentRotation, 360f);
            }
            else if (mRotationFeedDirection < 0f && mCurrentRotation < mExpectedRotation)
            {
                mCurrentRotation = mExpectedRotation;
                mAnimatingRotation = false;

                mCurrentRotation = Mathf.Repeat(mCurrentRotation, 360f);
            }

            transform.localRotation = Quaternion.Euler(0, -mCurrentRotation - mRotationGlobalOffset, 0);
        }
        else
        {
            mCurrentRotation = m_LabyrinthGenerator.m_MazeGeneratorManager.GetMazeGenerator().mShapeGenerator.getDirectionAngle(mCurrentDirection);
            transform.localRotation = Quaternion.Euler(0, -mCurrentRotation - mRotationGlobalOffset, 0);
        }

        if(mAnimatingPosition)
        {
            if(m_ArtificialGravity != null)
            {
                Vector3 lDeltaPosition = (mExpectedPosition - m_ArtificialGravity.transform.position);
                if (lDeltaPosition.magnitude < mTranslationFeedRate * Time.deltaTime)
                {
                    m_ArtificialGravity.transform.position = mExpectedPosition;
                    mAnimatingPosition = false;
                    //Debug.Log($"Ending {m_ArtificialGravity.transform.position} - {mExpectedPosition}");
                }
                else
                {
                    Vector3 lDirection = lDeltaPosition.normalized;
                    Vector3 lPosition = m_ArtificialGravity.transform.position + lDirection * mTranslationFeedRate * Time.deltaTime;
                    //HACK: Weird behaviour needed to compensate gravity
                    lPosition.y = 0;
                    m_ArtificialGravity.transform.position = lPosition;
                    //Debug.Log($"Running {m_ArtificialGravity.transform.position} - {mExpectedPosition}");
                }
            }
            else
            {
                mAnimatingPosition = false;
            }
        }

        SetForwardAndBackwardDirections();
    }

    void SetForwardAndBackwardDirections()
    {
        if (m_FrontDirectionObject != null && m_BackDirectionObject != null)
        {
            int lCurrenctCellIndex = m_LabyrinthGenerator.m_MazeGeneratorManager.GetMazeGenerator().mShapeGenerator.getRoomIndex(transform.position);
            int lForwardCellIndex = m_LabyrinthGenerator.m_MazeGeneratorManager.GetMazeGenerator().mShapeGenerator.getNextCellIndex(lCurrenctCellIndex, mCurrentDirection);
            int lBackwardCellIndex = m_LabyrinthGenerator.m_MazeGeneratorManager.GetMazeGenerator().mShapeGenerator.getNextCellIndex(lCurrenctCellIndex, m_LabyrinthGenerator.m_MazeGeneratorManager.GetMazeGenerator().mShapeGenerator.getOppositeDirection(mCurrentDirection));

            if (lForwardCellIndex != -1 )
            {
                m_FrontDirectionObject.position = m_LabyrinthGenerator.m_MazeGeneratorManager.GetMazeGenerator().mShapeGenerator.getRoomPosition(lForwardCellIndex);
            }
            else
            {
                m_FrontDirectionObject.position = new Vector3(-1000, -1000, 0);
            }

            if (lBackwardCellIndex != -1 )
            {
                m_BackDirectionObject.position = m_LabyrinthGenerator.m_MazeGeneratorManager.GetMazeGenerator().mShapeGenerator.getRoomPosition(lBackwardCellIndex);
            }
            else
            {
                m_BackDirectionObject.position = new Vector3(-1000, -1000, 0);
            }

            //Debug.Log($"{mCurrentRotation} degrees !");
        }
    }

    private void InternalTurnRight()
    {
        if (mAnimatingPosition || mAnimatingRotation )
        {
            return;
        }

        if (m_LabyrinthGenerator != null)
        {
            mExpectedRotation = m_LabyrinthGenerator.m_MazeGeneratorManager.GetMazeGenerator().mShapeGenerator.getDirectionAngle(mCurrentDirection) + m_LabyrinthGenerator.m_MazeGeneratorManager.GetMazeGenerator().mShapeGenerator.StepOffset;
            mCurrentDirection += 1;
            if (mCurrentDirection >= m_LabyrinthGenerator.m_MazeGeneratorManager.GetMazeGenerator().mShapeGenerator.NumberOfDirections || mCurrentDirection < 0)
            {
                mCurrentDirection = 0;
            }
            mRotationFeedDirection = -1f;
            mAnimatingRotation = true;
            //mCurrentDirection = mCurrentDirection % m_MazeGeneratorManager.GetMazeGenerator().mShapeGenerator.NumberOfDirections;
        }
    }

    public void TurnRight(UnityEngine.InputSystem.InputAction.CallbackContext pContext)
    {
        if (mAnimatingPosition || mAnimatingRotation || (pContext.phase != InputActionPhase.Started))
        {
            return;
        }

        InternalTurnRight();
        //if (m_LabyrinthGenerator != null)
        //{
        //    mExpectedRotation = m_LabyrinthGenerator.m_MazeGeneratorManager.GetMazeGenerator().mShapeGenerator.getDirectionAngle(mCurrentDirection) + m_LabyrinthGenerator.m_MazeGeneratorManager.GetMazeGenerator().mShapeGenerator.StepOffset;
        //    mCurrentDirection += 1;
        //    if (mCurrentDirection >= m_LabyrinthGenerator.m_MazeGeneratorManager.GetMazeGenerator().mShapeGenerator.NumberOfDirections || mCurrentDirection < 0)
        //    {
        //        mCurrentDirection = 0;
        //    }
        //    mRotationFeedDirection = -1f;
        //    mAnimatingRotation = true;
        //    //mCurrentDirection = mCurrentDirection % m_MazeGeneratorManager.GetMazeGenerator().mShapeGenerator.NumberOfDirections;
        //}
    }


    private void InternalTurnLeft()
    {
        if (mAnimatingPosition || mAnimatingRotation)
        {
            return;
        }

        if (m_LabyrinthGenerator != null)
        {
            mExpectedRotation = m_LabyrinthGenerator.m_MazeGeneratorManager.GetMazeGenerator().mShapeGenerator.getDirectionAngle(mCurrentDirection) - m_LabyrinthGenerator.m_MazeGeneratorManager.GetMazeGenerator().mShapeGenerator.StepOffset;
            //mExpectedRotation = m_LabyrinthGenerator.m_MazeGeneratorManager.GetMazeGenerator().mShapeGenerator.getDirectionAngle(mCurrentDirection);
            mCurrentDirection -= 1;
            if (mCurrentDirection >= m_LabyrinthGenerator.m_MazeGeneratorManager.GetMazeGenerator().mShapeGenerator.NumberOfDirections || mCurrentDirection < 0)
            {
                mCurrentDirection = m_LabyrinthGenerator.m_MazeGeneratorManager.GetMazeGenerator().mShapeGenerator.NumberOfDirections - 1;
            }
            mRotationFeedDirection = 1f;
            mAnimatingRotation = true;
            //mCurrentDirection = mCurrentDirection % m_MazeGeneratorManager.GetMazeGenerator().mShapeGenerator.NumberOfDirections;
        }
    }
    public void TurnLeft(UnityEngine.InputSystem.InputAction.CallbackContext pContext)
    {
        if (mAnimatingPosition || mAnimatingRotation || (pContext.phase != InputActionPhase.Started))
        {
            return;
        }

        InternalTurnLeft();
        //if (m_LabyrinthGenerator != null)
        //{
        //    mExpectedRotation = m_LabyrinthGenerator.m_MazeGeneratorManager.GetMazeGenerator().mShapeGenerator.getDirectionAngle(mCurrentDirection) - m_LabyrinthGenerator.m_MazeGeneratorManager.GetMazeGenerator().mShapeGenerator.StepOffset;
        //    //mExpectedRotation = m_LabyrinthGenerator.m_MazeGeneratorManager.GetMazeGenerator().mShapeGenerator.getDirectionAngle(mCurrentDirection);
        //    mCurrentDirection -= 1;
        //    if(mCurrentDirection >= m_LabyrinthGenerator.m_MazeGeneratorManager.GetMazeGenerator().mShapeGenerator.NumberOfDirections || mCurrentDirection < 0)
        //    {
        //        mCurrentDirection = m_LabyrinthGenerator.m_MazeGeneratorManager.GetMazeGenerator().mShapeGenerator.NumberOfDirections -1;
        //    }
        //    mRotationFeedDirection = 1f;
        //    mAnimatingRotation = true;
        //    //mCurrentDirection = mCurrentDirection % m_MazeGeneratorManager.GetMazeGenerator().mShapeGenerator.NumberOfDirections;
        //}
    }

    private void InternalMoveForward()
    {
        if (mAnimatingPosition || mAnimatingRotation)
        {
            return;
        }

        int lCurrenctCellIndex = m_LabyrinthGenerator.m_MazeGeneratorManager.GetMazeGenerator().mShapeGenerator.getRoomIndex(transform.position);
        int lNextCellIndex = m_LabyrinthGenerator.m_MazeGeneratorManager.GetMazeGenerator().mShapeGenerator.getNextCellIndex(lCurrenctCellIndex, mCurrentDirection);
        if (m_LabyrinthGenerator.m_MazeGeneratorManager.GetMazeGenerator().areConnected(lCurrenctCellIndex, lNextCellIndex))
        {
            mExpectedPosition = m_LabyrinthGenerator.m_MazeGeneratorManager.GetMazeGenerator().getPosition(lNextCellIndex);// + m_LabyrinthGenerator.m_SpawnPointsOffset;
            mAnimatingPosition = true;
        }
    }

    public void MoveForward(UnityEngine.InputSystem.InputAction.CallbackContext pContext)
    {
        if (mAnimatingPosition || mAnimatingRotation || (pContext.phase != InputActionPhase.Started))
        {
            return;
        }

        InternalMoveForward();

        //int lCurrenctCellIndex = m_LabyrinthGenerator.m_MazeGeneratorManager.GetMazeGenerator().mShapeGenerator.getRoomIndex(transform.position);
        //int lNextCellIndex = m_LabyrinthGenerator.m_MazeGeneratorManager.GetMazeGenerator().mShapeGenerator.getNextCellIndex(lCurrenctCellIndex, mCurrentDirection);
        //if ( m_LabyrinthGenerator.m_MazeGeneratorManager.GetMazeGenerator().areConnected(lCurrenctCellIndex, lNextCellIndex) )
        //{
        //    mExpectedPosition = m_LabyrinthGenerator.m_MazeGeneratorManager.GetMazeGenerator().getPosition(lNextCellIndex);// + m_LabyrinthGenerator.m_SpawnPointsOffset;
        //    mAnimatingPosition = true;
        //}
    }

    private void InternalMoveBackward()
    {
        if (mAnimatingPosition || mAnimatingRotation)
        {
            return;
        }

        int lCurrenctCellIndex = m_LabyrinthGenerator.m_MazeGeneratorManager.GetMazeGenerator().mShapeGenerator.getRoomIndex(transform.position);
        int lNextCellIndex = m_LabyrinthGenerator.m_MazeGeneratorManager.GetMazeGenerator().mShapeGenerator.getNextCellIndex(lCurrenctCellIndex, m_LabyrinthGenerator.m_MazeGeneratorManager.GetMazeGenerator().mShapeGenerator.getOppositeDirection(mCurrentDirection));

        if (m_LabyrinthGenerator.m_MazeGeneratorManager.GetMazeGenerator().areConnected(lCurrenctCellIndex, lNextCellIndex))
        {
            mExpectedPosition = m_LabyrinthGenerator.m_MazeGeneratorManager.GetMazeGenerator().getPosition(lNextCellIndex);// + m_LabyrinthGenerator.m_SpawnPointsOffset;
            mAnimatingPosition = true;
        }
    }

    public void MoveBackward(UnityEngine.InputSystem.InputAction.CallbackContext pContext)
    {
        if (mAnimatingPosition || mAnimatingRotation || (pContext.phase != InputActionPhase.Started))
        {
            return;
        }

        InternalMoveBackward();
        //int lCurrenctCellIndex = m_LabyrinthGenerator.m_MazeGeneratorManager.GetMazeGenerator().mShapeGenerator.getRoomIndex(transform.position);
        //int lNextCellIndex = m_LabyrinthGenerator.m_MazeGeneratorManager.GetMazeGenerator().mShapeGenerator.getNextCellIndex(lCurrenctCellIndex, m_LabyrinthGenerator.m_MazeGeneratorManager.GetMazeGenerator().mShapeGenerator.getOppositeDirection(mCurrentDirection));
        //
        //if (m_LabyrinthGenerator.m_MazeGeneratorManager.GetMazeGenerator().areConnected(lCurrenctCellIndex, lNextCellIndex))
        //{
        //    mExpectedPosition = m_LabyrinthGenerator.m_MazeGeneratorManager.GetMazeGenerator().getPosition(lNextCellIndex);// + m_LabyrinthGenerator.m_SpawnPointsOffset;
        //    mAnimatingPosition = true;
        //}

    }

    public void StopAnimations()
    {
        mAnimatingRotation = false;
        mAnimatingPosition = false;
    }
}
