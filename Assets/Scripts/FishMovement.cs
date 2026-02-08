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

    [Header("Побег")]
    [Range(1f, 5f)] public float fleeSpeedMultiplier = 2.5f;
    [Range(0.5f, 3f)] public float fleeDuration = 1f;

    private float _direction;
    private float _yOffsetPhase;
    private bool _isPaused;
    private float _pauseTimer;
    
    private float _fleeMultiplier = 1f;
    private float _fleeTimer;

    private Fish _fish;
    private AquariumController _aquarium;
    private MoveToPointBehavior _mtp;
    private Vector3 _prevPos;  // 🔥 для velocity поворота

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
        _prevPos = transform.position;  // 🔥 для LateUpdate velocity
        
        // ✅ КОРМЕЖКА ИМЕЕТ ПРИОРИТЕТ — FishMovement пропускает полностью!
        if (_mtp != null && _mtp.isMoving) 
            return;
        
        if (_aquarium == null) return;  

        Vector3 newPos = transform.position;

        // Паузы
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

        // Смена направления
        if (basicMovement && !_isPaused && Random.value < directionChangeChance)
            _direction *= -1f;

        // СКОРОСТЬ С Бустом!
        float baseSpeed = swimSpeed * Random.Range(0.8f, 1.2f);
        float currentSpeed = _isPaused ? 0f : baseSpeed;
        
        // Побег!
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

        // Y движение
        float yOffset = _fish.bottomDweller
            ? _aquarium.bottomLimit + 0.6f + Mathf.Sin(Time.time * 0.8f + _yOffsetPhase) * 0.3f
            : basicMovement
                ? Mathf.Lerp(transform.position.y, transform.position.y + Mathf.Sin(Time.time + _yOffsetPhase) * yWobbleAmount, 0.05f)
                : Mathf.Clamp(Mathf.Sin(Time.time * 2f + _yOffsetPhase) * 0.5f, _aquarium.bottomLimit, _aquarium.topLimit);

        newPos.y = Mathf.Clamp(yOffset, _aquarium.bottomLimit, _aquarium.topLimit);
        transform.position = newPos;

   
    }

    // 🔥 ПОВОРОТ ПО НАСТОЯЩЕМУ ДВИЖЕНИЮ (velocity) — игнор _direction багов!
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

    // 🔥 ФИКС АГРО: игнор Flee во время кормежки
    public void FleeFromFish(float fleeDirection)
    {
        if (_mtp != null && _mtp.isMoving)
        {
            Debug.Log($"{gameObject.name} КОРМИТСЯ — Flee игнор от ScaryMove!");
            return;
        }
        
        _direction = fleeDirection;
        _fleeMultiplier = fleeSpeedMultiplier;
        _fleeTimer = fleeDuration;
        Debug.Log($"{gameObject.name} УБЕГАЕТ dir={fleeDirection} x{fleeSpeedMultiplier}!");
    }
}
