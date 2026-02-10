using UnityEngine;
using Random = UnityEngine.Random;
using System.Linq;
using System.Collections.Generic;

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
    public Plant[] plants; // сюда попадают растения на сцене

    private List<FishMovement> activeFishes = new List<FishMovement>();

    // 🔥 СТАРЫЙ МЕТОД: рандомный спавн
    public void AddFish()
    {
        if (fishPrefabs.Length == 0) return;

        int idx = Random.Range(0, fishPrefabs.Length);
        GameObject prefab = fishPrefabs[idx];
        SpawnFishInstance(prefab);
    }

    // 🔥 НОВЫЙ МЕТОД: спавн по имени для кнопок
    public void SpawnSpecificFish(string fishType)
    {
        GameObject prefab = fishPrefabs.FirstOrDefault(f => f.name.Contains(fishType));
        
        if (prefab == null)
        {
            Debug.LogError($"🐟 Префаб '{fishType}' не найден!");
            return;
        }
        
        SpawnFishInstance(prefab);
    }

    // 🔥 ОБЩИЙ ПРИВАТНЫЙ МЕТОД: вся логика спавна
    private void SpawnFishInstance(GameObject prefab)
    {
        Fish prefabFish = prefab.GetComponent<Fish>();
        bool isBottom = prefabFish != null ? prefabFish.bottomDweller : false;
        
        float x = Random.Range(leftLimit + 0.5f, rightLimit - 0.7f);
        float y = isBottom
            ? Random.Range(bottomLimit + 0.5f, bottomLimit + 0.8f)
            : Random.Range(bottomLimit + 0.7f, topLimit - 0.2f);

        GameObject fishObj = Instantiate(prefab, new Vector3(x, y, 0), Quaternion.identity, transform);

        Fish fish = fishObj.GetComponent<Fish>();
        if (fish == null) 
        {
            Debug.LogError($"🐟 {prefab.name} без Fish компонента!");
            return;
        }
        fish.aquarium = this;

        // Кэшируем FishMovement
        FishMovement fishMovement = fishObj.GetComponent<FishMovement>();
        if (fishMovement != null)
        {
            activeFishes.Add(fishMovement);
            Debug.Log($"🐟 Добавлена в список. Всего рыб: {activeFishes.Count}");
            
            fishMovement.startDirection = Random.value > 0.5f ? 1 : -1;
            fishMovement.yOffsetSeed = Random.Range(0f, Mathf.PI * 2f);
        }

        // Логика растений для MoveToPointBehavior
        MoveToPointBehavior mtp = fishObj.GetComponent<MoveToPointBehavior>();
        if (mtp != null)
        {
            Plant targetPlant = null;

            // 1️⃣ Сначала пробуем любимое растение
            if (fish.favoritePlants != null && fish.favoritePlants.Count > 0)
            {
                string favoriteID = fish.favoritePlants[Random.Range(0, fish.favoritePlants.Count)];
                targetPlant = plants.FirstOrDefault(p => p != null && p.plantID.Trim() == favoriteID.Trim());
                
                if (targetPlant != null)
                {
                    mtp.plant = targetPlant;
                    Debug.Log($"🐟 {fishObj.name} → ЛЮБИМОЕ {targetPlant.plantID}");
                }
            }

            // 2️⃣ Если любимое не найдено — берем рандом из растений на сцене
            if (targetPlant == null && plants != null && plants.Length > 0)
            {
                targetPlant = plants.FirstOrDefault(p => p != null);
                if (targetPlant != null)
                {
                    mtp.plant = targetPlant;
                    Debug.Log($"🐟 {fishObj.name} → РАНДОМ {targetPlant.plantID}");
                }
            }
        }
    }

    // 🔹 Этот метод нужно вызвать после спавна нового растения, чтобы рыба его видела
    public void RegisterPlantOnScene(Plant newPlant)
    {
        if (plants == null) plants = new Plant[0];
        List<Plant> temp = plants.ToList();
        temp.Add(newPlant);
        plants = temp.ToArray();
    }
}