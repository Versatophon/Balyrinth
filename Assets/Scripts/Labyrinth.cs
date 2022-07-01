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

    public Labyrinth()
    {
        mNodes = new List<Node>();
    }

    public void findMaximumPath(out Node pFirstNode, out Node pSecondNode)
    {
        //pFirstNode = null;
        //pSecondNode = null;

        //Dictionary<Node, NodeDistance> lDistancesDictionary = new Dictionary<Node, NodeDistance>();
        //
        //HashSet<Node> lVisitedNodes = new HashSet<Node>();

        Node lNode = mNodes[0];

        while (lNode.mColor == -1)
        {
            lNode = mNodes[lNode.mIndex + 1];
        }

        int[] lDistances = Enumerable.Repeat(-1, mNodes.Count).ToArray();

        int lLargerIndex = lNode.mIndex;
        lDistances[lNode.mIndex] = 0;
        int lLargerValue = lDistances[lNode.mIndex];


        //First Step - find starting Node
        propagateDistances(lNode, lDistances, ref lLargerIndex, ref lLargerValue);

        pFirstNode = mNodes[lLargerIndex];
        lDistances = Enumerable.Repeat(-1, mNodes.Count).ToArray();
        lDistances[pFirstNode.mIndex] = 0;
        lLargerValue = lDistances[pFirstNode.mIndex];

        //Second Step - find ending Node
        propagateDistances(pFirstNode, lDistances, ref lLargerIndex, ref lLargerValue);
        pSecondNode = mNodes[lLargerIndex];
    }

    void propagateDistances(Node pNode, int[] pReferenceDistances, ref int pLargerIndex, ref int pLargerValue)
    {
        foreach (Node lNode in pNode.mConnectedNeighbours)
        {
            if (pReferenceDistances[lNode.mIndex] == -1)
            {
                pReferenceDistances[lNode.mIndex] = pReferenceDistances[pNode.mIndex] + 1;
                if(pReferenceDistances[lNode.mIndex] > pLargerValue)
                {
                    pLargerValue = pReferenceDistances[lNode.mIndex];
                    pLargerIndex = lNode.mIndex;
                }

                propagateDistances(lNode, pReferenceDistances, ref pLargerIndex, ref pLargerValue);
            }
        }
    }
}
