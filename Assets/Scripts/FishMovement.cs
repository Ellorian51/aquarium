using UnityEngine;

public class FishMovement : MonoBehaviour
{
    [Header("Скорость и базовое поведение")]
    public float swimSpeed = 1f;       // Базовая скорость плавания
    public int startDirection;         // Начальное направление: 1 или -1
    public float yOffsetSeed;          // Случайная фаза для движения по Y

    [Header("Loops")]
    public bool basicMovement;         // Включить базовое движение (влево/вправо)

    [Header("Живое поведение для BasicMovement")]
    [Range(0f, 0.2f)] public float pauseChance = 0.01f;           // Вероятность паузы
    [Range(0.1f, 3f)] public float pauseDurationMin = 0.3f;       // Мин. длительность паузы
    [Range(0.1f, 3f)] public float pauseDurationMax = 1.0f;       // Макс. длительность паузы
    [Range(0f, 0.05f)] public float directionChangeChance = 0.005f; // Шанс смены направления
    [Range(0f, 1f)] public float yWobbleAmount = 0.2f;           // Амплитуда колебаний по Y

    [Header("Dweller Movements")]
    public bool dwellerMovement;      // Режим донной рыбы
    [Range(0f, 0.2f)] public float dwellerPauseChance = 0.02f;
    [Range(0.1f, 3f)] public float dwellerPauseDurationMin = 0.8f;
    [Range(0.1f, 3f)] public float dwellerPauseDurationMax = 2.0f;
    [Range(0f, 0.05f)] public float dwellerDirectionChangeChance = 0.002f;
    [Range(0f, 1f)] public float dwellerYWobbleAmount = 0.1f;

    [Header("Побег")]
    [Range(1f, 5f)] public float fleeSpeedMultiplier = 2.5f;  // Множитель скорости при побеге
    [Range(0.5f, 3f)] public float fleeDuration = 1f;          // Время побега
    [Range(2f, 10f)] public float fleeDistance = 4f;           // Расстояние побега

    // Внутренние поля
    private float _direction;          // Текущее направление
    private float _yOffsetPhase;       // Фаза для Y колебаний
    private bool _isPaused;            // Флаг паузы
    private float _pauseTimer;         // Таймер паузы

    private float _fleeMultiplier = 1f; // Множитель скорости побега
    private float _fleeTimer;           // Таймер побега

    private Fish _fish;                  // Ссылка на Fish
    private AquariumController _aquarium; // Ссылка на Aquarium
    private MoveToPointBehavior _mtp;    // Ссылка на поведение к точке кормежки
    private Vector3 _prevPos;            // Для вычисления направления движения
    private const float RiseSpeed = 0.08f;

    void Start()
    {
        {
            _fish = GetComponent<Fish>(); // ← КЭШИРУЕМ ОДИН РАЗ!
            _aquarium = _fish.aquarium;
            _mtp = GetComponent<MoveToPointBehavior>();
        }

        Debug.Log($"🐟 {gameObject.name} FishMovement START: mtp={_mtp != null} plant={_mtp?.plant?.name ?? "NO"}");

        // Устанавливаем направление: если startDirection задан, используем его, иначе случайно
        _direction = startDirection != 0 ? startDirection : (Random.value > 0.5f ? 1f : -1f);

        // Фаза для Y колебаний + случайная фаза
        _yOffsetPhase = yOffsetSeed + Random.Range(0f, Mathf.PI * 2f);

        _prevPos = transform.position; // Запоминаем начальную позицию
    }

    void Update()
    {
        if (_fish != null) _fish.CheckStarvation();  // ← КЭШ ВМЕСТО CheckStarvation (ОН ЖРЁТ)
        _prevPos = transform.position;
        _prevPos = transform.position;

        // 🔥 НОВЫЙ КОД: подъём бездействующих рыб
        if(!IsActiveMovement())
        {
            Vector3 pos = transform.position;
            pos.y += RiseSpeed * Time.deltaTime;
            if (_aquarium != null)
            {
                pos.y = Mathf.Clamp(pos.y, _aquarium.bottomLimit + 0.5f, _aquarium.topLimit - 0.2f);
            }
            transform.position = pos;
        }

        // КОРМЕЖКА ИМЕЕТ ПРИОРИТЕТ: если рыба движется к точке, базовое движение не выполняется
        if (_mtp != null && _mtp.isMoving) return;

        if (_aquarium == null) return;

        Vector3 newPos = transform.position;

        // Паузы
        bool anyMovement = basicMovement || dwellerMovement;
        if (anyMovement && !_isPaused && Random.value < GetPauseChance())
        {
            _isPaused = true;
            _pauseTimer = Random.Range(GetPauseDurationMin(), GetPauseDurationMax());
        }

        if (_isPaused)
        {
            _pauseTimer -= Time.deltaTime;
            if (_pauseTimer <= 0f) _isPaused = false;
        }

        // Смена направления
        if (anyMovement && !_isPaused && Random.value < GetDirectionChangeChance())
            _direction *= -1f;

        // Вычисляем скорость
        float baseSpeed = swimSpeed * Random.Range(0.8f, 1.2f);
        float currentSpeed = _isPaused ? 0f : baseSpeed;

        // Применяем побег
        if (_fleeTimer > 0f)
        {
            currentSpeed *= _fleeMultiplier;
            _fleeTimer -= Time.deltaTime;
        }

        newPos.x += _direction * currentSpeed * Time.deltaTime;

        // Проверка границ аквариума
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

        // Движение по Y
        float yWobble = dwellerMovement ? dwellerYWobbleAmount : yWobbleAmount;
        float yOffset = _fish.bottomDweller
            ? _aquarium.bottomLimit + 0.6f + Mathf.Sin(Time.time * 0.8f + _yOffsetPhase) * 0.3f
            : Mathf.Lerp(transform.position.y, transform.position.y + Mathf.Sin(Time.time + _yOffsetPhase) * yWobble, 0.05f);

        newPos.y = Mathf.Clamp(yOffset, _aquarium.bottomLimit, _aquarium.topLimit);
        transform.position = newPos;
    }

    void LateUpdate()
    {
        // Вычисляем движение по X для корректировки спрайта
        Vector3 velocity = (transform.position - _prevPos) / Time.deltaTime;
        if (Mathf.Abs(velocity.x) > 0.05f)
        {
            float moveDir = Mathf.Sign(velocity.x);
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * moveDir;
            transform.localScale = scale;
        }
    }

    // Методы для получения параметров паузы и смены направления
    float GetPauseChance() => dwellerMovement ? dwellerPauseChance : pauseChance;
    float GetPauseDurationMin() => dwellerMovement ? dwellerPauseDurationMin : pauseDurationMin;
    float GetPauseDurationMax() => dwellerMovement ? dwellerPauseDurationMax : pauseDurationMax;
    float GetDirectionChangeChance() => dwellerMovement ? dwellerDirectionChangeChance : directionChangeChance;

    // Метод для побега от другой рыбы
    public void FleeFromFish(float fleeDirection)
    {
        if (_mtp != null && _mtp.isMoving) return;  // Кормящаяся рыба не убегает
    
        ScaryMove scary = GetComponent<ScaryMove>();
        if (scary != null) scary.OnScared();

        _direction = fleeDirection;
        _fleeMultiplier = fleeSpeedMultiplier;
        _fleeTimer = Mathf.Min(fleeDuration, fleeDistance / swimSpeed);

        Debug.Log($"{gameObject.name} УБЕГАЕТ {fleeDistance}m dir={fleeDirection}");
    }

    // 🔥 ГЕТТЕР ДЛЯ AquariumController - проверяет АКТИВНОЕ движение
    public bool IsActiveMovement()
    {
        return _isPaused || _fleeTimer > 0f || (_mtp != null && _mtp.isMoving);
    }
}
