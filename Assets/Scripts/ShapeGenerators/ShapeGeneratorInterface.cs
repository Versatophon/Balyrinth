using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public interface ShapeGeneratorInterface
{
    public int NumberOfDirections { get; }
    //public int GetNumberOfDirections();

    public Labyrinth generate(int pWidth = 10, int pHeight = 10, int pDepth = 10);

    public int getNextCellIndex(int pCurrentcellIndex, int pDirection);

    public Vector3 getRoomPosition(int pIndex);

    public int getRoomIndex(Vector3 pPosition);

    public Balyrinth.Utilities.LabyShape getLabyShape();

    public int getWidth();

    public int getHeight();

    public int getDepth();

    //Implemented Method

    float StartAngle { get; }
    float LeftOffset { get; }
    float StepOffset { get; }
    float DoorsExtermitiesDistance { get; }
    float RoomsDistance { get; }

    public int getOppositeDirection(int pDirection)
    {
        return (pDirection + (NumberOfDirections / 2)) % NumberOfDirections;
    }

    public Vector3 getLeftDoorSidePosition(int pDirection)
    {
        float lleftAngle = Mathf.Deg2Rad * (StartAngle + pDirection * StepOffset + LeftOffset);
        return (new Vector3(Mathf.Cos(lleftAngle), 0, Mathf.Sin(lleftAngle)) * DoorsExtermitiesDistance * Balyrinth.Utilities.VIEW_SCALE);
    }

    public void getDoorLocalExtremities(int pDirection, out Vector3 pLeft, out Vector3 pRight)
    {
        pLeft = getLeftDoorSidePosition(pDirection);
        pRight = getLeftDoorSidePosition((pDirection + 1) % NumberOfDirections);
    }

    public void getDoorExtremities(int pRoomIndex, int pDirection, out Vector3 pLeft, out Vector3 pRight)
    {
        Vector3 lCenter = getRoomPosition(pRoomIndex);
        getDoorLocalExtremities(pDirection, out pLeft, out pRight);

        pLeft += lCenter;
        pRight += lCenter;
    }

    public Vector3 getDirectionOffset(int pDirection)
    {
        float lAngle = Mathf.Deg2Rad * (StartAngle + pDirection * StepOffset);
        return (new Vector3(Mathf.Cos(lAngle), 0, Mathf.Sin(lAngle)) * RoomsDistance * Balyrinth.Utilities.VIEW_SCALE);
    }

    public int getRoomIndexNonEuclidian(Vector3 pPosition, out Vector3 pOffset)
    {
        pOffset = Vector3.zero;
        return getRoomIndex(pPosition);
    }
}
