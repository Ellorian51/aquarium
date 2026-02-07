using System;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Контроллер аквариума — создаёт рыбу и хранит границы.
/// </summary>
public class AquariumController : MonoBehaviour
{
    [Header("Границы аквариума")]
    public float leftLimit = -2.2f;
    public float rightLimit = 2.2f;
    public float bottomLimit = -1.3f;
    public float topLimit = 1.3f;

    [Header("Разные рыбы")]
    public GameObject[] fishPrefabs;

    /// Добавляет новую рыбу в аквариум
    [Obsolete("Obsolete")]
    public void AddFish()
    {
        // ===== 1. Случайная X позиция =====
        float x = Random.Range(leftLimit + 0.5f, rightLimit - 0.5f);

        // ===== 2. Выбор случайного префаба рыбы =====
        int idx = Random.Range(0, fishPrefabs.Length);
        GameObject prefab = fishPrefabs[idx];

        // ===== 3. Проверка на донную рыбу =====
        bool isBottom = prefab.GetComponent<Fish>().bottomDweller;

        // ===== 4. Случайная Y позиция =====
        float y = isBottom
            ? Random.Range(bottomLimit + 0.5f, bottomLimit + 0.8f)
            : Random.Range(bottomLimit + 0.7f, topLimit - 0.2f);

        Vector3 pos = new Vector3(x, y, 0f);

        // ===== 5. Создаём объект рыбы =====
        GameObject fishObj = Instantiate(prefab, pos, Quaternion.identity, transform);

        Fish fishScript = fishObj.GetComponent<Fish>();
        if (fishScript != null)
        {
            // ===== 6. Назначаем AquariumController =====
            fishScript.aquarium = this;

            // ===== 7. Назначаем растение для MoveToPointBehavior =====
            MoveToPointBehavior mtp = fishObj.GetComponent<MoveToPointBehavior>();
            if (mtp != null)
            {
                // Берём любое растение из сцены (или можно выбрать ближайшее)
                Plant plant = FindObjectOfType<Plant>();
                if (plant != null)
                {
                    mtp.plant = plant;
                }
                else
                {
                    Debug.LogWarning("В сцене нет Plant для MoveToPointBehavior!");
                }
            }

            // ===== 8. Настройка FishMovement =====
            FishMovement movement = fishObj.GetComponent<FishMovement>();
            if (movement != null)
            {
                movement.startDirection = Random.value > 0.5f ? 1 : -1;
                movement.yOffsetSeed = Random.Range(0f, Mathf.PI * 2f);
            }
        }
    }
}