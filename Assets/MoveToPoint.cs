using UnityEngine;

public class MoveToPointBehavior : MonoBehaviour
{
    [Header("Настройки кормежки")]
    public Plant plant;
    public float speed = 1f;
    public float feedingRadius = 0.2f;
    public float moveInterval = 5f;
    public float stayDurationMin = 3f;
    public float stayDurationMax = 5f;

    private Transform _targetPoint;
    private bool _moving;
    private float _stayTimer;
    private float _timer;

    public bool isMoving => _moving;

    void Start()
    {
        _timer = moveInterval;
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

        // ✅ Стоим и едим
        if (_stayTimer > 0f)
        {
            _stayTimer -= Time.deltaTime;
            if (_stayTimer <= 0f)
            {
                StopMove();
            }
            return;
        }

        // ✅ Плывём к растению
        if (_moving && _targetPoint != null)
        {
            Vector3 dir = (_targetPoint.position - transform.position);
            float distance = dir.magnitude;

            if (distance > 0.1f)
            {
                dir = dir.normalized;

                // Поворот
                Vector3 scale = transform.localScale;
                scale.x = Mathf.Abs(scale.x) * (dir.x > 0 ? 1f : -1f);
                transform.localScale = scale;

                // Движение
                transform.position += dir * (speed * Time.deltaTime);
            }
            else
            {
                // Доплыли = начинаем есть
                StopMove();
                _stayTimer = Random.Range(stayDurationMin, stayDurationMax);
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
        tempTarget.transform.position = point.position + offset;
        tempTarget.transform.parent = transform.parent;

        StartMove(tempTarget.transform);
    }

    void StartMove(Transform point)
    {
        _targetPoint = point;
        _moving = true;
        Debug.Log($"{gameObject.name} плывёт кормиться!");
    }

    void StopMove()
    {
        if (_targetPoint != null)
        {
            Destroy(_targetPoint.gameObject);
            _targetPoint = null;
        }
        _moving = false;
        Debug.Log($"{gameObject.name} доела и уплывает!");
    }
}
