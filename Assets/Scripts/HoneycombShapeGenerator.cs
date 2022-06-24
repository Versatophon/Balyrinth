using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoneycombShapeGenerator : ShapeGeneratorInterface
{
    public int GetNumberOfDirections()
    {
        return 6;
    }

    public int getNextCellIndex(int pCurrentcellIndex, int pDirection)
    {
        int lX = pCurrentcellIndex % mWidth;
        int lY = pCurrentcellIndex / mWidth;

        switch(pDirection)
        {
            case 0://WEST
                return getWest(lX, lY, mWidth, mHeight);
                break;
            case 1://NORTHWEST
                return getNorthWest(lX, lY, mWidth, mHeight);
                break;
            case 2://NORTHEAST
                return getNorthEast(lX, lY, mWidth, mHeight);
                break;
            case 3://EAST
                return getEast(lX, lY, mWidth, mHeight);
                break;
            case 4://SOUTHEAST
                return getSouthEast(lX, lY, mWidth, mHeight);
                break;
            case 5://SOUTHWEST
                return getSouthWest(lX, lY, mWidth, mHeight);
                break;
        }
        return -1;
    }

    public static int getEast(int pX, int pY, int pWidth, int pHeight)
    {
        return ((pX % pWidth) != (pWidth-1)) ? (pX + 1) + (pY * pWidth) : -1;
    }

    public static int getWest(int pX, int pY, int pWidth, int pHeight)
    {
        return ((pX % pWidth) != 0) ? (pX - 1) + (pY * pWidth) : -1;
    }

    public static int getNorthWest(int pX, int pY, int pWidth, int pHeight)
    {
        bool lIsEven = (pY % 2) == 0;

        if(lIsEven && pX == 0)
        {
            return -1;
        }

        int lIndexToReturn = (pY * pWidth) + pX + pWidth + (lIsEven?-1:0);

        return lIndexToReturn < pWidth * pHeight ? lIndexToReturn : -1;
    }

    public static int getNorthEast(int pX, int pY, int pWidth, int pHeight)
    {
        bool lIsEven = (pY % 2) == 0;

        if(!lIsEven && pX == pWidth-1)
        {
            return -1;
        }

        int lIndexToReturn = (pY * pWidth) + pX + pWidth + (lIsEven?0:1);

        return lIndexToReturn < pWidth * pHeight ? lIndexToReturn : -1;
    }

    public static int getSouthWest(int pX, int pY, int pWidth, int pHeight)
    {
        bool lIsEven = (pY % 2) == 0;

        if (lIsEven && pX == 0)
        {
            return -1;
        }

        int lIndexToReturn = (pY * pWidth) + pX - pWidth + (lIsEven ? -1 : 0);

        return lIndexToReturn >= 0 ? lIndexToReturn : -1;
    }

    public static int getSouthEast(int pX, int pY, int pWidth, int pHeight)
    {
        bool lIsEven = (pY % 2) == 0;

        if (!lIsEven && pX == pWidth - 1)
        {
            return -1;
        }

        int lIndexToReturn = (pY * pWidth) + pX - pWidth + (lIsEven ? 0 : 1);

        return lIndexToReturn >= 0 ? lIndexToReturn : -1;
    }

    public static int getIndex(int pX, int pY, int pWidth, int pHeight)
    {
        return (pY * pWidth) + pX;
    }

    public HoneycombShapeGenerator(int pWidth, int pHeight)
    {
        mWidth = pWidth;
        mHeight = pHeight;
    }

    int mWidth = 10;
    int mHeight = 10;

    public Labyrinth generate(int pWidth = 10, int pHeight = 10)
    {
        mWidth = pWidth;
        mHeight = pHeight;
        Labyrinth lLaby = new Labyrinth();

        for (int j = 0; j < mHeight; ++j)
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

                int lIndexToConnect = getEast(i, j, mWidth, mHeight);
                //Debug.Log("EA " + lIndexToConnect + " - " + (pWidth* pHeight));

                if( lIndexToConnect >= 0 )
                {
                    lLaby.mNodes[lIndex].mPotentialNeighbours.Add(lLaby.mNodes[lIndexToConnect]);
                }


                lIndexToConnect = getNorthEast(i, j, mWidth, mHeight);
                //Debug.Log("NE " + lIndexToConnect + " - " + (pWidth * pHeight));

                if ( lIndexToConnect >= 0 )
                {
                    lLaby.mNodes[lIndex].mPotentialNeighbours.Add(lLaby.mNodes[lIndexToConnect]);
                }


                lIndexToConnect = getNorthWest(i, j, mWidth, mHeight);
                //Debug.Log("NW " + lIndexToConnect + " - " + (pWidth * pHeight));

                if ( lIndexToConnect >= 0 )
                {
                    lLaby.mNodes[lIndex].mPotentialNeighbours.Add(lLaby.mNodes[lIndexToConnect]);
                }


                lIndexToConnect = getWest(i, j, mWidth, mHeight);
                //Debug.Log("WE " + lIndexToConnect + " - " + (pWidth * pHeight));

                if ( lIndexToConnect >= 0 )
                {
                    lLaby.mNodes[lIndex].mPotentialNeighbours.Add(lLaby.mNodes[lIndexToConnect]);
                }


                lIndexToConnect = getSouthWest(i, j, mWidth, mHeight);
                //Debug.Log("SW " + lIndexToConnect + " - " + (pWidth * pHeight));

                if ( lIndexToConnect >= 0 )
                {
                    lLaby.mNodes[lIndex].mPotentialNeighbours.Add(lLaby.mNodes[lIndexToConnect]);
                }


                lIndexToConnect = getSouthEast(i, j, mWidth, mHeight);
                //Debug.Log("SE " + lIndexToConnect + " - " + (pWidth * pHeight));

                if ( lIndexToConnect >= 0 )
                {
                    lLaby.mNodes[lIndex].mPotentialNeighbours.Add(lLaby.mNodes[lIndexToConnect]);
                }
            }
        }

        return lLaby;
    }
}
