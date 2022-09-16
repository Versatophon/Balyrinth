using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

#if true
public class HyperMazeShapeGenerator : ShapeGeneratorInterface
{
    int mWidth = 10;
    int mHeight = 10;
    int mDepth = 10;

    public HyperMazeShapeGenerator(int pWidth, int pHeight, int pDepth)
    {
        mWidth = pWidth;
        mHeight = pHeight;
        mDepth = pDepth;
    }
    public int GetNumberOfDirections()
    {
        return 6;
    }

    public int getNextCellIndex(int pCurrentcellIndex, int pDirection)
    {
        int lX = pCurrentcellIndex % mWidth;
        int lY = (pCurrentcellIndex % (mWidth * mHeight)) / mWidth;
        int lZ = pCurrentcellIndex / (mWidth*mHeight);

        switch (pDirection)
        {
            case 0://WEST
                return getWest(lX, lY, lZ);
                break;
            case 1://NORTH
                return getNorth(lX, lY, lZ);
                break;
            case 2://FLOOR
                return getFloor(lX, lY, lZ);
                break;
            case 3://EAST
                return getEast(lX, lY, lZ);
                break;
            case 4://SOUTH
                return getSouth(lX, lY, lZ);
                break;
            case 5://CEILING
                return getCeiling(lX, lY, lZ);
                break;
        }
        return -1;
    }

    //TODO: Implements a better way to perform these calculus
    public int getWest(int pX, int pY, int pZ)
    {
        return (pX != 0) ? (pX - 1) + (pY * mWidth) + (pZ * mWidth * mHeight) : -1;
    }

    public int getNorth(int pX, int pY, int pZ)
    {
        return (pY != mHeight-1) ? pX + ((pY+1) * mWidth) + (pZ * mWidth * mHeight) : -1;
    }

    public int getEast(int pX, int pY, int pZ)
    {
        return (pX != mWidth-1) ? (pX + 1) + (pY * mWidth) + (pZ * mWidth * mHeight) : -1;
    }

    public int getSouth(int pX, int pY, int pZ)
    {
        return (pY != 0) ? pX + ((pY-1) * mWidth) + (pZ * mWidth * mHeight) : -1;
    }

    public int getFloor(int pX, int pY, int pZ)
    {
        return (pZ != 0) ? pX + (pY * mWidth) + ((pZ-1) * mWidth * mHeight) : -1;
    }

    public int getCeiling(int pX, int pY, int pZ)
    {
        return (pZ != (mDepth - 1)) ? pX + (pY * mWidth) + ((pZ+1) * mWidth * mHeight) : -1;
    }

    public Labyrinth generate(int pWidth = 10, int pHeight = 10, int pDepth = 10)
    {
        mWidth = pWidth;
        mHeight = pHeight;
        mDepth = pDepth;

        Labyrinth lLaby = new Labyrinth();

        for (int k = 0; k < mDepth; ++k)
        {
            for (int j = 0; j < mHeight; ++j)
            {
                for (int i = 0; i < mWidth; ++i)
                {
                    Node lNode = new Node();

                    lLaby.mNodes.Add(lNode);
                }
            }
        }


        for (int k = 0; k < mDepth; ++k)
        {
            for (int j = 0; j < mHeight; ++j)
            {
                for (int i = 0; i < mWidth; ++i)
                {
                    int lIndex = (k * mWidth * mHeight) + (j * mWidth) + i;

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

                    if (k > 0)//FLOOR
                    {
                        lLaby.mNodes[lIndex].mPotentialNeighbours.Add(lLaby.mNodes[lIndex - (mWidth * mHeight)]);
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

                    if (k < mDepth - 1)//CEILING
                    {
                        lLaby.mNodes[lIndex].mPotentialNeighbours.Add(lLaby.mNodes[lIndex + (mWidth * mHeight)]);
                    }
                    else
                    {
                        lLaby.mNodes[lIndex].mPotentialNeighbours.Add(null);
                    }
                }
            }
        }

        return lLaby;
    }

    public int getOppositeDirection(int pDirection)
    {
        return (pDirection + GetNumberOfDirections() / 2) % GetNumberOfDirections();
    }

    public Vector3 getRoomPosition(int pIndex)
    {
        return new Vector3((pIndex % mWidth) * 2, (pIndex / (mWidth * mHeight)) * 2, (((pIndex%(mWidth*mHeight)) / mWidth) * 2)) * Balyrinth.Utilities.VIEW_SCALE;
    }

    public void getDoorExtremities(int pRoomIndex, int pDirection, out Vector3 pLeft, out Vector3 pRight)
    {
        pLeft = Vector3.zero;
        pRight = Vector3.zero;
    }

    public Balyrinth.Utilities.LabyShape getLabyShape()
    {
        return Balyrinth.Utilities.LabyShape.Hypermaze;
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
        return mDepth;
    }
}


#endif