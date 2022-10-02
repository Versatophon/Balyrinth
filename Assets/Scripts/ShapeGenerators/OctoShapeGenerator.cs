using Balyrinth;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OctoShapeGenerator : ShapeGeneratorInterface
{
    public int NumberOfDirections { get { return 8; } }
    public float StartAngle { get { return 180; } }
    public float LeftOffset { get { return 22.5f; } }
    public float StepOffset { get { return -45; } }
    public float DoorsExtermitiesDistance { get { return 1.08239f; } }
    public float RoomsDistance { get { return 2; } }

    //public int GetNumberOfDirections()
    //{
    //    return 8;
    //}

    public int getNextCellIndex(int pCurrentcellIndex, int pDirection)
    {
        int lX = pCurrentcellIndex % mWidth;
        int lY = pCurrentcellIndex / mWidth;

        switch (pDirection)
        {
            case 0://WEST
                return getWest(lX, lY);
                break;
            case 1://NORTHWEST
                return getNorthWest(lX, lY);
                break;
            case 2://NORTH
                return getNorth(lX, lY);
                break;
            case 3://NORTHEAST
                return getNorthEast(lX, lY);
                break;
            case 4://EAST
                return getEast(lX, lY);
                break;
            case 5://SOUTHEAST
                return getSouthEast(lX, lY);
                break;
            case 6://SOUTH
                return getSouth(lX, lY);
                break;
            case 7://SOUTHWEST
                return getSouthWest(lX, lY);
                break;
        }
        return -1;
    }

    public int getEast(int pX, int pY)
    {
        return ((pX % mWidth) != (mWidth - 1)) ? (pX + 1) + (pY * mWidth) : -1;
    }

    public int getWest(int pX, int pY)
    {
        return ((pX % mWidth) != 0) ? (pX - 1) + (pY * mWidth) : -1;
    }
    

    public int getNorth(int pX, int pY)
    {
        return ((pY % mHeight) != (mHeight - 1)) ? (pX + 0) + ((pY+1) * mWidth) : -1;
    }

    public int getSouth(int pX, int pY)
    {
        return ((pY % mHeight) != 0) ? (pX + 0) + ((pY - 1) * mWidth) : -1;
    }

    public int getNorthWest(int pX, int pY)
    {
        return ((pY % mHeight) != (mHeight - 1)) && ((pX % mWidth) != 0) ? (pX - 1) + ((pY + 1) * mWidth) : -1;
    }

    public int getNorthEast(int pX, int pY)
    {
        return ((pY % mHeight) != (mHeight - 1)) && ((pX % mWidth) != (mWidth - 1)) ? (pX + 1) + ((pY + 1) * mWidth) : -1;
    }

    public int getSouthWest(int pX, int pY)
    {
        return ((pY % mHeight) != 0) && ((pX % mWidth) != 0) ? (pX - 1) + ((pY - 1) * mWidth) : -1;
    }

    public int getSouthEast(int pX, int pY)
    {
        return ((pY % mHeight) != 0) && ((pX % mWidth) != (mWidth - 1)) ? (pX + 1) + ((pY - 1) * mWidth) : -1;
    }

    public static int getIndex(int pX, int pY, int pWidth, int pHeight)
    {
        return (pY * pWidth) + pX;
    }

    public OctoShapeGenerator(int pWidth, int pHeight)
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

                if (lIndexToConnect >= 0)
                {
                    lLaby.mNodes[lIndex].mPotentialNeighbours.Add(lLaby.mNodes[lIndexToConnect]);
                }
                else
                {
                    lLaby.mNodes[lIndex].mPotentialNeighbours.Add(null);
                }


                lIndexToConnect = getNorth(i, j);
                //Debug.Log("NW " + lIndexToConnect + " - " + (pWidth * pHeight));

                if (lIndexToConnect >= 0)
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


                lIndexToConnect = getSouth(i, j);
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

                if (lIndexToConnect >= 0)
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
    static Vector3[] sLeftDoorSidePosition =
    {
        new Vector3(-1, 0, -0.414f) * Balyrinth.Utilities.VIEW_SCALE,
        new Vector3(-1, 0, 0.414f) * Balyrinth.Utilities.VIEW_SCALE,
        new Vector3(-0.414f, 0, 1f) * Balyrinth.Utilities.VIEW_SCALE,
        new Vector3(0.414f, 0, 1f) * Balyrinth.Utilities.VIEW_SCALE,
        new Vector3(1f, 0, 0.414f) * Balyrinth.Utilities.VIEW_SCALE,
        new Vector3(1f, 0, -0.414f) * Balyrinth.Utilities.VIEW_SCALE,
        new Vector3(0.414f, 0, -1f) * Balyrinth.Utilities.VIEW_SCALE,
        new Vector3(-0.414f, 0, -1f) * Balyrinth.Utilities.VIEW_SCALE
    };

    public Vector3 getLeftDoorSidePosition(int pDirection)
    {
        return sLeftDoorSidePosition[pDirection];
    }
#endif

#if false
    const float sStartAngle = -180;
    const float sLeftOffset = -22.5f;
    //const float sRightOffset = 30;
    const float sStepOffset = 45;

    public Vector3 getLeftDoorSidePosition(int pDirection)
    {
        float lleftAngle = Mathf.Deg2Rad * (sStartAngle + pDirection * sStepOffset + sLeftOffset);
        return (new Vector3(Mathf.Cos(lleftAngle), 0, -Mathf.Sin(lleftAngle)) * Balyrinth.Utilities.VIEW_SCALE);
    }

    public void getDoorLocalExtremities(int pRoomIndex, int pDirection, out Vector3 pLeft, out Vector3 pRight)//Hexagon of 1 unit radius
    {
        //Vector3 lCenter = getRoomPosition(pRoomIndex);

        switch (pDirection)
        {
            case 0://WEST
                pLeft = new Vector3(-1, 0, -0.414f) * Balyrinth.Utilities.VIEW_SCALE;
                pRight = new Vector3(-1, 0, 0.414f) * Balyrinth.Utilities.VIEW_SCALE;
                break;
            case 1://NORTHWEST
                pLeft = new Vector3(-1, 0, 0.414f) * Balyrinth.Utilities.VIEW_SCALE;
                pRight = new Vector3(-0.414f, 0, 1f) * Balyrinth.Utilities.VIEW_SCALE;
                break;
            case 2://NORTH
                pLeft = new Vector3(-0.414f, 0, 1f) * Balyrinth.Utilities.VIEW_SCALE;
                pRight = new Vector3(0.414f, 0, 1f) * Balyrinth.Utilities.VIEW_SCALE;
                break;
            case 3://NORTHEAST
                pLeft = new Vector3(0.414f, 0, 1f) * Balyrinth.Utilities.VIEW_SCALE;
                pRight = new Vector3(1f, 0, 0.414f) * Balyrinth.Utilities.VIEW_SCALE;
                break;
            case 4://EAST
                pLeft = new Vector3(1f, 0, 0.414f) * Balyrinth.Utilities.VIEW_SCALE;
                pRight = new Vector3(1f, 0, -0.414f) * Balyrinth.Utilities.VIEW_SCALE;
                break;
            case 5://SOUHTEAST
                pLeft = new Vector3(1f, 0, -0.414f) * Balyrinth.Utilities.VIEW_SCALE;
                pRight = new Vector3(0.414f, 0, -1f) * Balyrinth.Utilities.VIEW_SCALE;
                break;
            case 6://SOUTH
                pLeft = new Vector3(0.414f, 0, -1f) * Balyrinth.Utilities.VIEW_SCALE;
                pRight = new Vector3(-0.414f, 0, -1f) * Balyrinth.Utilities.VIEW_SCALE;
                break;
            case 7://SOUTHWEST
                pLeft = new Vector3(-0.414f, 0, -1f) * Balyrinth.Utilities.VIEW_SCALE;
                pRight = new Vector3(-1f, 0, -0.414f) * Balyrinth.Utilities.VIEW_SCALE;
                break;
            default:
                pLeft = Vector3.zero;
                pRight = Vector3.zero;
                break;
        }
    }
#endif

    public Utilities.LabyShape getLabyShape()
    {
        return Utilities.LabyShape.Octogonized;
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
