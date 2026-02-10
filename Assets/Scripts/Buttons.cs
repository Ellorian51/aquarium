using UnityEngine;
using UnityEngine.UI;

public class SpecificFishSpawner : MonoBehaviour
{
    [Header("Что спавнить")]
    public string fishType = "Fish Agro";  // Просто имя типа рыбы
    
    private Button _button;
    private AquariumController aquarium;
    
    void Start()
    {
        _button = GetComponent<Button>();
        aquarium = FindObjectOfType<AquariumController>();  // Находит сам
        
        _button.onClick.AddListener(() => aquarium.SpawnSpecificFish(fishType));
        Debug.Log($"✅ Кнопка '{fishType}' готова");
    }
    
    void OnDestroy()
    {
        if (_button != null)
            _button.onClick.RemoveListener(() => aquarium?.SpawnSpecificFish(fishType));
    }
}