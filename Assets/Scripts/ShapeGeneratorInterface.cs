using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface ShapeGeneratorInterface
{
    public int GetNumberOfDirections();

    public Labyrinth generate(int pWidth = 10, int pHeight = 10);

    public int getNextCellIndex(int pCurrentcellIndex, int pDirection);
}
