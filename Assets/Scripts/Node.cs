using System.Collections.Generic;

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
        mColor = pColorToPropagate;
        foreach (Node lNeighbour in mConnectedNeighbours)
        {
            if (lNeighbour.mColor != mColor)
            {
                lNeighbour.propagateColor(pColorToPropagate);
            }
        }
    }
}
