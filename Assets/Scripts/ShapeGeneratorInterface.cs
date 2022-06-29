using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public interface ShapeGeneratorInterface
{
    public int GetNumberOfDirections();

    public Labyrinth generate(int pWidth = 10, int pHeight = 10);

    public int getNextCellIndex(int pCurrentcellIndex, int pDirection);
    
    public int getOppositeDirection(int pDirection);

    public Vector3 getRoomPosition(int pIndex);

    public void getDoorExtremities(int pRoomIndex, int pDirection, out Vector3 pLeft, out Vector3 pRight);

    public Balyrinth.Utilities.LabyShape getLabyShape();

    public int getWidth();

    public int getHeight();
}
