using UnityEngine;

public class FishMovement : MonoBehaviour
{
    [Header("Скорость и базовое поведение")]
    public float swimSpeed = 1f;
    public int startDirection;
    public float yOffsetSeed;

    [Header("Loops")]
    public bool basicMovement;

    [Header("Живое поведение для BasicMovement")]
    [Range(0f, 0.2f)] public float pauseChance = 0.01f;
    [Range(0.1f, 3f)] public float pauseDurationMin = 0.3f;
    [Range(0.1f, 3f)] public float pauseDurationMax = 1.0f;
    [Range(0f, 0.05f)] public float directionChangeChance = 0.005f;
    [Range(0f, 1f)] public float yWobbleAmount = 0.2f;

    [Header("Dweller Movements")]
    public bool dwellerMovement;  // ✅ Новая галка!
    [Range(0f, 0.2f)] public float dwellerPauseChance = 0.02f;
    [Range(0.1f, 3f)] public float dwellerPauseDurationMin = 0.8f;
    [Range(0.1f, 3f)] public float dwellerPauseDurationMax = 2.0f;
    [Range(0f, 0.05f)] public float dwellerDirectionChangeChance = 0.002f;
    [Range(0f, 1f)] public float dwellerYWobbleAmount = 0.1f;

    [Header("Побег")]
    [Range(1f, 5f)] public float fleeSpeedMultiplier = 2.5f;
    [Range(0.5f, 3f)] public float fleeDuration = 1f;
    [Range(2f, 10f)] public float fleeDistance = 4f;
    

    private float _direction;
    private float _yOffsetPhase;
    private bool _isPaused;
    private float _pauseTimer;
    
    private float _fleeMultiplier = 1f;
    private float _fleeTimer;

    private Fish _fish;
    private AquariumController _aquarium;
    private MoveToPointBehavior _mtp;
    private Vector3 _prevPos;

    void Start()
    {
        _fish = GetComponent<Fish>();
        _aquarium = _fish.aquarium;
        _mtp = GetComponent<MoveToPointBehavior>();

        Debug.Log($"🐟 {gameObject.name} FishMovement START: mtp={_mtp != null} plant={_mtp?.plant?.name ?? "NO"}");

        _direction = startDirection != 0 ? startDirection : (Random.value > 0.5f ? 1f : -1f);
        _yOffsetPhase = yOffsetSeed + Random.Range(0f, Mathf.PI * 2f);
        _prevPos = transform.position;
    }

    void Update()
    {
        Fish fish = GetComponent<Fish>();
        if (fish != null) fish.CheckStarvation();  
        
        _prevPos = transform.position;
        
        // ✅ КОРМЕЖКА ИМЕЕТ ПРИОРИТЕТ
        if (_mtp != null && _mtp.isMoving) return;
        
        if (_aquarium == null) return;  

        Vector3 newPos = transform.position;

        // 🔥 ПАУЗЫ — для любого режима
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

        // 🔥 СМЕНА НАПРАВЛЕНИЯ
        if (anyMovement && !_isPaused && Random.value < GetDirectionChangeChance())
            _direction *= -1f;

        // СКОРОСТЬ
        float baseSpeed = swimSpeed * Random.Range(0.8f, 1.2f);
        float currentSpeed = _isPaused ? 0f : baseSpeed;
        
        if (_fleeTimer > 0f)
        {
            currentSpeed *= _fleeMultiplier;
            _fleeTimer -= Time.deltaTime;
        }

        newPos.x += _direction * currentSpeed * Time.deltaTime;

        // Границы
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

        // 🔥 Y движение — выбор по режиму
        float yWobble = dwellerMovement ? dwellerYWobbleAmount : yWobbleAmount;
        float yOffset = _fish.bottomDweller
            ? _aquarium.bottomLimit + 0.6f + Mathf.Sin(Time.time * 0.8f + _yOffsetPhase) * 0.3f
            : Mathf.Lerp(transform.position.y, transform.position.y + Mathf.Sin(Time.time + _yOffsetPhase) * yWobble, 0.05f);

        newPos.y = Mathf.Clamp(yOffset, _aquarium.bottomLimit, _aquarium.topLimit);
        transform.position = newPos;
    }

    void LateUpdate()
    {
        Vector3 velocity = (transform.position - _prevPos) / Time.deltaTime;
        if (Mathf.Abs(velocity.x) > 0.05f)
        {
            float moveDir = Mathf.Sign(velocity.x);
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * moveDir;
            transform.localScale = scale;
        }
    }

    // 🔥 Get методы для режимов
    float GetPauseChance()
    {
        if (dwellerMovement) return dwellerPauseChance;
        return pauseChance;
    }

    float GetPauseDurationMin()
    {
        if (dwellerMovement) return dwellerPauseDurationMin;
        return pauseDurationMin;
    }

    float GetPauseDurationMax()
    {
        if (dwellerMovement) return dwellerPauseDurationMax;
        return pauseDurationMax;
    }

    float GetDirectionChangeChance()
    {
        if (dwellerMovement) return dwellerDirectionChangeChance;
        return directionChangeChance;
    }

    public void FleeFromFish(float fleeDirection)
    {
        // if (_mtp != null && _mtp.isMoving) return;  ← ЗАКОММЕНТИРУЙ!
    
        ScaryMove scary = GetComponent<ScaryMove>();
        if (scary != null) scary.OnScared();

        _direction = fleeDirection;
        _fleeMultiplier = fleeSpeedMultiplier;
        _fleeTimer = Mathf.Min(fleeDuration, fleeDistance / swimSpeed);
        Debug.Log($"{gameObject.name} УБЕГАЕТ {fleeDistance}m dir={fleeDirection}");
    }
}
