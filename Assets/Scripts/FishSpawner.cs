using UnityEngine;
using System.Linq;
using Random = UnityEngine.Random;
using UnityEngine.UI;

public class FishSpawner : MonoBehaviour
{
    [Header("Префабы рыб")]
    public GameObject[] fishPrefabs;
    
    [Header("Аквариум (обязательно!)")]
    public AquariumController aquarium;
    
    [Header("Тип рыбы для кнопки")]
    public string fishType = "Fish Agro";

    private Button _button;

    void Awake()
    {
        // ✅ Инициализация ДО Start() всех скриптов
        _button = GetComponent<Button>();
        aquarium = aquarium ?? FindFirstObjectByType<AquariumController>();
    }

    void Start()
    {
        if (_button == null)
        {
            Debug.LogWarning("⚠️ FishSpawner без Button — пропускаем");
            return;
        }
    
        if (aquarium == null)
        {
            Debug.LogError("❌ AquariumController не найден!");
            return;
        }
    
        // ✅ Теперь безопасно настраиваем кнопку
        _button.onClick.RemoveAllListeners(); // Очищаем старые
        _button.onClick.AddListener(() => SpawnSpecificFish(fishType));
        Debug.Log($"✅ Кнопка '{fishType}' готова (Aquarium: {aquarium.name})");
    }

    void OnDestroy()
    {
        _button?.onClick.RemoveAllListeners();
    }

    public void SpawnSpecificFish(string fishName)
    {
        var prefab = fishPrefabs.FirstOrDefault(f => f.name.Contains(fishName));
        if (prefab == null)
        {
            Debug.LogError($"🐟 Префаб '{fishName}' не найден! Доступно: {string.Join(", ", fishPrefabs.Select(p => p.name))}");
            return;
        }
        SpawnFishPrefab(prefab);
    }

    private void SpawnFishPrefab(GameObject prefab)
    {
        float x = Random.Range(aquarium.leftLimit + 0.5f, aquarium.rightLimit - 0.7f);
        Fish fishComp = prefab.GetComponent<Fish>();
        bool isBottom = fishComp != null ? fishComp.bottomDweller : false;
        float y = isBottom
            ? Random.Range(aquarium.bottomLimit + 0.5f, aquarium.bottomLimit + 0.8f)
            : Random.Range(aquarium.bottomLimit + 0.7f, aquarium.topLimit - 0.2f);

        GameObject fishObj = Instantiate(prefab, new Vector3(x, y, 0), Quaternion.identity, aquarium.transform);
        
        Fish fish = fishObj.GetComponent<Fish>();
        if (fish != null) fish.aquarium = aquarium;
        
        FishMovement fishMovement = fishObj.GetComponent<FishMovement>();
        if (fishMovement != null)
        {
            aquarium.RegisterFish(fishMovement);
            fishMovement.startDirection = Random.value > 0.5f ? 1 : -1;
            fishMovement.yOffsetSeed = Random.Range(0f, Mathf.PI * 2f);
        }

        Debug.Log($"🐟 {fishObj.name} заспавнена!");
    }
}
