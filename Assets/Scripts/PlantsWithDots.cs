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

    // 🔥 КЭШ аквариума (1 раз!)
    private AquariumController aquarium;

    public delegate void PlantDestroyed();
    public event PlantDestroyed OnPlantDestroyed;

    void Awake()
    {
        energy = maxEnergy;
    }

    void Start()  // 🔥 ДОБАВЛЕН: САМ регистрируется!
    {
        aquarium = FindFirstObjectByType<AquariumController>();
        if (aquarium != null)
        {
            aquarium.RegisterPlant(this);
            Debug.Log($"🌿 '{plantID}' САМ зарегистрировался в AC");
        }
        else
        {
            Debug.LogWarning($"🌿 '{plantID}' НЕ НАШЁЛ AC!");
        }
    }

    // Метод для потребления энергии рыбой
    public bool TryConsume(float amount = 1f)
    {
        if (energy <= 0f) return false;

        energy -= amount;
        if (energy <= 0f)
        {
            energy = 0f;
            Destroy(gameObject);
            OnPlantDestroyed?.Invoke();
        }

        return true;
    }

    public Transform GetRandomFeedingPoint()
    {
        if (feedingPoints == null || feedingPoints.Length == 0) return null;
        return feedingPoints[Random.Range(0, feedingPoints.Length)];
    }

    void OnDestroy()
    {
        if (aquarium != null)
        {
            aquarium.UnregisterPlant(this);
            Debug.Log($"🌿 '{plantID}' САМ удалился из AC");
        }
    }
}