using UnityEngine;
using Random = UnityEngine.Random;

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
    public Plant[] plants;  // перетаскивай все Plant сюда

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

        // Назначаем Plant для MoveToPointBehavior
        MoveToPointBehavior mtp = fishObj.GetComponent<MoveToPointBehavior>();
        if (mtp != null && plants.Length > 0)
        {
            int plantIdx = Random.Range(0, plants.Length);
            mtp.plant = plants[plantIdx];
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