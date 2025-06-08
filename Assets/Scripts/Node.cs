using System.Collections.Generic;

//Class that represent a graph node
public class Node
{
    public List<Node> mPotentialNeighbours = new List<Node>();
    public List<Node> mConnectedNeighbours = new List<Node>();

    public int mColor = -1;
    public int mIndex = -1;

    public Node()
    {
    }

    public void propagateColor(int pColorToPropagate)
    {
        if (mColor == pColorToPropagate)
        {
            return;
        }

        mColor = pColorToPropagate;
        foreach (Node lNeighbour in mConnectedNeighbours)
        {
            lNeighbour.propagateColor(pColorToPropagate);
        }
    }
}
