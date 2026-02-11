using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class AquariumController : MonoBehaviour
{
    [Header("Границы аквариума")]
    public float leftLimit = -7f;
    public float rightLimit = 7f;
    public float bottomLimit = -4.5f;
    public float topLimit = 4f;

    // 🔥 ПРИВАТНЫЕ списки + публичные геттеры
    private List<FishMovement> _activeFishes = new List<FishMovement>();
    private Plant[] _plants = new Plant[0];
    
    public IReadOnlyList<FishMovement> activeFishes => _activeFishes;
    public Plant[] plants => _plants;

    // 🔥 Методы регистрации
    public void RegisterFish(FishMovement fish)
    {
        _activeFishes.Add(fish);
        Debug.Log($"🐟 Зарегистрирована рыба. Всего: {_activeFishes.Count}");
    }
    
    public void UnregisterFish(FishMovement fish)
    {
        _activeFishes.Remove(fish);
        Debug.Log($"🐟 Удалена рыба. Осталось: {_activeFishes.Count}");
    }
    
    public void RegisterPlant(Plant plant)
    {
        _plants = _plants.Append(plant).ToArray();
        Debug.Log($"🌿 Зарегистрировано растение '{plant.plantID}'. Всего: {_plants.Length}");
    }
    
    public void UnregisterPlant(Plant plant)
    {
        _plants = _plants.Where(p => p != plant).ToArray();
        Debug.Log($"🌿 Удалено растение. Осталось: {_plants.Length}");
    }
}