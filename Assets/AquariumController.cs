using UnityEngine;

public class AquariumController : MonoBehaviour
{
    [Header("Границы аквариума")]
    public float leftLimit = -2.2f;
    public float rightLimit = 2.2f;  
    public float bottomLimit = -1.3f;
    public float topLimit = 1.3f;

    [Header("Разные рыбы")]
    public GameObject[] fishPrefabs;

    public void AddFish()
    {
        // 1. X случайный
        float x = Random.Range(leftLimit + 0.5f, rightLimit - 0.5f);
        
        // 2. ПРЕФАБ
        int idx = Random.Range(0, fishPrefabs.Length);
        GameObject prefab = fishPrefabs[idx];
        
        // 🔥 ПРОСТО: проверяем bottomDweller префаба
        bool isBottom = prefab.GetComponent<Fish>().bottomDweller;
        
        // 3. Y по типу
        float y;
        if (isBottom)
            y = Random.Range(bottomLimit + 0.5f, bottomLimit + 0.8f);  // Только у дна
        else
            y = Random.Range(bottomLimit + 0.7f, topLimit - 0.2f);    // ПО ВСЕЙ ВЫСОТЕ!
            
        Vector3 pos = new Vector3(x, y, 0);
        
        // 4. Создать
        GameObject fish = Instantiate(prefab, pos, Quaternion.identity, transform);
        
        Fish script = fish.GetComponent<Fish>();
        if (script != null)
        {
            // script.swimSpeed = ... // закомментировано
            script.startDirection = Random.value > 0.5f ? 1 : -1;
            script.yOffsetSeed = Random.Range(0f, 6.28f);
            script.aquarium = this;
        }
    }
}