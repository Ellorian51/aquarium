using UnityEngine;
using Random = UnityEngine.Random;

public class MoveToPointBehavior : MonoBehaviour
{
    [Header("Настройки")]
    public Plant plant;
    public float speed = 1f;
    public float feedingRadius = 0.2f;
    public float moveInterval = 5f;
    public float stayDurationMin = 3f;
    public float stayDurationMax = 5f;

    private AquariumController _aquarium;  // 🔥 ИСПОЛЬЗУЕТСЯ!
    private Transform _targetPoint;
    private bool _moving;
    private float _stayTimer;
    private float _timer;

    public bool isMoving => _moving;

    void Start()
    {
        _aquarium = GetComponentInParent<AquariumController>();
        _timer = moveInterval;
    }

    void Update()
    {
        if (plant == null) return;

        // 🔥 #1: ПАУЗА (приоритет!)
        if (_stayTimer > 0f)
        {
            _stayTimer -= Time.deltaTime;
            if (_stayTimer <= 0f)
            {
                Debug.Log($"🐠 {gameObject.name} НАЕЛАСЬ!");
                _stayTimer = 0f;
                _moving = false;
            }
            return;
        }

        // 🔥 #2: Ждём интервал
        if (!_moving)
        {
            _timer -= Time.deltaTime;
            if (_timer <= 0f)
            {
                _timer = moveInterval;
                ChooseTargetPoint();
            }
            return;
        }

        // 🔥 #3: ДВИЖЕНИЕ с Clamp!
        if (_targetPoint != null)
        {
            Vector3 dir = (_targetPoint.position - transform.position);
            float dist = dir.magnitude;
            if (dist > 0.05f)
            {
                dir.Normalize();
                Vector3 newPos = transform.position + dir * speed * Time.deltaTime;  // 🔥 Оптимизировано!
                
                // 🔥 Clamp с _aquarium!
                newPos.x = Mathf.Clamp(newPos.x, _aquarium.leftLimit + 0.5f, _aquarium.rightLimit - 0.5f);
                newPos.y = Mathf.Clamp(newPos.y, _aquarium.bottomLimit + 0.5f, _aquarium.topLimit - 0.5f);
                transform.position = newPos;
            }
            else
            {
                _stayTimer = Random.Range(stayDurationMin, stayDurationMax);
                Destroy(_targetPoint.gameObject);
                _targetPoint = null;
                Debug.Log($"🐠 {gameObject.name} ПРИБЫЛ — stay {_stayTimer:F1}s");
            }
        }
    }

    void ChooseTargetPoint()
    {
        Transform point = plant.GetRandomFeedingPoint();
        if (point == null) return;

        GameObject target = new GameObject("TempTarget");
        // 🔥 Vector3 — нет ambiguous!
        target.transform.position = point.position + new Vector3(
            Random.Range(-feedingRadius, feedingRadius),
            Random.Range(-feedingRadius, feedingRadius), 0);
        target.transform.parent = transform.parent;
        _targetPoint = target.transform;
        _moving = true;
    }
}
