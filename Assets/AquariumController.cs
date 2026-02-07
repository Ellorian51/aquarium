using System;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Контроллер аквариума: создаёт рыбу, управляет границами.
/// </summary>
public class AquariumController : MonoBehaviour
{
    [Header("Границы аквариума")]
    public float leftLimit = -2.2f;
    public float rightLimit = 2.2f;
    public float bottomLimit = -1.3f;
    public float topLimit = 1.3f;

    [Header("Пресеты рыб")]
    public GameObject[] fishPrefabs;

    /// Создаёт новую рыбу
    [Obsolete("Obsolete")]
    public void AddFish()
    {
        if (fishPrefabs.Length == 0) return;

        int idx = Random.Range(0, fishPrefabs.Length);
        GameObject prefab = fishPrefabs[idx];

        bool isBottom = prefab.GetComponent<Fish>().bottomDweller;
        float x = Random.Range(leftLimit + 0.5f, rightLimit - 0.5f);
        float y = isBottom
            ? Random.Range(bottomLimit + 0.5f, bottomLimit + 0.8f)
            : Random.Range(bottomLimit + 0.7f, topLimit - 0.2f);

        GameObject fishObj = Instantiate(prefab, new Vector3(x, y, 0), Quaternion.identity, transform);

        // Связь с Aquarium
        Fish fish = fishObj.GetComponent<Fish>();
        if (fish != null) fish.aquarium = this;

        // Назначаем Plant для MoveToPointBehavior
        MoveToPointBehavior mtp = fishObj.GetComponent<MoveToPointBehavior>();
        if (mtp != null)
        {
            Plant plant = FindObjectOfType<Plant>(); // выбираем любое растение из сцены
            if (plant != null)
                mtp.plant = plant;
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