using UnityEngine;
using Random = UnityEngine.Random;
using System.Linq;  // для Array.Find

/// <summary>
/// Контроллер аквариума: создаёт рыбу, управляет границами.
/// </summary>
public class AquariumController : MonoBehaviour
{
    [Header("Границы аквариума")]
    public float leftLimit = -7f;
    public float rightLimit = 7f;
    public float bottomLimit = -4.5f;
    public float topLimit = 4f;

    [Header("Пресеты рыб")]
    public GameObject[] fishPrefabs;

    [Header("Растения для кормежки")]
    public Plant[] plants;  // перетаскивай все Plant ИЗ HIERARCHY сюда

    /// Создаёт новую рыбу
    public void AddFish()
    {
        if (fishPrefabs.Length == 0) return;

        int idx = Random.Range(0, fishPrefabs.Length);
        GameObject prefab = fishPrefabs[idx];

        bool isBottom = prefab.GetComponent<Fish>().bottomDweller;
        float x = Random.Range(leftLimit + 0.5f, rightLimit - 0.7f);
        float y = isBottom
            ? Random.Range(bottomLimit + 0.5f, bottomLimit + 0.8f)
            : Random.Range(bottomLimit + 0.7f, topLimit - 0.2f);

        GameObject fishObj = Instantiate(prefab, new Vector3(x, y, 0), Quaternion.identity, transform);

        // Связь с Aquarium
        Fish fish = fishObj.GetComponent<Fish>();
        if (fish != null) fish.aquarium = this;

        // 🔥 ID СИСТЕМА — Назначаем Plant для MoveToPointBehavior
        MoveToPointBehavior mtp = fishObj.GetComponent<MoveToPointBehavior>();
        if (mtp != null && plants != null && plants.Length > 0)
        {
            Plant targetPlant;
            
            // ✅ ПРИОРИТЕТ #1: любимое растение по ID
            if (!string.IsNullOrEmpty(fish.favoritePlantID))
            {
                targetPlant = plants.FirstOrDefault(p => p.plantID == fish.favoritePlantID);
                if (targetPlant != null)
                {
                    mtp.plant = targetPlant;
                    Debug.Log($"🐟 {fishObj.name} → ЛЮБИМОЕ {targetPlant.plantID}");
                }
                else
                {
                    Debug.LogWarning($"🐟 {fishObj.name} НЕ НАЙДЕН PLANT ID '{fish.favoritePlantID}'!");
                }
            }
            // ✅ ПРИОРИТЕТ #2: рандом из массива plants
            else
            {
                int plantIdx = Random.Range(0, plants.Length);
                targetPlant = plants[plantIdx];
                mtp.plant = targetPlant;
                Debug.Log($"🐟 {fishObj.name} → РАНДОМ {targetPlant.plantID} (#{plantIdx})");
            }
        }
        else
        {
            Debug.LogWarning($"🐟 {fishObj.name} нет MoveToPointBehavior или plants[] пуст!");
        }

        // Настройка FishMovement
        FishMovement movement = fishObj.GetComponent<FishMovement>();
        if (movement != null)
        {
            movement.startDirection = Random.value > 0.5f ? 1 : -1;
            movement.yOffsetSeed = Random.Range(0f, Mathf.PI * 2f);
        }
    }
}
