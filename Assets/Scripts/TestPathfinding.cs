using UnityEngine;
using PathfindingModule; // подключаем наш модуль

public class TestPathfinding : MonoBehaviour
{
    void Start()
    {
        // Ищем на том же объекте, к которому прикреплён скрипт, компонент UnityGridProvider
        UnityGridProvider gridProvider = GetComponent<UnityGridProvider>();

        // Если компонент не найден, выводим ошибку и выходим
        if (gridProvider == null)
        {
            Debug.LogError("UnityGridProvider not found! Attach this script to the GridManager.");
            return;
        }

        // Создаём экземпляр поисковика путей, разрешаем диагональное движение
        Pathfinder pathfinder = new Pathfinder(gridProvider, allowDiagonal: true);

        // Ищем путь от клетки (0,0) до клетки (5,5)
        var path = pathfinder.FindPath(0, 0, 5, 5);

        // Выводим в консоль длину пути
        Debug.Log($"Path found: {path.Count} cells");

        // Рисуем красные линии между соседними клетками пути
        for (int i = 0; i < path.Count - 1; i++)
        {
            var from = path[i];
            var to = path[i + 1];
            // Отладочная линия, видна в окне Scene (не в Game) во время игры и 10 секунд после остановки
            Debug.DrawLine(
                new Vector3(from.X, 0.5f, from.Y),
                new Vector3(to.X, 0.5f, to.Y),
                Color.red,
                10f
            );
        }
    }
}