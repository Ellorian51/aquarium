using System;
using UnityEngine;
using System.Linq;

/// Спавн растений в заранее подготовленные слоты.
/// При спавне новое растение добавляется в AquariumController.plants
public class PlantSpawner : MonoBehaviour
{
    [Header("Слоты для растений на сцене")]
    public Transform[] plantSlots; // Пустые объекты для точек спавна

    [Header("Доступные префабы растений")]
    public GameObject[] plantPrefabs; // Префабы растений

    /// <summary>
    /// Спавн нового растения в указанном слоте
    /// </summary>
    [Obsolete("Obsolete")]
    public void SpawnPlant(int prefabIndex)
    {
        if (plantSlots == null || plantSlots.Length == 0)
        {
            Debug.LogWarning("Нет слотов для спавна растений!");
            return;
        }

        if (plantPrefabs == null || plantPrefabs.Length == 0)
        {
            Debug.LogWarning("Нет доступных префабов растений!");
            return;
        }

        if (prefabIndex < 0 || prefabIndex >= plantPrefabs.Length)
        {
            Debug.LogWarning($"Префаб с индексом {prefabIndex} не существует!");
            return;
        }

        // Ищем первый свободный слот
        Transform slot = plantSlots.FirstOrDefault(s => s.childCount == 0);
        if (slot == null)
        {
            Debug.LogWarning("Нет свободных слотов для нового растения!");
            return;
        }

        // Создаем объект растения
        GameObject plantObj = Instantiate(plantPrefabs[prefabIndex], slot.position, Quaternion.identity, slot);

        // Получаем компонент Plant
        Plant newPlant = plantObj.GetComponent<Plant>();
        if (newPlant == null)
        {
            Debug.LogError("Префаб растения не содержит компонент Plant!");
            return;
        }
        Debug.Log($"🌿 Растение '{newPlant.plantID}' заспавнено в слоте #{slot.GetSiblingIndex()}");
    }
}