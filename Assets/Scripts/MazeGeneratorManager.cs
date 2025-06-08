using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[Serializable]
public class MazeGeneratorManager
{
    public Balyrinth.Utilities.LabyShape m_Shape = Balyrinth.Utilities.LabyShape.Rectangle;

    public int m_NumberOfColumns = 10;
    public int m_NumberOfRows = 10;
    public int m_NumberOfFloors = 10;

    public bool m_ProgressiveGeneration = false;

    public int m_AlgorithmIndex = 0;

    public int m_AlgorithmCorridorPseudoLength = 4;

    public float m_NumberOfConnectionsPerSeconds = 50f;

    //TODO: Create Solver Class
    Node mStartingNode;
    Node mEndingNode;

    bool mIsGenerationPerforming = false;

    Labyrinth mInternalRepresentation;
    MazeGenerator mMazeGenerator = null;
    List<Node> mLabyrinthPath = null;

    float m_TimeCounter = 0f;

    MazeGenerationListener mMazeGenerationListener = null;

    public MazeGeneratorManager()
    {
        
    }

    public void SetGenerationListener(MazeGenerationListener pGenerationListener)
    {
        mMazeGenerationListener = pGenerationListener;
    }

    public bool IsGenerationPerforming()
    {
        return mIsGenerationPerforming;
    }

    public void SetNumberOfColumns(string pNumberOfColumns)
    {
        int.TryParse(pNumberOfColumns, out m_NumberOfColumns);
    }

    public void SetNumberOfRows(string pNumberOfRows)
    {
        int.TryParse(pNumberOfRows, out m_NumberOfRows);
    }

    public void SetNumberOfFloors(string pNumberOfFloors)
    {
        int.TryParse(pNumberOfFloors, out m_NumberOfFloors);
    }

    public void SetShapeType(int pShapeType)
    {
        m_Shape = (Balyrinth.Utilities.LabyShape)pShapeType;
    }

    public void SetAlgorithm(int pAlgorithmIndex)
    {
        m_AlgorithmIndex = pAlgorithmIndex;
    }

    public void SetProgressiveGeneration(bool pProgressiveGeneration)
    {
        m_ProgressiveGeneration = pProgressiveGeneration;
    }

    public void SetCorridorPseudoLength(string pCorridorPseudoLength)
    {
        int.TryParse(pCorridorPseudoLength, out m_AlgorithmCorridorPseudoLength);
    }

    public void SetNumberOfConnectionsPerSecond(string pNumberOfConnectionsPerSecond)
    {
        float.TryParse(pNumberOfConnectionsPerSecond, out m_NumberOfConnectionsPerSeconds);
    }

    public bool[] GetConnectedDirections(int pNodeIndex)
    {
        int lNumDirections = mMazeGenerator.mShapeGenerator.NumberOfDirections;
        bool[] lReturnArray = new bool[lNumDirections];

        //TODO: use Linq
        for (int i = 0; i < lNumDirections; ++i)
        {
            lReturnArray[i] = mMazeGenerator.areConnected(pNodeIndex, mMazeGenerator.mShapeGenerator.getNextCellIndex(pNodeIndex, i));
        }

        return lReturnArray;
    }

    public int GetStartingNodeIndex()
    {
        return mStartingNode.mIndex;
    }

    public int GetEndingNodeIndex()
    {
        return mEndingNode.mIndex;
    }

    public int GetNumberOfNodes()
    {
        return mInternalRepresentation.mNodes.Count;
    }

    //TODO: managing 3D position should not be in this class
    public Vector3 GetObjectPosition(int pNodeIndex)
    {
        return mMazeGenerator.getPosition(pNodeIndex);
    }

    public void InitiateGeneration()
    {
        if ( mIsGenerationPerforming )
        {
            return;
        }

        mIsGenerationPerforming = true;

        m_TimeCounter = 0;

        switch (m_Shape)
        {
            case Balyrinth.Utilities.LabyShape.Rectangle:
                {
                    ShapeGeneratorInterface lShapeGeneratorInterface = new SquareShapeGenerator(m_NumberOfColumns, m_NumberOfRows);
                    mInternalRepresentation = lShapeGeneratorInterface.generate(m_NumberOfColumns, m_NumberOfRows);
                }
                break;
            case Balyrinth.Utilities.LabyShape.HoneyComb:
                {
                    ShapeGeneratorInterface lShapeGeneratorInterface = new HoneycombShapeGenerator(m_NumberOfColumns, m_NumberOfRows);
                    mInternalRepresentation = lShapeGeneratorInterface.generate(m_NumberOfColumns, m_NumberOfRows);
                }
                break;
            case Balyrinth.Utilities.LabyShape.Octogonized:
                {
                    ShapeGeneratorInterface lShapeGeneratorInterface = new OctoShapeGenerator(m_NumberOfColumns, m_NumberOfRows);
                    mInternalRepresentation = lShapeGeneratorInterface.generate(m_NumberOfColumns, m_NumberOfRows);
                }
                break;
            case Balyrinth.Utilities.LabyShape.Hypermaze:
                {
                    ShapeGeneratorInterface lShapeGeneratorInterface = new HyperMazeShapeGenerator(m_NumberOfColumns, m_NumberOfRows, m_NumberOfFloors);
                    mInternalRepresentation = lShapeGeneratorInterface.generate(m_NumberOfColumns, m_NumberOfRows, m_NumberOfFloors);
                }
                break;
            case Balyrinth.Utilities.LabyShape.Sphere:
                break;
            case Balyrinth.Utilities.LabyShape.Torus:
                break;
        }

        mMazeGenerator = new MazeGenerator(mInternalRepresentation, m_Shape, m_NumberOfColumns, m_NumberOfRows, m_NumberOfFloors);

        if (!m_ProgressiveGeneration)
        {
            mMazeGenerator.generate(m_AlgorithmIndex, m_AlgorithmCorridorPseudoLength);

            mInternalRepresentation.findMaximumPathExtremities(out mStartingNode, out mEndingNode);
            mInternalRepresentation.findMaximumCompletePath(mStartingNode, mEndingNode, out mLabyrinthPath);
            mMazeGenerationListener?.GenerationDone();

            mIsGenerationPerforming = false;
        }
    }

    public int[] GetActiveNodes()
    {
        return mInternalRepresentation.mNodes.Select(lItem => lItem.mIndex).Where(lValue => lValue > -1).ToArray();
    }

    public int[] GetCompletePath()
    {
        return mLabyrinthPath.Select(lItem => lItem.mIndex).ToArray();
    }

    /// <summary>
    /// Update generations steps according generation parameters
    /// </summary>
    /// <returns> Indices of Created/Updated Rooms </returns>
    public int[] UpdateGeneration()
    {
        HashSet<int> lModifiedNodeIndices = new HashSet<int>();
        m_TimeCounter += Time.deltaTime;

        float lTimeBetweenUpdates = (1f / m_NumberOfConnectionsPerSeconds);

        if (m_ProgressiveGeneration && mIsGenerationPerforming)
        {
            while (m_TimeCounter > lTimeBetweenUpdates && mIsGenerationPerforming)
            {
                m_TimeCounter -= lTimeBetweenUpdates;

                int lNodeIndex1 = -1;
                int lNodeIndex2 = -1;

                if (m_AlgorithmIndex == 0)
                {
                    mIsGenerationPerforming = !mMazeGenerator.generateStep(ref lNodeIndex1, ref lNodeIndex2);
                }
                else if (m_AlgorithmIndex == 1)
                {
                    mIsGenerationPerforming = !mMazeGenerator.generateStep2(ref lNodeIndex1, ref lNodeIndex2);
                }
                else if (m_AlgorithmIndex == 2)
                {
                    mIsGenerationPerforming = !mMazeGenerator.generateStep3(ref lNodeIndex1, ref lNodeIndex2);
                }
                else if (m_AlgorithmIndex == 3)
                {
                    mIsGenerationPerforming = !mMazeGenerator.generateStep4(ref lNodeIndex1, ref lNodeIndex2, m_AlgorithmCorridorPseudoLength);
                }

                lModifiedNodeIndices.Add(lNodeIndex1);
                lModifiedNodeIndices.Add(lNodeIndex2);
            }

            if (!mIsGenerationPerforming)
            {
                mInternalRepresentation.findMaximumPathExtremities(out mStartingNode, out mEndingNode);
                mInternalRepresentation.findMaximumCompletePath(mStartingNode, mEndingNode, out mLabyrinthPath);
                mMazeGenerationListener?.GenerationDone();
            }
        }

        return lModifiedNodeIndices.ToArray();
    }

    internal MazeGenerator GetMazeGenerator()
    {
        return mMazeGenerator;
    }
}

