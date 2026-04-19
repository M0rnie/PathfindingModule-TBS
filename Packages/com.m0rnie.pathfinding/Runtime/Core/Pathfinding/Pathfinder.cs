using System.Collections.Generic;
using UnityEngine;

namespace PathfindingModule
{
    public class Pathfinder
    {
        private IGridProvider grid;
        private bool allowDiagonal;

        public Pathfinder(IGridProvider grid, bool allowDiagonal = true)
        {
            this.grid = grid;
            this.allowDiagonal = allowDiagonal;
        }

        public List<GridCell> FindPath(int startX, int startY, int targetX, int targetY)
        {
            var cameFrom = new Dictionary<(int x, int y), (int x, int y)>();
            var costSoFar = new Dictionary<(int x, int y), float>();
            var frontier = new PriorityQueue<(int x, int y)>();

            frontier.Enqueue((startX, startY), 0);
            costSoFar[(startX, startY)] = 0;

            while (frontier.Count > 0)
            {
                var current = frontier.Dequeue();

                if (current.x == targetX && current.y == targetY)
                    break;

                foreach (var next in GetNeighbors(current.x, current.y))
                {
                    float newCost = costSoFar[current] + GetMoveCost(current.x, current.y, next.x, next.y);
                    if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
                    {
                        costSoFar[next] = newCost;
                        float priority = newCost + Heuristic(next.x, next.y, targetX, targetY);
                        frontier.Enqueue(next, priority);
                        cameFrom[next] = current;
                    }
                }
            }

            return ReconstructPath(cameFrom, startX, startY, targetX, targetY);
        }

        private List<GridCell> ReconstructPath(Dictionary<(int x, int y), (int x, int y)> cameFrom, int startX, int startY, int targetX, int targetY)
        {
            var path = new List<GridCell>();
            var current = (x: targetX, y: targetY); // явно именуем пол€ x и y
            if (!cameFrom.ContainsKey(current) && !(current.x == startX && current.y == startY))
                return path;

            while (!(current.x == startX && current.y == startY))
            {
                path.Add(grid.GetCell(current.x, current.y));
                current = cameFrom[current];
            }
            path.Add(grid.GetCell(startX, startY));
            path.Reverse();
            return path;
        }

        private IEnumerable<(int x, int y)> GetNeighbors(int x, int y)
        {
            // 4 направлени€
            if (x > 0) yield return (x - 1, y);
            if (x < grid.Width - 1) yield return (x + 1, y);
            if (y > 0) yield return (x, y - 1);
            if (y < grid.Height - 1) yield return (x, y + 1);

            // диагонали (если включены)
            if (allowDiagonal)
            {
                if (x > 0 && y > 0) yield return (x - 1, y - 1);
                if (x > 0 && y < grid.Height - 1) yield return (x - 1, y + 1);
                if (x < grid.Width - 1 && y > 0) yield return (x + 1, y - 1);
                if (x < grid.Width - 1 && y < grid.Height - 1) yield return (x + 1, y + 1);
            }
        }

        private float GetMoveCost(int fromX, int fromY, int toX, int toY)
        {
            bool diagonal = (fromX != toX && fromY != toY);
            float baseCost = diagonal ? 1.414f : 1f;
            return baseCost * grid.GetMovementCost(toX, toY);
        }

        private float Heuristic(int x, int y, int targetX, int targetY)
        {
            if (allowDiagonal)
                return Mathf.Max(Mathf.Abs(x - targetX), Mathf.Abs(y - targetY));
            else
                return Mathf.Abs(x - targetX) + Mathf.Abs(y - targetY);
        }
    }
}