using UnityEngine;

/// <summary>
/// Спавн растений в подготовленные слоты.
/// </summary>
public class PlantSpawner : MonoBehaviour
{
    [Header("Слоты для растений")]
    public Transform[] plantSlots; // Пустые объекты в сцене, расставленные вручную

    [Header("Доступные префабы растений")]
    public GameObject[] plantPrefabs; // Префабы растений, уже с plantID и feedingPoints

    // Следим, какие слоты заняты
    private bool[] _slotOccupied;

    void Awake()
    {
        // Инициализируем массив занятости слотов
        _slotOccupied = new bool[plantSlots.Length];
        for (int i = 0; i < _slotOccupied.Length; i++)
            _slotOccupied[i] = false; // Все слоты свободны
    }

    /// <summary>
    /// Спавн нового растения в свободный слот.
    /// </summary>
    public void SpawnPlant(int index)
    {
        if (plantSlots.Length == 0 || plantPrefabs.Length == 0)
        {
            Debug.LogWarning("Нет слотов или префабов для спавна!");
            return;
        }

        if (index < 0 || index >= plantPrefabs.Length)
        {
            Debug.LogWarning("Неверный индекс префаба растения!");
            return;
        }

        // Ищем первый свободный слот
        int slotIndex = -1;
        for (int i = 0; i < _slotOccupied.Length; i++)
        {
            if (!_slotOccupied[i])
            {
                slotIndex = i;
                break;
            }
        }

        if (slotIndex == -1)
        {
            Debug.LogWarning("Нет свободных слотов для нового растения!");
            return;
        }

        // Спавним растение в выбранный слот
        GameObject plantObj = Instantiate(
            plantPrefabs[index],
            plantSlots[slotIndex].position,
            Quaternion.identity,
            transform // можно сделать дочерним объектом спавнера для чистоты
        );

        // Слот теперь занят
        _slotOccupied[slotIndex] = true;

        // Сохраняем ссылку на слот в самом растении, чтобы освободить позже
        Plant plantComp = plantObj.GetComponent<Plant>();
        if (plantComp != null)
        {
            plantComp.OnPlantDestroyed += () => _slotOccupied[slotIndex] = false;
        }

        Debug.Log($"🌿 Растение '{plantObj.name}' заспавнено в слоте #{slotIndex}");
    }
}
