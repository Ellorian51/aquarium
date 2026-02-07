using UnityEngine;

/// <summary>
/// Сущность рыбы — хранит тип и связь с аквариумом.
/// Не отвечает за движение.
/// </summary>
public class Fish : MonoBehaviour
{
    [Header("ВИД РЫБЫ")]
    public bool bottomDweller = false;   // донная или нет

    [HideInInspector]
    public AquariumController aquarium;

    void Start()
    {
        if (aquarium == null)
            aquarium = GetComponentInParent<AquariumController>();
    }
}