using UnityEngine;

/// <summary>
/// Скрипт для растения: хранит точки кормежки для рыб.
/// </summary>
public class Plant : MonoBehaviour
{
    [Header("Точки кормежки")]
    public Transform[] feedingPoints; // сюда через инспектор добавляем точки внутри префаба
    
    /// Возвращает случайную точку кормежки
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