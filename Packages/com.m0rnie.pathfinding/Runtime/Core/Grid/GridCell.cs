using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PathfindingModule
{
    public struct GridCell
    {
        public int X;
        public int Y;
        public bool IsWalkable;
        public float MovementCost;
        public bool IsOccupied;
    }
}
