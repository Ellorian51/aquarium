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

    private float _direction;
    private float _yOffsetPhase;
    private bool _isPaused;
    private float _pauseTimer;

    private Fish _fish;
    private AquariumController _aquarium;
    private MoveToPointBehavior _mtp;  // ✅ Используется ниже!

    void Start()
    {
        _fish = GetComponent<Fish>();
        _aquarium = _fish.aquarium;
        _mtp = GetComponent<MoveToPointBehavior>();

        _direction = startDirection != 0 ? startDirection : (Random.value > 0.5f ? 1f : -1f);
        _yOffsetPhase = yOffsetSeed + Random.Range(0f, Mathf.PI * 2f);
    }

    void Update()
    {
        if (_mtp != null && _mtp.isMoving) return;  
        
        if (_aquarium == null) return;  

        Vector3 newPos = transform.position;

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

        if (basicMovement && !_isPaused && Random.value < directionChangeChance)
        {
            _direction *= -1f;
        }

        float currentSpeed = _isPaused ? 0f : swimSpeed * Random.Range(0.8f, 1.2f);
        newPos.x += _direction * currentSpeed * Time.deltaTime;

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

        float yOffset;

        if (_fish.bottomDweller)
        {
            yOffset = _aquarium.bottomLimit + -1f +
                Mathf.Sin(Time.time * 1f + _yOffsetPhase) * 0.1f;
        }
        else if (basicMovement)
        {
            float targetY = transform.position.y + Mathf.Sin(Time.time + _yOffsetPhase) * yWobbleAmount;
            yOffset = Mathf.Lerp(transform.position.y, targetY, 0.05f);
        }
        else
        {
            yOffset = Mathf.Sin(Time.time * 2f + _yOffsetPhase) * 0.5f;
            yOffset = Mathf.Clamp(yOffset, _aquarium.bottomLimit, _aquarium.topLimit);
        }

        newPos.y = Mathf.Clamp(yOffset, _aquarium.bottomLimit, _aquarium.topLimit);
        transform.position = newPos;

        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * (_direction > 0 ? 1 : -1);
        transform.localScale = scale;
    }
}
