using UnityEngine;  // Всё базовое Unity (Vector3, Time, Random)

public class Fish : MonoBehaviour  // Этот скрипт на КАЖДОЙ рыбе
{
    // ========================================
    // ПАРАМЕТРЫ (видны в Inspector префаба!)
    // ========================================
    [Header("Скорость и направление")]
    public float swimSpeed = 1f;       // СКОРОСТЬ плавания (0.5=медленно, 2=быстро)
    public int startDirection = 1;     // НАЧАЛЬНОЕ направление: 1=→, -1=←

    [Header("Волны Y (асинхронность стаи)")]
    public float yOffsetSeed = 0f;     // ФАЗОВЫЙ СДВИГ: 0=синхронно, 1.5=разные волны

    [Header("ПОВЕДЕНИЕ ВИДА рыбы")]
    public bool bottomDweller = false; // TRUE=сом (по дну), FALSE=обычная (волны)

    // ========================================
    // ПЕРЕМЕННЫЕ (скрытые, Unity управляет)
    // ========================================
    public AquariumController aquarium; // ССЫЛКА на родителя (границы)
    private int direction = 1;          // ТЕКУЩЕЕ направление (меняется при отскоке)
    private float yOffsetPhase;         // ФАЗА волны Y (уникальна для рыбы)

    // ========================================
    // START: настройка 1 раз при рождении
    // ========================================
    void Start()
    {
        direction = startDirection;  // Берём из Inspector
        yOffsetPhase = yOffsetSeed + Random.Range(0f, Mathf.PI * 2f);  // +случайность
        
        // Ищем аквариум-родителя (если не назначен)
        if (aquarium == null)
            aquarium = GetComponentInParent<AquariumController>();
    }

    // ========================================
    // UPDATE: движение КАЖДЫЙ КАДР (60 раз/сек)
    // ========================================
    void Update()
    {
        // 1. ДВИЖЕНИЕ ПО X (горизонталь)
        float moveX = direction * swimSpeed * Time.deltaTime;  // СКОРОСТЬ * время (плавно)
        Vector3 newPos = transform.position + new Vector3(moveX, 0f, 0f);

        // 2. ОТСКОК ОТ СТЕН X
        if (aquarium != null)
        {
            if (newPos.x > aquarium.rightLimit)     // Упёрлись в правую стену?
            {
                newPos.x = aquarium.rightLimit;     // Прижать к стене
                direction = -1;                     // Развернуть ←
            }
            else if (newPos.x < aquarium.leftLimit) // Левая стена?
            {
                newPos.x = aquarium.leftLimit;
                direction = 1;                      // Развернуть →
            }
        }

        // 3. ПОЗИЦИЯ Y (вертикаль)
        float yOffset;
        if (bottomDweller)  // СОМ: покачивается у дна
        {
            yOffset = aquarium.bottomLimit + 0.9f + Mathf.Sin(Time.time * 1f + yOffsetPhase) * 0.1f;  // ДНО + маленькая синусоида
        }
        else                // ОБЫЧНЫЕ РЫБЫ: волны
        {
            yOffset = Mathf.Sin(Time.time * 2f + yOffsetPhase) * 0.5f;  // СИНУСОИДА вверх-вниз
            if (aquarium != null)
                yOffset = Mathf.Clamp(yOffset, aquarium.bottomLimit, aquarium.topLimit);  // Границы Y
        }
        newPos.y = yOffset;

        // 4. ПРИМЕНЯЕМ новую позицию
        transform.position = newPos;
        
        // 5. РАЗВОРОТ спрайта (не поворот)
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * (direction > 0 ? 1 : -1);  // → scale.x>0, ← scale.x<0
        transform.localScale = scale;
    }
}
