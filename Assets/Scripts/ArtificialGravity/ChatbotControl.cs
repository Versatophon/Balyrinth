using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ChatbotControl : MonoBehaviour
{
    //public MazeGeneratorManager m_MazeGeneratorManager = null;
    public LabyrinthGenerator m_LabyrinthGenerator = null;
    public ArtificialGravity m_ArtificialGravity = null;

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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(mAnimatingRotation)
        {
            mCurrentRotation += Time.deltaTime * mRotationFeedRate * mRotationFeedDirection;
            Debug.Log($"{mCurrentRotation} - {mExpectedRotation}");

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

            transform.localRotation = Quaternion.Euler(0, mCurrentRotation - mRotationGlobalOffset, 0);
        }
        else
        {
            mCurrentRotation = m_LabyrinthGenerator.m_MazeGeneratorManager.GetMazeGenerator().mShapeGenerator.getDirectionAngle(mCurrentDirection);
            transform.localRotation = Quaternion.Euler(0, mCurrentRotation - mRotationGlobalOffset, 0);
        }

        if(mAnimatingPosition)
        {
            if(m_ArtificialGravity != null)
            {
                Vector3 lDeltaPosition = (mExpectedPosition - transform.position);
                if (lDeltaPosition.magnitude < mTranslationFeedRate * Time.deltaTime)
                {
                    m_ArtificialGravity.transform.position = mExpectedPosition;
                    mAnimatingPosition = false;
                }
                else
                {
                    Vector3 lDirection = lDeltaPosition.normalized;
                    m_ArtificialGravity.transform.position += lDirection * mTranslationFeedRate * Time.deltaTime;
                }
            }
            else
            {
                mAnimatingPosition = false;
            }
        }
    }

    public void TurnLeft(UnityEngine.InputSystem.InputAction.CallbackContext pContext)
    {
        if (mAnimatingPosition || mAnimatingRotation || (pContext.phase != InputActionPhase.Started))
        {
            return;
        }

        if(m_LabyrinthGenerator != null)
        {
            mExpectedRotation = m_LabyrinthGenerator.m_MazeGeneratorManager.GetMazeGenerator().mShapeGenerator.getDirectionAngle(mCurrentDirection) + m_LabyrinthGenerator.m_MazeGeneratorManager.GetMazeGenerator().mShapeGenerator.StepOffset;
            mCurrentDirection += 1;
            if(mCurrentDirection >= m_LabyrinthGenerator.m_MazeGeneratorManager.GetMazeGenerator().mShapeGenerator.NumberOfDirections || mCurrentDirection < 0)
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

        if (m_LabyrinthGenerator != null)
        {
            mExpectedRotation = m_LabyrinthGenerator.m_MazeGeneratorManager.GetMazeGenerator().mShapeGenerator.getDirectionAngle(mCurrentDirection) - m_LabyrinthGenerator.m_MazeGeneratorManager.GetMazeGenerator().mShapeGenerator.StepOffset;
            mCurrentDirection -= 1;
            if (mCurrentDirection >= m_LabyrinthGenerator.m_MazeGeneratorManager.GetMazeGenerator().mShapeGenerator.NumberOfDirections || mCurrentDirection < 0)
            {
                mCurrentDirection = m_LabyrinthGenerator.m_MazeGeneratorManager.GetMazeGenerator().mShapeGenerator.NumberOfDirections -1;
            }
            //mExpectedRotation = m_LabyrinthGenerator.m_MazeGeneratorManager.GetMazeGenerator().mShapeGenerator.getDirectionAngle(mCurrentDirection);
            mRotationFeedDirection = 1f;
            mAnimatingRotation = true;
            //mCurrentDirection = mCurrentDirection % m_MazeGeneratorManager.GetMazeGenerator().mShapeGenerator.NumberOfDirections;
        }
    }

    public void MoveForward(UnityEngine.InputSystem.InputAction.CallbackContext pContext)
    {
        if (mAnimatingPosition || mAnimatingRotation || (pContext.phase != InputActionPhase.Started))
        {
            return;
        }

        int lCurrenctCellIndex = m_LabyrinthGenerator.m_MazeGeneratorManager.GetMazeGenerator().mShapeGenerator.getRoomIndex(transform.position);
        int lNextCellIndex = m_LabyrinthGenerator.m_MazeGeneratorManager.GetMazeGenerator().mShapeGenerator.getNextCellIndex(lCurrenctCellIndex, mCurrentDirection);
        if ( m_LabyrinthGenerator.m_MazeGeneratorManager.GetMazeGenerator().areConnected(lCurrenctCellIndex, lNextCellIndex) )
        {
            mExpectedPosition = m_LabyrinthGenerator.m_MazeGeneratorManager.GetMazeGenerator().getPosition(lNextCellIndex) + m_LabyrinthGenerator.m_SpawnPointsOffset;
            mAnimatingPosition = true;
        }
    }

    public void MoveBackward(UnityEngine.InputSystem.InputAction.CallbackContext pContext)
    {
        if (mAnimatingPosition || mAnimatingRotation || (pContext.phase != InputActionPhase.Started))
        {
            return;
        }
        int lCurrenctCellIndex = m_LabyrinthGenerator.m_MazeGeneratorManager.GetMazeGenerator().mShapeGenerator.getRoomIndex(transform.position);
        int lNextCellIndex = m_LabyrinthGenerator.m_MazeGeneratorManager.GetMazeGenerator().mShapeGenerator.getNextCellIndex(lCurrenctCellIndex, m_LabyrinthGenerator.m_MazeGeneratorManager.GetMazeGenerator().mShapeGenerator.getOppositeDirection(mCurrentDirection));
        if (m_LabyrinthGenerator.m_MazeGeneratorManager.GetMazeGenerator().areConnected(lCurrenctCellIndex, lNextCellIndex))
        {
            mExpectedPosition = m_LabyrinthGenerator.m_MazeGeneratorManager.GetMazeGenerator().getPosition(lNextCellIndex) + m_LabyrinthGenerator.m_SpawnPointsOffset;
            mAnimatingPosition = true;
        }

    }
}
