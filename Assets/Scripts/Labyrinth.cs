using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Labyrinth
{
    private struct NodeDistance
    {
        public Node mNode;
        public int mDistance;
    }

    public List<Node> mNodes;

    private int[] mDistances;

    public Labyrinth()
    {
        mNodes = new List<Node>();
    }

    public void findMaximumPathExtremities(out Node pStartingNode, out Node pEndingNode)
    {
        Node lNode = mNodes[0];

        while (lNode.mColor == -1)
        {
            lNode = mNodes[lNode.mIndex + 1];
        }

        mDistances = Enumerable.Repeat(-1, mNodes.Count).ToArray();

        int lLargerIndex = lNode.mIndex;
        mDistances[lNode.mIndex] = 0;
        int lLargerValue = mDistances[lNode.mIndex];

        //First Step - find starting Node
        propagateDistances(lNode, ref lLargerIndex, ref lLargerValue);

        pStartingNode = mNodes[lLargerIndex];
        mDistances = Enumerable.Repeat(-1, mNodes.Count).ToArray();
        mDistances[pStartingNode.mIndex] = 0;
        lLargerValue = mDistances[pStartingNode.mIndex];

        //Second Step - find ending Node
        propagateDistances(pStartingNode, ref lLargerIndex, ref lLargerValue);
        pEndingNode = mNodes[lLargerIndex];
    }

    public void findMaximumCompletePath(Node pStartingNode, Node pEndingNode, out List<Node> pPath)
    {
        pPath = new List<Node>();
        //
        Node lCurrentNode = pEndingNode;
        pPath.Insert(0, lCurrentNode);
        int lCurrentDistance = mDistances[lCurrentNode.mIndex];
        while (lCurrentDistance > 1)
        {
            foreach (Node lNextNode in lCurrentNode.mConnectedNeighbours)
            {//Find in connected nodes, the one which is the next on the path
                if (mDistances[lNextNode.mIndex] == (lCurrentDistance - 1))
                {
                    --lCurrentDistance;
                    lCurrentNode = lNextNode;

                    pPath.Insert(0, lCurrentNode);
                    break;
                }
            }
        }

        pPath.Insert(0, pStartingNode);
    }

    void propagateDistances(Node pNode, ref int pLargerIndex, ref int pLargerValue)
    {
        foreach (Node lNode in pNode.mConnectedNeighbours)
        {
            if (mDistances[lNode.mIndex] == -1)
            {
                mDistances[lNode.mIndex] = mDistances[pNode.mIndex] + 1;
                if(mDistances[lNode.mIndex] > pLargerValue)
                {
                    pLargerValue = mDistances[lNode.mIndex];
                    pLargerIndex = lNode.mIndex;
                }

                propagateDistances(lNode, ref pLargerIndex, ref pLargerValue);
            }
        }
    }
}
