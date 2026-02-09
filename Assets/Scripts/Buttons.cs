using UnityEngine;
using UnityEngine.UI;

public class SpecificFishSpawner : MonoBehaviour
{
    [Header("Настройки")]
    public AquariumController aquarium;
    [Range(0, 10)] public int fishIndex;  // № рыбы из fishPrefabs[]
    
    private Button button;
    
    void Start()
    {
        button = GetComponent<Button>();
        if (button == null)
        {
            Debug.LogError("❌ Button компонент не найден!");
            return;
        }
        
        button.onClick.AddListener(SpawnFish);
        Debug.Log($"✅ Кнопка готова спавнить рыбу #{fishIndex}");
    }
    
    void SpawnFish()
    {
        if (aquarium == null)
        {
            Debug.LogError("❌ Перетащи AquariumController!");
            return;
        }
        
        if (fishIndex >= aquarium.fishPrefabs.Length)
        {
            Debug.LogError($"❌ fishIndex {fishIndex} вне массива! Размер: {aquarium.fishPrefabs.Length}");
            return;
        }
        
        GameObject fishPrefab = aquarium.fishPrefabs[fishIndex];
        Instantiate(fishPrefab, aquarium.transform);
        Debug.Log($"🐟 Спавнена рыба #{fishIndex}: {fishPrefab.name}");
    }
    
    void OnDestroy()
    {
        if (button != null)
            button.onClick.RemoveListener(SpawnFish);
    }
}