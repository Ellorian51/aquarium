using UnityEngine;

public class MoveToPointBehavior : MonoBehaviour
{
    public Plant plant;

    public float speed = 1f;
    public float feedingRadius = 0.2f;
    public float moveInterval = 5f;
    public float stayDurationMin = 3f;
    public float stayDurationMax = 5f;

    private Transform _targetPoint;
    bool _moving;
    private float _stayTimer;
    private float _timer;
    private FishMovement _fishMovement;

    public bool isMoving => _moving;

    void Start()
    {
        _timer = moveInterval;
        _fishMovement = GetComponent<FishMovement>();
    }

    void Update()
    {
        if (plant == null) return;

        if (_targetPoint == null && !_moving && _timer <= 0f)
        {
            ChooseTargetPoint();
        }

        if (_moving && _targetPoint != null)
        {
            if (_fishMovement != null) _fishMovement.enabled = false;

            Vector3 dir = _targetPoint.position - transform.position;

            if (dir.magnitude > 0.05f)
            {
                transform.position += dir.normalized * (speed * Time.deltaTime);
            }
            else
            {
                StopMove();
                _stayTimer = Random.Range(stayDurationMin, stayDurationMax);
            }
        }
        else if (_stayTimer > 0f)
        {
            _stayTimer -= Time.deltaTime;
        }
        else
        {
            if (_fishMovement != null) _fishMovement.enabled = true;

            _timer -= Time.deltaTime;
            if (_timer <= 0f)
                _timer = moveInterval;
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

        StartMove(tempTarget.transform);
    }

    void StartMove(Transform point)
    {
        _targetPoint = point;
        _moving = true;
        Debug.Log("Иду к растению");
    }

    void StopMove()
    {
        _moving = false;
    }
}