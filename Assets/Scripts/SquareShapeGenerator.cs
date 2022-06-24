using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareShapeGenerator:ShapeGeneratorInterface
{
    public int GetNumberOfDirections()
    {
        return 4;
    }

    public int getNextCellIndex(int pCurrentcellIndex, int pDirection)
    {
        //West,North,East,South,
        switch(pDirection)
        {
            case 0://WEST
                if ((pCurrentcellIndex % mWidth) != 0 )
                {
                    return pCurrentcellIndex - 1;
                }
                break;
            case 1://NORTH
                if ((pCurrentcellIndex / mWidth) < mHeight - 1 )
                {
                    return pCurrentcellIndex + mWidth;
                }
                break;
            case 2://EAST
                if ((pCurrentcellIndex % mWidth) != mWidth-1)
                {
                    return pCurrentcellIndex + 1;
                }
                break;
            case 3://SOUTH
                if ((pCurrentcellIndex / mWidth) > 0)
                {
                    return pCurrentcellIndex - mWidth;
                }
                break;
            default://ERROR
                return -1;
                break;
        }

        return -1;
    }

    public SquareShapeGenerator(int pWidth, int pHeight)
    {
        mWidth = pWidth;
        mHeight = pHeight;
    }

    public static int getIndex(int pX, int pY, int pWidth, int pHeight)
    {
        return (pY * pWidth) + pX;
    }

    int mWidth = 10;
    int mHeight = 10;

    public Labyrinth generate(int pWidth = 10, int pHeight = 10)
    {
        mWidth = pWidth;
        mHeight = pHeight;

        Labyrinth lLaby = new Labyrinth();

        for (int j = 0; j < pHeight; ++j)
        {
            for (int i = 0; i < mWidth; ++i)
            {
                Node lNode = new Node();

                lLaby.mNodes.Add(lNode);
            }
        }


        for (int j = 0; j < mHeight; ++j)
        {
            for (int i = 0; i < mWidth; ++i)
            {
                int lIndex = j * mWidth + i;

                lLaby.mNodes[lIndex].mIndex = lIndex;

                if (i > 0)
                {
                    lLaby.mNodes[lIndex].mPotentialNeighbours.Add(lLaby.mNodes[lIndex - 1]);
                }

                if (i < mWidth - 1)
                {
                    lLaby.mNodes[lIndex].mPotentialNeighbours.Add(lLaby.mNodes[lIndex + 1]);
                }

                if (j > 0)
                {
                    lLaby.mNodes[lIndex].mPotentialNeighbours.Add(lLaby.mNodes[lIndex - mWidth]);
                }

                if (j < mHeight - 1)
                {
                    lLaby.mNodes[lIndex].mPotentialNeighbours.Add(lLaby.mNodes[lIndex + mWidth]);
                }
            }
        }

        return lLaby;
    }
}
