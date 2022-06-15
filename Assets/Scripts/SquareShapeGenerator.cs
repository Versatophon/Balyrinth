using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareShapeGenerator
{
   public static Labyrinth generate(int pWidth = 10, int pHeight = 10)
    {
        Labyrinth lLaby = new Labyrinth();

        for (int j = 0; j < pHeight; ++j)
        {
            for (int i = 0; i < pWidth; ++i)
            {
                Node lNode = new Node();

                lLaby.mNodes.Add(lNode);
            }
        }


        for (int j = 0; j < pHeight; ++j)
        {
            for (int i = 0; i < pWidth; ++i)
            {
                int lIndex = j * pWidth + i;

                lLaby.mNodes[lIndex].mIndex = lIndex;

                if (i > 0)
                {
                    lLaby.mNodes[lIndex].mPotentialNeighbours.Add(lLaby.mNodes[lIndex - 1]);
                }

                if (i < pWidth - 1)
                {
                    lLaby.mNodes[lIndex].mPotentialNeighbours.Add(lLaby.mNodes[lIndex + 1]);
                }

                if (j > 0)
                {
                    lLaby.mNodes[lIndex].mPotentialNeighbours.Add(lLaby.mNodes[lIndex - pWidth]);
                }

                if (j < pHeight - 1)
                {
                    lLaby.mNodes[lIndex].mPotentialNeighbours.Add(lLaby.mNodes[lIndex + pWidth]);
                }
            }
        }

        return lLaby;
    }
}
