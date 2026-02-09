using UnityEngine;
using Random = UnityEngine.Random;
using System.Linq;

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
    public Plant[] plants;

    private const float RiseSpeed = 0.08f;  // 🔥 Приватная константа подъёма

    void Update()
    {
        // 🔥 ПРИВАТНЫЙ ПОДЪЁМ всех бездействующих рыб
        RiseInactiveFish();
    }

    private void RiseInactiveFish()
    {
        foreach (Transform child in transform)
        {
            if (!child.gameObject.activeInHierarchy) continue;
            
            FishMovement fm = child.GetComponent<FishMovement>();
            if (fm == null) continue;
            
            // Поднимаем ТОЛЬКО полностью бездействующих
            if (!fm.IsActiveMovement())
            {
                Vector3 pos = child.position;
                pos.y += RiseSpeed * Time.deltaTime;
                pos.y = Mathf.Clamp(pos.y, bottomLimit + 0.5f, topLimit - 0.2f);
                child.position = pos;
            }
        }
    }

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

        Fish fish = fishObj.GetComponent<Fish>();
        if (fish != null) fish.aquarium = this;

        MoveToPointBehavior mtp = fishObj.GetComponent<MoveToPointBehavior>();
        if (mtp != null && plants != null && plants.Length > 0)
        {
            Plant targetPlant = null;
            
            if (!string.IsNullOrEmpty(fish.favoritePlantID))
            {
                string[] favoriteIDs = fish.favoritePlantID.Split(',');
                foreach (string id in favoriteIDs)
                {
                    targetPlant = plants.FirstOrDefault(p => p.plantID.Trim() == id.Trim());
                    if (targetPlant != null) break;
                }
                
                if (targetPlant != null)
                {
                    mtp.plant = targetPlant;
                    Debug.Log($"🐟 {fishObj.name} → ЛЮБИМОЕ {targetPlant.plantID}");
                }
            }
            
            if (targetPlant == null)
            {
                int plantIdx = Random.Range(0, plants.Length);
                targetPlant = plants[plantIdx];
                mtp.plant = targetPlant;
                Debug.Log($"🐟 {fishObj.name} → РАНДОМ {targetPlant.plantID}");
            }
        }

        FishMovement movement = fishObj.GetComponent<FishMovement>();
        if (movement != null)
        {
            movement.startDirection = Random.value > 0.5f ? 1 : -1;
            movement.yOffsetSeed = Random.Range(0f, Mathf.PI * 2f);
        }
    }
}