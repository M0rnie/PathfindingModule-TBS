using UnityEngine;
using PathfindingModule;

public class PathfindingInputHandler : MonoBehaviour
{
    private UnityGridProvider grid;
    private Pathfinder pathfinder;

    private Vector2Int? startCell = null;
    private Vector2Int? targetCell = null;

    void Start()
    {
        grid = GetComponent<UnityGridProvider>();
        if (grid == null)
        {
            Debug.LogError("PathfindingInputHandler needs a UnityGridProvider on the same object!");
            return;
        }
        pathfinder = new Pathfinder(grid, allowDiagonal: true);
    }

    void Update()
    {
        if (grid == null) return;

        // Получаем клетку под курсором мыши
        if (Camera.main == null) return;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f))
        {
            // Ищем, по какому кубику мы попали
            GameObject hitObject = hit.collider.gameObject;
            // Находим координаты из имени (формат "Cell_x_y")
            string[] parts = hitObject.name.Split('_');
            if (parts.Length == 3 && parts[0] == "Cell")
            {
                int x = int.Parse(parts[1]);
                int y = int.Parse(parts[2]);

                // Левый клик: установка старта или цели
                if (Input.GetMouseButtonDown(0))
                {
                    if (startCell == null)
                    {
                        // Устанавливаем старт
                        SetStart(x, y);
                    }
                    else if (targetCell == null)
                    {
                        // Устанавливаем цель и строим путь
                        SetTarget(x, y);
                        FindAndVisualizePath();
                    }
                    else
                    {
                        // Если старт и цель уже есть, сбрасываем и начинаем заново
                        ResetSelection();
                        SetStart(x, y);
                    }
                }
                // Правый клик: установка/снятие препятствия
                else if (Input.GetMouseButtonDown(1))
                {
                    bool currentWalkable = grid.IsCellWalkable(x, y);
                    grid.SetWalkable(x, y, !currentWalkable);
                    // Если устанавливаем препятствие на старте или цели, сбрасываем выбор
                    if ((startCell.HasValue && startCell.Value.x == x && startCell.Value.y == y) ||
                        (targetCell.HasValue && targetCell.Value.x == x && targetCell.Value.y == y))
                    {
                        ResetSelection();
                    }
                    else
                    {
                        // Если путь уже построен, перестраиваем
                        if (startCell.HasValue && targetCell.HasValue)
                            FindAndVisualizePath();
                        else
                            grid.ResetColors();
                    }
                }
            }
        }
    }

    private void SetStart(int x, int y)
    {
        if (!grid.IsCellWalkable(x, y)) return;
        startCell = new Vector2Int(x, y);
        grid.ResetColors();
        grid.SetCellColor(x, y, Color.green);
    }

    private void SetTarget(int x, int y)
    {
        if (!grid.IsCellWalkable(x, y)) return;
        targetCell = new Vector2Int(x, y);
        grid.SetCellColor(x, y, Color.blue);
    }

    private void ResetSelection()
    {
        startCell = null;
        targetCell = null;
        grid.ResetColors();
    }

    private void FindAndVisualizePath()
    {
        if (!startCell.HasValue || !targetCell.HasValue) return;

        var path = pathfinder.FindPath(startCell.Value.x, startCell.Value.y, targetCell.Value.x, targetCell.Value.y);

        // Сначала сбрасываем цвета (кроме старта и цели, они будут перекрашены)
        grid.ResetColors();
        // Заново подсвечиваем старт и цель
        grid.SetCellColor(startCell.Value.x, startCell.Value.y, Color.green);
        grid.SetCellColor(targetCell.Value.x, targetCell.Value.y, Color.blue);

        // Если путь найден и содержит больше 2 клеток (старт+цель или более), подсвечиваем промежуточные
        if (path.Count > 0)
        {
            foreach (var cell in path)
            {
                // Не перекрашиваем старт и цель
                if ((cell.X == startCell.Value.x && cell.Y == startCell.Value.y) ||
                    (cell.X == targetCell.Value.x && cell.Y == targetCell.Value.y))
                    continue;
                grid.SetCellColor(cell.X, cell.Y, Color.yellow);
            }
            Debug.Log($"Path found: {path.Count} cells");
        }
        else
        {
            Debug.Log("No path found!");
        }
    }
}