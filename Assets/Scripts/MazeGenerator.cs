
using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class MazeGenerator
{
#if false
    public enum CardinalDirection
    {
        West,
        North,
        East,
        South,
        Last,
    }
#endif

    System.Random mRandomGenerator = new System.Random();

    int mCurrentColor = 0;

    List<Node> mPotentialConnections = new List<Node>();
    List<Node> mStillConnectablesNodes;

    Labyrinth mLabyrinth;

    Balyrinth.Utilities.LabyShape mShape;
    public ShapeGeneratorInterface mShapeGenerator;

    Node mLastConnectedNode = null;
    int mNumConnectionsPerformed = 0;

    int mCurrentDirection = 0;// CardinalDirection.East;

    public MazeGenerator(Labyrinth pLaby, Balyrinth.Utilities.LabyShape pShape, int pWidth, int pHeight)
    {
        mShape = pShape;

        switch (mShape)
        {
            case Balyrinth.Utilities.LabyShape.Rectangle:
                mShapeGenerator = new SquareShapeGenerator(pWidth, pHeight);
                break;
            case Balyrinth.Utilities.LabyShape.HoneyComb:
                mShapeGenerator = new HoneycombShapeGenerator(pWidth, pHeight);
                break;
        }

        mLabyrinth = pLaby;
        mStillConnectablesNodes = new List<Node>(pLaby.mNodes);

        mCurrentDirection = mRandomGenerator.Next(mShapeGenerator.GetNumberOfDirections());
        //mCurrentDirection = mRandomGenerator.Next(4);
    }

    public Node getNode(int pIndex)
    {
        return mLabyrinth.mNodes[pIndex];
    }

    public Vector3 getPosition(int pIndex)
    {
        return mShapeGenerator.getRoomPosition(pIndex);
    }

    public bool areConnected(int pRoomIndex1, int pRoomIndex2)
    {
        if (pRoomIndex1 < 0 || pRoomIndex1 >= mLabyrinth.mNodes.Count || pRoomIndex2 < 0 || pRoomIndex2 >= mLabyrinth.mNodes.Count)
        {
            return false;
        }
        return mLabyrinth.mNodes[pRoomIndex1].mConnectedNeighbours.Contains(mLabyrinth.mNodes[pRoomIndex2]);
    }

    public void generate(int pAlgorithmIndex, int pWidth, int pHeight, int pCorridorPseudoLength)
    {
        bool lIsDone = false;

        int lDummyIndex1 = -1;
        int lDummyIndex2 = -1;

        float lStartupTime = Time.realtimeSinceStartup;

        while (!lIsDone)
        {
            switch (pAlgorithmIndex)
            {
                default:
                case 0:
                    lIsDone = generateStep(ref lDummyIndex1, ref lDummyIndex2);
                    break;
                case 1:
                    lIsDone = generateStep2(ref lDummyIndex1, ref lDummyIndex2);
                    break;
                case 2:
                    lIsDone = generateStep3(ref lDummyIndex1, ref lDummyIndex2, pWidth, pHeight);
                    break;
                case 3:
                    lIsDone = generateStep4(ref lDummyIndex1, ref lDummyIndex2, pWidth, pHeight, pCorridorPseudoLength);
                    break;
            }
        }

        float lGenerationTime = Time.realtimeSinceStartup;

        Debug.Log((lGenerationTime - lStartupTime) + " s. for maze generation !");
    }


    //returns true when generation is complete
    public bool generateStep(ref int pNodeIndex1, ref int pNodeIndex2)
    {
        bool lIsDone = false;

        pNodeIndex1 = -1;
        pNodeIndex2 = -1;

        while (pNodeIndex1 == -1)
        {
            mPotentialConnections.Clear();

            //Debug.Log("Connectables Count " + mStillConnectablesNodes.Count);

            int lIndex = mRandomGenerator.Next(mStillConnectablesNodes.Count);

            Node lNode = mStillConnectablesNodes[lIndex];

            foreach (Node lPotentialNode in lNode.mPotentialNeighbours)
            {
                if (lPotentialNode != null)
                {
                    if (lPotentialNode.mColor == -1 || lPotentialNode.mColor != lNode.mColor)
                    {
                        mPotentialConnections.Add(lPotentialNode);
                    }
                }
            }
 

            if (mPotentialConnections.Count == 0)
            {
                mStillConnectablesNodes.RemoveAt(lIndex);
            }
            else
            {
                int lConnectionIndex = mRandomGenerator.Next(mPotentialConnections.Count);
                Node lPotentialNode = mPotentialConnections[lConnectionIndex];
                
                lNode.mConnectedNeighbours.Add(lPotentialNode);
                lPotentialNode.mConnectedNeighbours.Add(lNode);


                if (lNode.mColor == -1)
                {

                    if (lPotentialNode.mColor == -1)
                    {
                        lNode.mColor = mCurrentColor;
                        lPotentialNode.mColor = mCurrentColor;
                        mCurrentColor++;
                    }
                    else
                    {
                        lNode.mColor = lPotentialNode.mColor;
                    }
                }
                else
                {
                    if (lPotentialNode.mColor == -1)
                    {
                        lPotentialNode.mColor = lNode.mColor;
                    }
                    else
                    {
                        int lColorToPropagate = Math.Min(lNode.mColor, lPotentialNode.mColor);
                        
                        if ( lColorToPropagate == lPotentialNode.mColor )
                        {
                            lNode.propagateColor(lColorToPropagate);
                        }
                        else
                        {
                            lPotentialNode.propagateColor(lColorToPropagate);
                        }
                        
                    }
                }

                pNodeIndex1 = lNode.mIndex;
                pNodeIndex2 = lPotentialNode.mIndex;
            }


            lIsDone = !mLabyrinth.mNodes.Any(node => node.mColor != 0);

        }

        return lIsDone;
    }


    bool canNodeBeConnected(Node pNode)
    {
        foreach (Node lPotentialNode in pNode.mPotentialNeighbours)
        {
            if(lPotentialNode!=null && (lPotentialNode.mColor == -1 || lPotentialNode.mColor != pNode.mColor))
            {
                return true;
            }
        }

        return false;
    }


    public bool generateStep2(ref int pNodeIndex1, ref int pNodeIndex2)
    {
        //bool lIsDone = false;

        pNodeIndex1 = -1;
        pNodeIndex2 = -1;

        if ( mLastConnectedNode == null )
        {
            mLastConnectedNode = mLabyrinth.mNodes[mRandomGenerator.Next(mLabyrinth.mNodes.Count)];
            mLastConnectedNode.mColor = 0;
            mStillConnectablesNodes.Clear();
            mStillConnectablesNodes.Add(mLastConnectedNode);
        }

        bool lConnectionPerformed = false;

        while (!lConnectionPerformed)
        {

            mPotentialConnections.Clear();

            foreach (Node lPotentialNode in mLastConnectedNode.mPotentialNeighbours)
            {
                if (lPotentialNode!=null && lPotentialNode.mColor == -1)
                {
                    mPotentialConnections.Add(lPotentialNode);
                }
            }

            if (mPotentialConnections.Count == 0)
            {
                mStillConnectablesNodes.Remove(mLastConnectedNode);
                //find another candidate
                mLastConnectedNode = mStillConnectablesNodes[mRandomGenerator.Next(mStillConnectablesNodes.Count)];
            }
            else
            {
                Node lNewConnectedNode = mPotentialConnections[mRandomGenerator.Next(mPotentialConnections.Count)];
                
                pNodeIndex1 = mLastConnectedNode.mIndex;
                pNodeIndex2 = lNewConnectedNode.mIndex;

                mLastConnectedNode.mConnectedNeighbours.Add(lNewConnectedNode);
                lNewConnectedNode.mConnectedNeighbours.Add(mLastConnectedNode);

                if(mPotentialConnections.Count == 1) //remove if new connection filled previous node
                {
                    mStillConnectablesNodes.Remove(mLastConnectedNode);
                }

                lNewConnectedNode.mColor = 0;

                if(canNodeBeConnected(lNewConnectedNode))
                {
                    mLastConnectedNode = lNewConnectedNode;
                    mStillConnectablesNodes.Add(mLastConnectedNode);
                }
                else //remove if new connection leads to a dead end
                {
                    mLastConnectedNode = mStillConnectablesNodes[mRandomGenerator.Next(mStillConnectablesNodes.Count)];
                }

                mNumConnectionsPerformed++;
                lConnectionPerformed = true;
            }
        }

        return (mNumConnectionsPerformed == (mLabyrinth.mNodes.Count - 1));
    }


    private Node getConnectableNodeFromDirection(Node pCurrentNode, int pDirection, int pLabyrinthWidth, int pLabyrinthHeight)
    {

        int lNextNodeIndex = mShapeGenerator.getNextCellIndex(pCurrentNode.mIndex, pDirection);

        if( lNextNodeIndex != -1 && pCurrentNode.mPotentialNeighbours.Contains(mLabyrinth.mNodes[lNextNodeIndex]))
        {
            if (mLabyrinth.mNodes[lNextNodeIndex].mColor != pCurrentNode.mColor)
            {
                return mLabyrinth.mNodes[lNextNodeIndex];
            }
        }

#if false
        switch (pDirection)
        {
            case CardinalDirection.West:
                if((pCurrentNode.mIndex%pLabyrinthWidth) != 0 && pCurrentNode.mPotentialNeighbours.Contains(mLabyrinth.mNodes[pCurrentNode.mIndex-1]))
                {
                    if(mLabyrinth.mNodes[pCurrentNode.mIndex - 1].mColor != pCurrentNode.mColor)
                    {
                        return mLabyrinth.mNodes[pCurrentNode.mIndex - 1];
                    }
                }
                break;
            case CardinalDirection.North:
                if ((pCurrentNode.mIndex / pLabyrinthWidth) < pLabyrinthHeight-1 && pCurrentNode.mPotentialNeighbours.Contains(mLabyrinth.mNodes[pCurrentNode.mIndex + pLabyrinthWidth]))
                {
                    if (mLabyrinth.mNodes[pCurrentNode.mIndex + pLabyrinthWidth].mColor != pCurrentNode.mColor)
                    {
                        return mLabyrinth.mNodes[pCurrentNode.mIndex + pLabyrinthWidth];
                    }
                }
                break;
            case CardinalDirection.East:
                if ((pCurrentNode.mIndex % pLabyrinthWidth) != pLabyrinthWidth-1 && pCurrentNode.mPotentialNeighbours.Contains(mLabyrinth.mNodes[pCurrentNode.mIndex + 1]))
                {
                    if (mLabyrinth.mNodes[pCurrentNode.mIndex + 1].mColor != pCurrentNode.mColor)
                    {
                        return mLabyrinth.mNodes[pCurrentNode.mIndex + 1];
                    }
                }
                break;
            case CardinalDirection.South:
                if ((pCurrentNode.mIndex / pLabyrinthWidth) > 0 && pCurrentNode.mPotentialNeighbours.Contains(mLabyrinth.mNodes[pCurrentNode.mIndex - pLabyrinthWidth]))
                {
                    if (mLabyrinth.mNodes[pCurrentNode.mIndex - pLabyrinthWidth].mColor != pCurrentNode.mColor)
                    {
                        return mLabyrinth.mNodes[pCurrentNode.mIndex - pLabyrinthWidth];
                    }
                }
                break;
        }
#endif

        return null;
    }

    //Pyramidal Generator
    public bool generateStep3(ref int pNodeIndex1, ref int pNodeIndex2, int pLabyrinthWidth, int pLabyrinthHeight)
    {
        //bool lIsDone = false;

        pNodeIndex1 = -1;
        pNodeIndex2 = -1;

        if (mLastConnectedNode == null)
        {
            mLastConnectedNode = mLabyrinth.mNodes[mRandomGenerator.Next(mLabyrinth.mNodes.Count)];
            mLastConnectedNode.mColor = 0;
            mStillConnectablesNodes.Clear();
            mStillConnectablesNodes.Add(mLastConnectedNode);
        }

        bool lConnectionPerformed = false;

        while (!lConnectionPerformed)
        {

            mPotentialConnections.Clear();

            foreach (Node lPotentialNode in mLastConnectedNode.mPotentialNeighbours)
            {
                if (lPotentialNode != null && lPotentialNode.mColor == -1)
                {
                    mPotentialConnections.Add(lPotentialNode);
                }
            }

            if (mPotentialConnections.Count == 0)
            {
                mStillConnectablesNodes.Remove(mLastConnectedNode);
                //find another candidate
                mLastConnectedNode = mStillConnectablesNodes[mRandomGenerator.Next(mStillConnectablesNodes.Count)];
            }
            else
            {
                Node lNewConnectedNode = getConnectableNodeFromDirection(mLastConnectedNode, mCurrentDirection, pLabyrinthWidth, pLabyrinthHeight);

                while (lNewConnectedNode == null)
                {
                    mCurrentDirection = mRandomGenerator.Next(mShapeGenerator.GetNumberOfDirections());
                    lNewConnectedNode = getConnectableNodeFromDirection(mLastConnectedNode, mCurrentDirection, pLabyrinthWidth, pLabyrinthHeight);
                }

                //Node lNewConnectedNode = mPotentialConnections[mRandomGenerator.Next(mPotentialConnections.Count)];

                pNodeIndex1 = mLastConnectedNode.mIndex;
                pNodeIndex2 = lNewConnectedNode.mIndex;

                mLastConnectedNode.mConnectedNeighbours.Add(lNewConnectedNode);
                lNewConnectedNode.mConnectedNeighbours.Add(mLastConnectedNode);

                if (mPotentialConnections.Count == 1) //remove if new connection filled previous node
                {
                    mStillConnectablesNodes.Remove(mLastConnectedNode);
                }

                lNewConnectedNode.mColor = 0;

                if (canNodeBeConnected(lNewConnectedNode))
                {
                    mLastConnectedNode = lNewConnectedNode;
                    mStillConnectablesNodes.Add(mLastConnectedNode);
                }
                else //remove if new connection leads to a dead end
                {
                    mLastConnectedNode = mStillConnectablesNodes[mRandomGenerator.Next(mStillConnectablesNodes.Count)];
                }

                mNumConnectionsPerformed++;
                lConnectionPerformed = true;
            }
        }

        return (mNumConnectionsPerformed == (mLabyrinth.mNodes.Count - 1));
    }

    public bool generateStep4(ref int pNodeIndex1, ref int pNodeIndex2, int pLabyrinthWidth, int pLabyrinthHeight, int m_AlgorithmCorridorPseudoLength)
    {
        //bool lIsDone = false;

        pNodeIndex1 = -1;
        pNodeIndex2 = -1;

        if (mLastConnectedNode == null)
        {
            mLastConnectedNode = mLabyrinth.mNodes[mRandomGenerator.Next(mLabyrinth.mNodes.Count)];
            mLastConnectedNode.mColor = 0;
            mStillConnectablesNodes.Clear();
            mStillConnectablesNodes.Add(mLastConnectedNode);
        }

        bool lConnectionPerformed = false;

        while (!lConnectionPerformed)
        {

            mPotentialConnections.Clear();

            foreach (Node lPotentialNode in mLastConnectedNode.mPotentialNeighbours)
            {
                if (lPotentialNode != null && lPotentialNode.mColor == -1)
                {
                    mPotentialConnections.Add(lPotentialNode);
                }
            }

            if (mPotentialConnections.Count == 0)
            {
                mStillConnectablesNodes.Remove(mLastConnectedNode);
                //find another candidate
                mLastConnectedNode = mStillConnectablesNodes[mRandomGenerator.Next(mStillConnectablesNodes.Count)];
            }
            else
            {
                //randomize direction
                if(mRandomGenerator.Next(m_AlgorithmCorridorPseudoLength) == 0)
                {
                    mCurrentDirection = mRandomGenerator.Next(mShapeGenerator.GetNumberOfDirections());
                }

                Node lNewConnectedNode = getConnectableNodeFromDirection(mLastConnectedNode, mCurrentDirection, pLabyrinthWidth, pLabyrinthHeight);

                while (lNewConnectedNode == null)
                {
                    mCurrentDirection = mRandomGenerator.Next(mShapeGenerator.GetNumberOfDirections());
                    lNewConnectedNode = getConnectableNodeFromDirection(mLastConnectedNode, mCurrentDirection, pLabyrinthWidth, pLabyrinthHeight);
                }

                //Node lNewConnectedNode = mPotentialConnections[mRandomGenerator.Next(mPotentialConnections.Count)];

                pNodeIndex1 = mLastConnectedNode.mIndex;
                pNodeIndex2 = lNewConnectedNode.mIndex;

                mLastConnectedNode.mConnectedNeighbours.Add(lNewConnectedNode);
                lNewConnectedNode.mConnectedNeighbours.Add(mLastConnectedNode);

                if (mPotentialConnections.Count == 1) //remove if new connection filled previous node
                {
                    mStillConnectablesNodes.Remove(mLastConnectedNode);
                }

                lNewConnectedNode.mColor = 0;

                if (canNodeBeConnected(lNewConnectedNode))
                {
                    mLastConnectedNode = lNewConnectedNode;
                    mStillConnectablesNodes.Add(mLastConnectedNode);
                }
                else //remove if new connection leads to a dead end
                {
                    mLastConnectedNode = mStillConnectablesNodes[mRandomGenerator.Next(mStillConnectablesNodes.Count)];
                }

                mNumConnectionsPerformed++;
                lConnectionPerformed = true;
            }
        }

        return (mNumConnectionsPerformed == (mLabyrinth.mNodes.Count - 1));
    }

}

