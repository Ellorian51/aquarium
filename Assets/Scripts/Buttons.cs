using System;
using UnityEngine;
using UnityEngine.UI;

public class SpecificFishSpawner : MonoBehaviour
{
    [Header("Что спавнить")]
    public string fishType = "Fish Agro";  // Просто имя типа рыбы
    
    private Button _button;
    [SerializeField] private AquariumController aquarium;
    
    [Obsolete("Obsolete")]
    void Start()
    {
        _button = GetComponent<Button>();
        aquarium = FindObjectOfType<AquariumController>();  
    
        // ✅ УБРАЛИ вызов старого метода - кнопка теперь "пустая"
        Debug.Log($"✅ Кнопка '{fishType}' готова (OnClick через Inspector)");
    }

    void OnDestroy()
    {
        if (_button != null)
            _button.onClick.RemoveAllListeners();
    
    }
}