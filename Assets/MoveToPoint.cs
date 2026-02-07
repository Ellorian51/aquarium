using UnityEngine;
using Random = UnityEngine.Random;

public class MoveToPointBehavior : MonoBehaviour
{
    [Header("Настройки кормежки")]
    public Plant plant;
    public float speed = 1f;
    public float feedingRadius = 0.2f;
    public float moveInterval = 5f;
    public float stayDurationMin = 3f;
    public float stayDurationMax = 5f;

    private AquariumController _aquarium;
    private Transform _targetPoint;
    private bool _moving;
    private float _stayTimer;
    private float _timer;

    public bool isMoving => _moving;

    void Start()
    {
        _aquarium = GetComponentInParent<AquariumController>();
        _timer = moveInterval;
        Debug.Log($"🐠 {gameObject.name} START: plant={plant?.plantID ?? "NULL"}");
    }

    void Update()
    {
        if (plant == null) return;

        // Таймер кормежки
        if (!_moving && _stayTimer <= 0f)
        {
            _timer -= Time.deltaTime;
            if (_timer <= 0f)
            {
                _timer = moveInterval;
                ChooseTargetPoint();
            }
        }

        if (_stayTimer > 0f)
        {
            _stayTimer -= Time.deltaTime;
            if (_stayTimer <= 0f)
            {
                StopMove();
            }
            return;
        }

        // ✅ ПЛЫВЁМ — ТОЛЬКО position!
        if (_moving && _targetPoint != null)
        {
            Vector3 dir = (_targetPoint.position - transform.position);
            float distance = dir.magnitude;

            if (distance > 0.1f)
            {
                dir = dir.normalized;
                // ✅ УДАЛЁН поворот scale.x — FishMovement сам!
                
                // Clamp в аквариум
                Vector3 newPos = transform.position + dir * (speed * Time.deltaTime);
                newPos.x = Mathf.Clamp(newPos.x, _aquarium.leftLimit + 0.5f, _aquarium.rightLimit - 0.5f);
                newPos.y = Mathf.Clamp(newPos.y, _aquarium.bottomLimit + 0.5f, _aquarium.topLimit - 0.5f);
                transform.position = newPos;
            }
            else
            {
                StopMove();
                _stayTimer = Random.Range(stayDurationMin, stayDurationMax);
                Debug.Log($"🐠 {gameObject.name} ЕСТ!");
            }
        }
    }

    private void ChooseTargetPoint()
    {
        Transform point = plant.GetRandomFeedingPoint();
        if (point == null) return;

        Vector3 offset = new Vector3(
            Random.Range(-feedingRadius, feedingRadius),
            Random.Range(-feedingRadius, feedingRadius),
            0f
        );

        GameObject tempTarget = new GameObject("TempTarget");
        Vector3 finalPos = point.position + offset;
        
        // ✅ Clamp в аквариум
        finalPos.x = Mathf.Clamp(finalPos.x, _aquarium.leftLimit + 0.5f, _aquarium.rightLimit - 0.5f);
        finalPos.y = Mathf.Clamp(finalPos.y, _aquarium.bottomLimit + 0.5f, _aquarium.topLimit - 0.5f);
        
        tempTarget.transform.position = finalPos;
        tempTarget.transform.parent = transform.parent;

        StartMove(tempTarget.transform);
    }

    void StartMove(Transform point)
    {
        _targetPoint = point;
        _moving = true;
        Debug.Log($"🐠 {gameObject.name} плывёт к {plant.plantID}!");
    }

    void StopMove()
    {
        if (_targetPoint != null)
        {
            Destroy(_targetPoint.gameObject);
            _targetPoint = null;
        }
        _moving = false;
        Debug.Log($"🐠 {gameObject.name} доела {plant.plantID}!");
    }
}
