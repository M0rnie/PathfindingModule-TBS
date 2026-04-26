using UnityEngine;

namespace PathfindingModule
{
    public class UnityGridProvider : MonoBehaviour, IGridProvider
    {
        [SerializeField] private int width = 10;
        [SerializeField] private int height = 10;
        [SerializeField] private float cellSize = 1f;
        [SerializeField] private GameObject cellPrefab;

        private GridCell[,] cells;
        private GameObject[,] visuals;

        public int Width => width;
        public int Height => height;

        void Awake()
        {
            GenerateGrid();
        }

        private void GenerateGrid()
        {
            cells = new GridCell[width, height];
            visuals = new GameObject[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    cells[x, y] = new GridCell
                    {
                        X = x,
                        Y = y,
                        IsWalkable = true,
                        MovementCost = 1f,
                        IsOccupied = false
                    };

                    if (cellPrefab != null)
                    {
                        Vector3 pos = new Vector3(x * cellSize, 0, y * cellSize);
                        visuals[x, y] = Instantiate(cellPrefab, pos, Quaternion.identity, transform);
                        visuals[x, y].name = $"Cell_{x}_{y}";
                    }
                }
            }
        }

        public GridCell GetCell(int x, int y)
        {
            if (x < 0 || x >= width || y < 0 || y >= height)
                return new GridCell { IsWalkable = false };
            return cells[x, y];
        }

        public bool IsCellWalkable(int x, int y) => GetCell(x, y).IsWalkable;
        public float GetMovementCost(int x, int y) => GetCell(x, y).MovementCost;

        // Новый метод: получить визуальный объект клетки
        public GameObject GetCellVisual(int x, int y)
        {
            if (x < 0 || x >= width || y < 0 || y >= height) return null;
            return visuals[x, y];
        }

        // Изменить проходимость клетки
        public void SetWalkable(int x, int y, bool walkable)
        {
            if (x < 0 || x >= width || y < 0 || y >= height) return;
            cells[x, y].IsWalkable = walkable;
            // Меняем цвет визуального объекта (серый/белый)
            var visual = visuals[x, y];
            if (visual != null)
            {
                var renderer = visual.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = walkable ? Color.white : Color.gray;
                }
            }
        }

        // Вспомогательный метод: изменить цвет клетки без изменения логики
        public void SetCellColor(int x, int y, Color color)
        {
            var visual = visuals[x, y];
            if (visual != null)
            {
                var renderer = visual.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = color;
                }
            }
        }

        // Сброс цвета всех клеток (кроме препятствий)
        public void ResetColors()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (cells[x, y].IsWalkable)
                        SetCellColor(x, y, Color.white);
                    else
                        SetCellColor(x, y, Color.gray);
                }
            }
        }
    }
}