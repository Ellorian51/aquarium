using UnityEngine;

public class AquariumController : MonoBehaviour
{
    [Header("Границы аквариума")]
    public float leftLimit = -2.2f;
    public float rightLimit = 2.2f;
    public float bottomLimit = -1.3f;
    public float topLimit = 1.3f;

    [Header("Спавн рыб")]
    public GameObject fishPrefab;  // Перетащили prefab

    [ContextMenu("🐟 Добавить рыбу")]  // Правой кнопка → спавн!
    public void AddFish()
    {
        float x = Random.Range(leftLimit + 0.5f, rightLimit - 0.5f);
        float y = Random.Range(bottomLimit + 0.5f, topLimit - 0.5f);
        Vector3 pos = new Vector3(x, y, 0);
        
        GameObject fish = Object.Instantiate(fishPrefab, pos, Quaternion.identity, transform);
        
        Fish script = fish.GetComponent<Fish>();
        if (script != null)
        {
            script.swimSpeed = Random.Range(0.8f, 1.8f);
            script.startDirection = Random.value > 0.5f ? 1 : -1;
            script.yOffsetSeed = Random.Range(0f, 6.28f);
            script.aquarium = this;
        }
    }
}