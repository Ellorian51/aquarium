using UnityEngine;

/// Контроллер аквариума — создаёт рыбу и хранит границы.
public class AquariumController : MonoBehaviour
{
    [Header("Границы аквариума")]
    public float leftLimit = -2.2f;
    public float rightLimit = 2.2f;
    public float bottomLimit = -1.3f;
    public float topLimit = 1.3f;

    [Header("Разные рыбы")]
    public GameObject[] fishPrefabs;

    /// <summary>
    /// Добавляет новую рыбу в аквариум.
    /// </summary>
    public void AddFish()
    {
        // случайная X позиция
        float x = Random.Range(leftLimit + 0.5f, rightLimit - 0.5f);

        // выбираем случайный префаб
        int idx = Random.Range(0, fishPrefabs.Length);
        GameObject prefab = fishPrefabs[idx];

        // проверка на донную рыбу
        bool isBottom = prefab.GetComponent<Fish>().bottomDweller;

        // случайная Y позиция
        float y = isBottom
            ? Random.Range(bottomLimit + 0.5f, bottomLimit + 0.8f)
            : Random.Range(bottomLimit + 0.7f, topLimit - 0.2f);

        Vector3 pos = new Vector3(x, y, 0f);

        // создаём объект
        GameObject fishObj = Instantiate(prefab, pos, Quaternion.identity, transform);

        Fish fishScript = fishObj.GetComponent<Fish>();
        if (fishScript != null)
        {
            fishScript.aquarium = this; // привязываем к аквариуму

            // случайное направление
            FishMovement movement = fishObj.GetComponent<FishMovement>();
            if (movement != null)
            {
                movement.startDirection = Random.value > 0.5f ? 1 : -1;
                movement.yOffsetSeed = Random.Range(0f, Mathf.PI * 2f);
            }
        }
    }
}