using UnityEngine;

/// <summary>
/// Сущность рыбы — хранит тип и связь с аквариумом.
/// </summary>
public class Fish : MonoBehaviour
{
    [Header("ВИД РЫБЫ")]
    public bool bottomDweller;  // донная или нет
    public bool isAggressive;   // ✅ АГРЕССИВНАЯ!

    [Header("Кормежка")]
    public Plant favoritePlant; // своё растение (опционально)

    [HideInInspector]
    public AquariumController aquarium;

    void Start()
    {
        if (aquarium == null)
            aquarium = GetComponentInParent<AquariumController>();
    }
}