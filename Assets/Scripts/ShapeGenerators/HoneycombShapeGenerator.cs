using Balyrinth;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoneycombShapeGenerator : ShapeGeneratorInterface
{
    public int NumberOfDirections { get { return 6; } }
    public float StartAngle { get {return 180;} }
    public float LeftOffset { get {return 30;} }
    public float StepOffset { get { return -60; } }
    public float DoorsExtermitiesDistance { get { return 1.154775f; } }
    public float RoomsDistance { get { return 2; } }

    public int getNextCellIndex(int pCurrentcellIndex, int pDirection)
    {
        int lX = pCurrentcellIndex % mWidth;
        int lY = pCurrentcellIndex / mWidth;

        switch(pDirection)
        {
            case 0://WEST
                return getWest(lX, lY);
                break;
            case 1://NORTHWEST
                return getNorthWest(lX, lY);
                break;
            case 2://NORTHEAST
                return getNorthEast(lX, lY);
                break;
            case 3://EAST
                return getEast(lX, lY);
                break;
            case 4://SOUTHEAST
                return getSouthEast(lX, lY);
                break;
            case 5://SOUTHWEST
                return getSouthWest(lX, lY);
                break;
        }
        return -1;
    }

    public int getEast(int pX, int pY)
    {
        return ((pX % mWidth) != (mWidth-1)) ? (pX + 1) + (pY * mWidth) : -1;
    }

    public int getWest(int pX, int pY)
    {
        return ((pX % mWidth) != 0) ? (pX - 1) + (pY * mWidth) : -1;
    }

    public int getNorthWest(int pX, int pY)
    {
        bool lIsEven = (pY % 2) == 0;

        if(lIsEven && pX == 0)
        {
            return -1;
        }

        int lIndexToReturn = (pY * mWidth) + pX + mWidth + (lIsEven?-1:0);

        return lIndexToReturn < mWidth * mHeight ? lIndexToReturn : -1;
    }

    public int getNorthEast(int pX, int pY)
    {
        bool lIsEven = (pY % 2) == 0;

        if(!lIsEven && pX == mWidth-1)
        {
            return -1;
        }

        int lIndexToReturn = (pY * mWidth) + pX + mWidth + (lIsEven?0:1);

        return lIndexToReturn < mWidth * mHeight ? lIndexToReturn : -1;
    }

    public int getSouthWest(int pX, int pY)
    {
        bool lIsEven = (pY % 2) == 0;

        if (lIsEven && pX == 0)
        {
            return -1;
        }

        int lIndexToReturn = (pY * mWidth) + pX - mWidth + (lIsEven ? -1 : 0);

        return lIndexToReturn >= 0 ? lIndexToReturn : -1;
    }

    public int getSouthEast(int pX, int pY)
    {
        bool lIsEven = (pY % 2) == 0;

        if (!lIsEven && pX == mWidth - 1)
        {
            return -1;
        }

        int lIndexToReturn = (pY * mWidth) + pX - mWidth + (lIsEven ? 0 : 1);

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

    public Labyrinth generate(int pWidth = 10, int pHeight = 10, int pDepth = 10)
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

                //Order is important
                int lIndexToConnect = getWest(i, j);
                //Debug.Log("WE " + lIndexToConnect + " - " + (pWidth * pHeight));

                if (lIndexToConnect >= 0)
                {
                    lLaby.mNodes[lIndex].mPotentialNeighbours.Add(lLaby.mNodes[lIndexToConnect]);
                }
                else
                {
                    lLaby.mNodes[lIndex].mPotentialNeighbours.Add(null);
                }


                lIndexToConnect = getNorthWest(i, j);
                //Debug.Log("NW " + lIndexToConnect + " - " + (pWidth * pHeight));

                if ( lIndexToConnect >= 0 )
                {
                    lLaby.mNodes[lIndex].mPotentialNeighbours.Add(lLaby.mNodes[lIndexToConnect]);
                }
                else
                {
                    lLaby.mNodes[lIndex].mPotentialNeighbours.Add(null);
                }


                lIndexToConnect = getNorthEast(i, j);
                //Debug.Log("NE " + lIndexToConnect + " - " + (pWidth * pHeight));

                if (lIndexToConnect >= 0)
                {
                    lLaby.mNodes[lIndex].mPotentialNeighbours.Add(lLaby.mNodes[lIndexToConnect]);
                }
                else
                {
                    lLaby.mNodes[lIndex].mPotentialNeighbours.Add(null);
                }


                lIndexToConnect = getEast(i, j);
                //Debug.Log("EA " + lIndexToConnect + " - " + (pWidth* pHeight));

                if (lIndexToConnect >= 0)
                {
                    lLaby.mNodes[lIndex].mPotentialNeighbours.Add(lLaby.mNodes[lIndexToConnect]);
                }
                else
                {
                    lLaby.mNodes[lIndex].mPotentialNeighbours.Add(null);
                }


                lIndexToConnect = getSouthEast(i, j);
                //Debug.Log("SE " + lIndexToConnect + " - " + (pWidth * pHeight));

                if (lIndexToConnect >= 0)
                {
                    lLaby.mNodes[lIndex].mPotentialNeighbours.Add(lLaby.mNodes[lIndexToConnect]);
                }
                else
                {
                    lLaby.mNodes[lIndex].mPotentialNeighbours.Add(null);
                }


                lIndexToConnect = getSouthWest(i, j);
                //Debug.Log("SW " + lIndexToConnect + " - " + (pWidth * pHeight));

                if ( lIndexToConnect >= 0 )
                {
                    lLaby.mNodes[lIndex].mPotentialNeighbours.Add(lLaby.mNodes[lIndexToConnect]);
                }
                else
                {
                    lLaby.mNodes[lIndex].mPotentialNeighbours.Add(null);
                }
            }
        }

        return lLaby;
    }

    public Vector3 getRoomPosition(int pIndex)
    {
        return new Vector3(2f * ((pIndex % mWidth) + ((pIndex / mWidth) % 2 == 0 ? 0 : 0.5f)), 0, (3f / Mathf.Sqrt(3)) * (pIndex / mWidth)) * Balyrinth.Utilities.VIEW_SCALE;
    }

    public int getRoomIndex(Vector3 pPosition)
    {
        //4/3 * sqrt(3) ==> half height

        //?x?           ? sqrt(3)   sqrt(3) / 2 ?   ?q?
        //? ?  = size × ?                       ? × ? ?
        //?y?           ?    0          3 / 2   ?   ?r?

        //pPosition.z + 4 / 3 * sqrt(3)
        //sqrt(3) * 2/3 * 3/2
        //3/2
        
        
        int lZPosition = (int)((pPosition.z + Mathf.Sqrt(3) * Balyrinth.Utilities.VIEW_SCALE) / (Mathf.Sqrt(3) * Balyrinth.Utilities.VIEW_SCALE));

        bool lEven = (lZPosition % 2) == 0;
        int lXPosition = (int)(((pPosition.x + (lEven ? Balyrinth.Utilities.VIEW_SCALE : 0)) / (2f * Balyrinth.Utilities.VIEW_SCALE)));

        lZPosition = Mathf.Clamp(lZPosition, 0, mHeight - 1);
        lXPosition = Mathf.Clamp(lXPosition, 0, mWidth - 1);

        int lRoomIndex = lZPosition * mWidth + lXPosition;// HoneycombShapeGenerator.getIndex(lXPosition, lZPosition, lNumberOfColumns, lNumberOfRows);

        float lMagnitude =  (getRoomPosition(lRoomIndex) - pPosition).magnitude;

#if true
        for (int j = -1; j < 2; ++j)
        {
            for (int i = -1; i < 2; ++i)
            {
                int lLocalRoomIndex = lXPosition + i + (lZPosition + j) * mWidth;
                if (lLocalRoomIndex >= 0 && lLocalRoomIndex < mWidth * mHeight)
                {
                    float lLocalMagnitude = (getRoomPosition(lLocalRoomIndex) - pPosition).magnitude; 

                    if (lLocalMagnitude < lMagnitude)
                    {
                        lMagnitude = lLocalMagnitude;
                        lRoomIndex = lLocalRoomIndex;
                    }
                }
            }
        }
#endif


        return lRoomIndex;
    }

    public Utilities.LabyShape getLabyShape()
    {
        return Utilities.LabyShape.HoneyComb;
    }

    public int getWidth()
    {
        return mWidth;
    }

    public int getHeight()
    {
        return mHeight;
    }

    public int getDepth()
    {
        return 0;
    }
}
