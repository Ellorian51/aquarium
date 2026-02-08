using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Скрипт для растения: хранит точки кормежки для рыб.
/// </summary>
public class Plant : MonoBehaviour
{
    [Header("🆔 ID СИСТЕМА")]
    [SerializeField] public string plantID = "Plant1";  // Plant1, Plant2, Plant3...
    [Header("Точки кормежки")]
    public Transform[] feedingPoints; // сюда через инспектор добавляем точки внутри префаба
    
    /// Возвращает случайную точку кормежки
    public Transform GetRandomFeedingPoint()
    {
        if (feedingPoints == null || feedingPoints.Length == 0)
        {
            Debug.LogWarning($"🌿 {plantID} ({name}): нет feedingPoints!");
            return null;
        }

        int index = Random.Range(0, feedingPoints.Length);
        Debug.Log($"🌿 {plantID}: выбрана точка #{index}");
        return feedingPoints[index];
    }
}