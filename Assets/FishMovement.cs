using UnityEngine;

/// <summary>
/// Двигает рыбу в аквариуме на основе данных Fish и AquariumController.
/// Поддерживает базовое поведение BasicMovement для травоядных рыб.
/// Все публичные параметры можно менять в Play Mode.
/// </summary>
public class FishMovement : MonoBehaviour
{
    [Header("Скорость и базовое поведение")]
    public float swimSpeed = 1f;        // базовая скорость, задаётся в Inspector
    public int startDirection;      // 1 = вправо, -1 = влево
    public float yOffsetSeed;      // фазовый сдвиг для Y колебаний

    [Header("Тип поведения")]
    public bool basicMovement; // галка для травоядной рыбы с BasicMovement

    [Header("Живое поведение для BasicMovement")]
    [Range(0f, 0.2f)]
    public float pauseChance = 0.01f;      // шанс замереть на кадр
    [Range(0.1f, 3f)]
    public float pauseDurationMin = 0.3f;  // мин. время паузы
    [Range(0.1f, 3f)]
    public float pauseDurationMax = 1.0f;  // макс. время паузы
    [Range(0f, 0.05f)]
    public float directionChangeChance = 0.005f; // шанс слегка изменить направление по X
    [Range(0f, 1f)]
    public float yWobbleAmount = 0.2f;    // амплитуда хаотичного колебания Y

    // 🔹 приватные поля для внутренней логики движения
    private float _direction;       // текущее направление движения
    private float _yOffsetPhase;    // для хаотичных колебаний по Y

    private bool _isPaused;
    private float _pauseTimer;

    private Fish _fish;
    private AquariumController _aquarium;

    void Start()
    {
        _fish = GetComponent<Fish>();
        _aquarium = _fish.aquarium;

        // Инициализация направления и фазы
        _direction = startDirection != 0 ? startDirection : (Random.value > 0.5f ? 1f : -1f);
        _yOffsetPhase = yOffsetSeed + Random.Range(0f, Mathf.PI * 2f);
    }

    void Update()
    {
        if (_aquarium == null) return;

        Vector3 newPos = transform.position;

        // ==== BasicMovement: логика паузы ====
        if (basicMovement && !_isPaused && Random.value < pauseChance)
        {
            _isPaused = true;
            _pauseTimer = Random.Range(pauseDurationMin, pauseDurationMax);
        }

        if (_isPaused)
        {
            _pauseTimer -= Time.deltaTime;
            if (_pauseTimer <= 0f) _isPaused = false;
        }

        // ==== BasicMovement: хаотичная смена направления ====
        if (basicMovement && !_isPaused && Random.value < directionChangeChance)
        {
            _direction *= -1f; // слегка меняем направление
        }

        // ==== Движение по X ====
        float currentSpeed = _isPaused ? 0f : swimSpeed * Random.Range(0.8f, 1.2f);
        newPos.x += _direction * currentSpeed * Time.deltaTime;

        // ==== Реакция на стены аквариума ====
        if (newPos.x > _aquarium.rightLimit)
        {
            newPos.x = _aquarium.rightLimit;
            _direction = -1f;
        }
        else if (newPos.x < _aquarium.leftLimit)
        {
            newPos.x = _aquarium.leftLimit;
            _direction = 1f;
        }

        // ==== Движение по Y ====
        float yOffset;

        if (_fish.bottomDweller)
        {
            // донные рыбы — маленькие колебания над дном
            yOffset = _aquarium.bottomLimit + 0.9f +
                      Mathf.Sin(Time.time * 1f + _yOffsetPhase) * 0.1f;
        }
        else if (basicMovement)
        {
            // BasicMovement — плавное хаотичное движение
            float targetY = transform.position.y + Mathf.Sin(Time.time + _yOffsetPhase) * yWobbleAmount;
            // плавное приближение к targetY
            yOffset = Mathf.Lerp(transform.position.y, targetY, 0.05f);
        }
        else
        {
            // обычное движение (синусоида)
            yOffset = Mathf.Sin(Time.time * 2f + _yOffsetPhase) * 0.5f;
            yOffset = Mathf.Clamp(yOffset, _aquarium.bottomLimit, _aquarium.topLimit);
        }

        newPos.y = Mathf.Clamp(yOffset, _aquarium.bottomLimit, _aquarium.topLimit);
        transform.position = newPos;

        // ==== Разворот спрайта по направлению ====
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * (_direction > 0 ? 1 : -1);
        transform.localScale = scale;
    }
}