using UnityEngine;
using Random = UnityEngine.Random;

/// Скрипт для растения: хранит точки кормежки для рыб.
public class Plant : MonoBehaviour
{
    [Header("🆔 ID СИСТЕМА")]
    public string plantID = "Plant1";
    public Transform[] feedingPoints;

    [Header("Энергия растения")]
    public float maxEnergy = 10f;
    public float energy;

    public delegate void PlantDestroyed();
    public event PlantDestroyed OnPlantDestroyed;

    void Awake()
    {
        energy = maxEnergy;
    }

    // Метод для потребления энергии рыбой
    public bool TryConsume(float amount = 1f)
    {
        if (energy <= 0f) return false;

        energy -= amount;
        if (energy <= 0f)
        {
            energy = 0f;
            // Растение исчезает
            Destroy(gameObject);
            OnPlantDestroyed?.Invoke(); // уведомляем слот, что можно освободить
        }

        return true;
    }

    public Transform GetRandomFeedingPoint()
    {
        if (feedingPoints == null || feedingPoints.Length == 0) return null;
        return feedingPoints[Random.Range(0, feedingPoints.Length)];
    }
}