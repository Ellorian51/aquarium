using UnityEngine;  // Базовые классы Unity

public class AquariumController : MonoBehaviour
{
    [Header("Границы аквариума")]
    public float leftLimit = -2.2f;    // Левая стена
    public float rightLimit = 2.2f;    // Правая стена  
    public float bottomLimit = -1.3f;  // Дно
    public float topLimit = 1.3f;      // Поверхность

    [Header("Разные рыбы (перетащите префабы)")]
    public GameObject[] fishPrefabs;   // Массив: FishRed, FishBlue...

    // 🔥 КНОПКА вызывает ЭТО!
    public void AddFish()
    {
        // 1. Случайная позиция ВНУТРИ границ
        float x = Random.Range(leftLimit + 0.5f, rightLimit - 0.5f);
        float y = Random.Range(bottomLimit + 0.5f, topLimit - 0.5f);
        Vector3 pos = new Vector3(x, y, 0);
        
        // 2. СЛУЧАЙНЫЙ ПРЕФАБ РЫБЫ
        int randomFish = Random.Range(0, fishPrefabs.Length);
        GameObject fishPrefabRandom = fishPrefabs[randomFish];
        
        // 3. СОЗДАЁМ рыбу (дочерняя Aquarium)
        GameObject fish = Instantiate(fishPrefabRandom, pos, Quaternion.identity, transform);
        
        // 4. УНИКАЛЬНЫЕ ПАРАМЕТРЫ (скорость/направление)
        Fish script = fish.GetComponent<Fish>();
        if (script != null)
        {
            script.swimSpeed = Random.Range(0.8f, 1.8f);      // Скорость
            script.startDirection = Random.value > 0.5f ? 1 : -1;  // ←/→
            script.yOffsetSeed = Random.Range(0f, 6.28f);    // Волны Y
            script.aquarium = this;                          // Границы
        }
    }
}