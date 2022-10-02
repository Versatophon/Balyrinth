using Balyrinth;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareShapeGenerator:ShapeGeneratorInterface
{
    public int NumberOfDirections { get { return 4; } }
    public float StartAngle { get { return 180; } }
    public float LeftOffset { get { return 45; } }
    public float StepOffset { get { return -90; } }
    public float DoorsExtermitiesDistance { get { return 1.414215f; } }
    public float RoomsDistance { get { return 2; } }

    //public int GetNumberOfDirections()
    //{
    //    return 4;
    //}

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

#if false
    public static int getIndex(int pX, int pY, int pWidth, int pHeight)
    {
        return (pY * pWidth) + pX;
    }
#endif
    int mWidth = 10;
    int mHeight = 10;

    public Labyrinth generate(int pWidth = 10, int pHeight = 10, int pDepth = 10)
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
                
                //Order is important
                if (i > 0)//WEST
                {
                    lLaby.mNodes[lIndex].mPotentialNeighbours.Add(lLaby.mNodes[lIndex - 1]);
                }
                else
                {
                    lLaby.mNodes[lIndex].mPotentialNeighbours.Add(null);
                }

                if (j < mHeight - 1)//NORTH
                {
                    lLaby.mNodes[lIndex].mPotentialNeighbours.Add(lLaby.mNodes[lIndex + mWidth]);
                }
                else
                {
                    lLaby.mNodes[lIndex].mPotentialNeighbours.Add(null);
                }

                if (i < mWidth - 1)//EAST
                {
                    lLaby.mNodes[lIndex].mPotentialNeighbours.Add(lLaby.mNodes[lIndex + 1]);
                }
                else
                {
                    lLaby.mNodes[lIndex].mPotentialNeighbours.Add(null);
                }

                if (j > 0)//SOUTH
                {
                    lLaby.mNodes[lIndex].mPotentialNeighbours.Add(lLaby.mNodes[lIndex - mWidth]);
                }
                else
                {
                    lLaby.mNodes[lIndex].mPotentialNeighbours.Add(null);
                }
            }
        }

        return lLaby;
    }

    //public int getOppositeDirection(int pDirection)
    //{
    //    return (pDirection+GetNumberOfDirections()/2)%GetNumberOfDirections();
    //}

    public Vector3 getRoomPosition(int pIndex)
    {
        return new Vector3((pIndex % mWidth) * 2, 0, (pIndex / mWidth) * 2) * Balyrinth.Utilities.VIEW_SCALE;
    }

    public int getRoomIndex(Vector3 pPosition)
    {
        //TODO: Check this part
        int lZPosition = (int)((pPosition.z + 1f * Balyrinth.Utilities.VIEW_SCALE) / (2f * Balyrinth.Utilities.VIEW_SCALE));
        int lXPosition = (int)((pPosition.x + 1f * Balyrinth.Utilities.VIEW_SCALE) / (2f * Balyrinth.Utilities.VIEW_SCALE));

        lZPosition = Mathf.Clamp(lZPosition, 0, mHeight - 1);
        lXPosition = Mathf.Clamp(lXPosition, 0, mWidth - 1);

        return (lZPosition * mWidth) + lXPosition;
    }


#if false
    public void getDoorExtremities(int pRoomIndex, int pDirection, out Vector3 pLeft, out Vector3 pRight)
    {
        Vector3 lCenter = getRoomPosition(pRoomIndex);
        switch (pDirection) 
        {
            case 0://WEST
                pLeft = lCenter + new Vector3(-1, 0, -1) * Balyrinth.Utilities.VIEW_SCALE;
                pRight = lCenter + new Vector3(-1, 0, 1) * Balyrinth.Utilities.VIEW_SCALE;
                break;
            case 1://NORTH
                pLeft = lCenter + new Vector3(-1, 0, 1) * Balyrinth.Utilities.VIEW_SCALE;
                pRight = lCenter + new Vector3(1, 0, 1) * Balyrinth.Utilities.VIEW_SCALE;
                break;
            case 2://EAST
                pLeft = lCenter + new Vector3(1, 0, 1) * Balyrinth.Utilities.VIEW_SCALE;
                pRight = lCenter + new Vector3(1, 0, -1) * Balyrinth.Utilities.VIEW_SCALE;
                break;
            case 3://SOUTH
                pLeft = lCenter + new Vector3(1, 0, -1) * Balyrinth.Utilities.VIEW_SCALE;
                pRight = lCenter + new Vector3(-1, 0, -1) * Balyrinth.Utilities.VIEW_SCALE;
                break;
            default:
                pLeft = lCenter;
                pRight = lCenter;
                break;
        }
    }
#endif

    public Utilities.LabyShape getLabyShape()
    {
        return Balyrinth.Utilities.LabyShape.Rectangle;
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
