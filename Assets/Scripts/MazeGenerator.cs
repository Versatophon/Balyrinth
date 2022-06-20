
using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class MazeGenerator
{

    public enum CardinalDirection
    {
        West,
        North,
        East,
        South,
        Last,
    }

    System.Random mRandomGenerator = new System.Random();

    int mCurrentColor = 0;

    List<Node> mPotentialConnections = new List<Node>();
    List<Node> mStillConnectablesNodes;

    Labyrinth mLabyrinth;

    Node mLastConnectedNode = null;
    int mNumConnectionsPerformed = 0;

    CardinalDirection mCurrentDirection = CardinalDirection.East;

    public MazeGenerator(Labyrinth pLaby)
    {
        mLabyrinth = pLaby;
        mStillConnectablesNodes = new List<Node>(pLaby.mNodes);

        mCurrentDirection = (CardinalDirection)mRandomGenerator.Next((int)CardinalDirection.Last);
    }

    public void generate()
    {
        bool lIsDone = false;

        int lDummyIndex1 = -1;
        int lDummyIndex2 = -1;
        while (!lIsDone)
        {
            lIsDone = generateStep(ref lDummyIndex1, ref lDummyIndex2);
        }
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

            Debug.Log("Connectables Count " + mStillConnectablesNodes.Count);

            int lIndex = mRandomGenerator.Next(mStillConnectablesNodes.Count);

            Node lNode = mStillConnectablesNodes[lIndex];

            foreach (Node lPotentialNode in lNode.mPotentialNeighbours)
            {
                if ( lPotentialNode.mColor == -1 || lPotentialNode.mColor != lNode.mColor )
                {
                    mPotentialConnections.Add(lPotentialNode);
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
            if(lPotentialNode.mColor == -1 || lPotentialNode.mColor != pNode.mColor)
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
                if (lPotentialNode.mColor == -1)
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


    private Node getConnectableNodeFromDirection(Node pCurrentNode, CardinalDirection pDirection, int pLabyrinthWidth, int pLabyrinthHeight)
    {
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
                if (lPotentialNode.mColor == -1)
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
                    mCurrentDirection = (CardinalDirection)mRandomGenerator.Next((int)CardinalDirection.Last);
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
                if (lPotentialNode.mColor == -1)
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
                    mCurrentDirection = (CardinalDirection)mRandomGenerator.Next((int)CardinalDirection.Last);
                }

                Node lNewConnectedNode = getConnectableNodeFromDirection(mLastConnectedNode, mCurrentDirection, pLabyrinthWidth, pLabyrinthHeight);

                while (lNewConnectedNode == null)
                {
                    mCurrentDirection = (CardinalDirection)mRandomGenerator.Next((int)CardinalDirection.Last);
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

