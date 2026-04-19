using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PathfindingModule
{
    public interface IGridProvider
    {
        int Width { get; }
        int Height { get; }
        GridCell GetCell(int x, int y);
        bool IsCellWalkable(int x, int y);
        float GetMovementCost(int x, int y);
    }
}
