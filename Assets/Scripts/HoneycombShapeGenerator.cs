using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoneycombShapeGenerator
{

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

                int lIndexToConnect = getEast(i, j, pWidth, pHeight);
                //Debug.Log("EA " + lIndexToConnect + " - " + (pWidth* pHeight));

                if( lIndexToConnect >= 0 )
                {
                    lLaby.mNodes[lIndex].mPotentialNeighbours.Add(lLaby.mNodes[lIndexToConnect]);
                }


                lIndexToConnect = getNorthEast(i, j, pWidth, pHeight);
                //Debug.Log("NE " + lIndexToConnect + " - " + (pWidth * pHeight));

                if ( lIndexToConnect >= 0 )
                {
                    lLaby.mNodes[lIndex].mPotentialNeighbours.Add(lLaby.mNodes[lIndexToConnect]);
                }


                lIndexToConnect = getNorthWest(i, j, pWidth, pHeight);
                //Debug.Log("NW " + lIndexToConnect + " - " + (pWidth * pHeight));

                if ( lIndexToConnect >= 0 )
                {
                    lLaby.mNodes[lIndex].mPotentialNeighbours.Add(lLaby.mNodes[lIndexToConnect]);
                }


                lIndexToConnect = getWest(i, j, pWidth, pHeight);
                //Debug.Log("WE " + lIndexToConnect + " - " + (pWidth * pHeight));

                if ( lIndexToConnect >= 0 )
                {
                    lLaby.mNodes[lIndex].mPotentialNeighbours.Add(lLaby.mNodes[lIndexToConnect]);
                }


                lIndexToConnect = getSouthWest(i, j, pWidth, pHeight);
                //Debug.Log("SW " + lIndexToConnect + " - " + (pWidth * pHeight));

                if ( lIndexToConnect >= 0 )
                {
                    lLaby.mNodes[lIndex].mPotentialNeighbours.Add(lLaby.mNodes[lIndexToConnect]);
                }


                lIndexToConnect = getSouthEast(i, j, pWidth, pHeight);
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
