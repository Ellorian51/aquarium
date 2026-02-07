using UnityEngine;

/// Скрипт для растения: хранит точки кормежки для рыб.
public class Plant : MonoBehaviour
{
    [Header("Точки кормежки")]
    public Transform[] feedingPoints; // сюда через инспектор добавляем точки внутри префаба
    
    /// Возвращает случайную точку кормежки.
    /// Пока одна точка — возвращает её.
    public Transform GetRandomFeedingPoint()
    {
        if (feedingPoints == null || feedingPoints.Length == 0)
        {
            Debug.LogWarning("У растения нет точек кормежки!");
            return null;
        }

        int index = Random.Range(0, feedingPoints.Length);
        return feedingPoints[index];
    }
}