using UnityEngine;

/// <summary>
/// Паттерн поведения: рыба подплывает к точке растения, делает паузу, потом возвращается к базовому движению.
/// </summary>
public class MoveToPointBehavior : MonoBehaviour
{
    [Header("Ссылка на растение")]
    public Plant plant;               // Префаб растения, к которому подплывать

    [Header("Скорость и радиус подплывания")]
    public float speed = 1f;          
    public float feedingRadius = 0.2f; // Случайное смещение вокруг точки

    [Header("Интервал движения и диапазон паузы")]
    public float moveInterval = 5f;         
    public float stayDurationMin = 3f;      
    public float stayDurationMax = 5f;      

    private Transform _targetPoint;    // текущая цель движения
    private bool _moving;             
    private float _stayTimer;         
    private float _timer;             

    private FishMovement _fishMovement;

    void Start()
    {
        _timer = moveInterval;
        _fishMovement = GetComponent<FishMovement>();
    }

    void Update()
    {
        if (plant == null) return;

        // если у нас нет цели, выбираем новую точку у растения
        if (_targetPoint == null && !_moving && _timer <= 0f)
        {
            ChooseTargetPoint();
            _moving = true;
        }

        if (_moving && _targetPoint != null)
        {
            if (_fishMovement != null) _fishMovement.enabled = false;

            Vector3 dir = (_targetPoint.position - transform.position);
            float distance = dir.magnitude;

            if (distance > 0.05f)
            {
                transform.position += dir.normalized * (speed * Time.deltaTime);
            }
            else
            {
                _moving = false;
                _stayTimer = Random.Range(stayDurationMin, stayDurationMax);
            }
        }
        else if (_stayTimer > 0f)
        {
            // пауза у точки (симуляция кормежки)
            _stayTimer -= Time.deltaTime;
        }
        else
        {
            // включаем базовое движение обратно
            if (_fishMovement != null) _fishMovement.enabled = true;

            // ждём следующего интервала
            _timer -= Time.deltaTime;
            if (_timer <= 0f)
            {
                _timer = moveInterval;
            }
        }
    }

    /// <summary>
    /// Выбирает случайную точку у растения с небольшим смещением
    /// </summary>
    private void ChooseTargetPoint()
    {
        Transform point = plant.GetRandomFeedingPoint();
        if (point == null) return;

        Vector3 offset = new Vector3(
            Random.Range(-feedingRadius, feedingRadius),
            Random.Range(-feedingRadius, feedingRadius),
            0f
        );

        // создаём временную цель, чтобы не менять сам Transform точки растения
        GameObject tempTarget = new GameObject("TempTarget");
        tempTarget.transform.position = point.position + offset;
        _targetPoint = tempTarget.transform;
    }
}