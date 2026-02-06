using UnityEngine;

public class Fish : MonoBehaviour
{
    [Header("Скорость и направление")]
    public float swimSpeed = 1f;     // БАЗОВАЯ СКОРОСТЬ (из Inspector!)
    public int startDirection = 1;     // 1=→, -1=←

    [Header("Волны Y (асинхронность стаи)")]
    public float yOffsetSeed = 0f;     // ФАЗОВЫЙ СДВИГ

    [Header("ПОВЕДЕНИЕ ВИДА рыбы")]
    public bool bottomDweller = false; // TRUE=сом (по дну)

    // 🔥 ПЕРЕМЕННЫЕ для ЖИВОЙ скорости
    private float baseSwimSpeed;       // Запоминаем базовую
    private float speedVariation;      // Уникальная вариация рыбы
    private int direction = 1;
    private float yOffsetPhase;
    public AquariumController aquarium;

    void Start()
    {
        direction = startDirection;
        yOffsetPhase = yOffsetSeed + Random.Range(0f, Mathf.PI * 2f);
        
        // 🔥 ЖИВАЯ СКОРОСТЬ: уникальная личность + дыхание
        baseSwimSpeed = swimSpeed;                    // Из Inspector
        speedVariation = Random.Range(0.7f, 1.3f);    // ±30% уникальности
        
        if (aquarium == null)
            aquarium = GetComponentInParent<AquariumController>();
    }

    void Update()
    {
        // 🔥 ТЕКУЩАЯ скорость: база × личность × дыхание (±10%)
        float currentSpeed = baseSwimSpeed * speedVariation * (1f + Mathf.Sin(Time.time * 0.5f) * 0.1f);
        float moveX = direction * currentSpeed * Time.deltaTime;
        Vector3 newPos = transform.position + new Vector3(moveX, 0f, 0f);

        // Отскок X
        if (aquarium != null)
        {
            if (newPos.x > aquarium.rightLimit)
            {
                newPos.x = aquarium.rightLimit;
                direction = -1;
            }
            else if (newPos.x < aquarium.leftLimit)
            {
                newPos.x = aquarium.leftLimit;
                direction = 1;
            }
        }

        // Y позиция
        float yOffset;
        if (bottomDweller)
        {
            yOffset = aquarium.bottomLimit + 0.9f + Mathf.Sin(Time.time * 1f + yOffsetPhase) * 0.1f;
        }
        else
        {
            yOffset = Mathf.Sin(Time.time * 2f + yOffsetPhase) * 0.5f;
            if (aquarium != null)
                yOffset = Mathf.Clamp(yOffset, aquarium.bottomLimit, aquarium.topLimit);
        }
        newPos.y = yOffset;

        transform.position = newPos;
        
        // Разворот спрайта
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * (direction > 0 ? 1 : -1);
        transform.localScale = scale;
    }
}
