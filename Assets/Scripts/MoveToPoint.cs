using UnityEngine;
using Random = UnityEngine.Random;
using System.Linq;

public class MoveToPointBehavior : MonoBehaviour
{
    [Header("Настройки")]
    public Plant plant;  // Игнорируем — теперь динамически!
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
    }

    void Update()
    {
        if (_aquarium == null || _aquarium.plants == null || _aquarium.plants.Length == 0) return;

        // 1. ПАУЗА (stayTimer)
        if (_stayTimer > 0f)
        {
            _stayTimer -= Time.deltaTime;
            if (_stayTimer <= 0f)
            {
              
                _stayTimer = 0f;
                _moving = false;
    
                // ✅ СБРОС ГОЛОДА!
                Fish fish = GetComponent<Fish>();
                if (fish != null) fish.OnEaten();
            }
            return;
        }

        // 2. Ждём интервал → НОВЫЙ РАНДОМ!
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

        // 3. ДВИЖЕНИЕ
        if (_targetPoint != null)
        {
            Vector3 dir = (_targetPoint.position - transform.position);
            float dist = dir.magnitude;
            
            if (dist > 0.3f)
            {
                dir.Normalize();
                Vector3 newPos = transform.position + dir * (speed * Time.deltaTime);
                
                newPos.x = Mathf.Clamp(newPos.x, _aquarium.leftLimit + 0.5f, _aquarium.rightLimit - 0.5f);
                newPos.y = Mathf.Clamp(newPos.y, _aquarium.bottomLimit + 0.5f, _aquarium.topLimit - 0.5f);
                
                transform.position = newPos;
            }
            else
            {
                transform.position = _targetPoint.position;
                _stayTimer = Random.Range(stayDurationMin, stayDurationMax);
                Destroy(_targetPoint.gameObject);
                _targetPoint = null;
                Debug.Log($"🐠 {gameObject.name} ПРИБЫЛ — stay {_stayTimer:F1}s");
            }
        }
    }

    /// 🔥 НОВЫЙ РАНДОМ ИЗ ЛЮБИМЫХ КАЖДЫЙ РАЗ!
    void ChooseTargetPoint()
    {
        Fish fish = GetComponent<Fish>();
    
        if (string.IsNullOrEmpty(fish.favoritePlantID))
        {
            Debug.LogWarning($"🐟 {gameObject.name} БЕЗ ЛЮБИМЫХ — НЕ ЕСТ!");
            return;
        }
    
        string[] favoriteIDs = fish.favoritePlantID.Split(',');
        string chosenID = favoriteIDs[Random.Range(0, favoriteIDs.Length)].Trim();
        Plant chosenPlant = _aquarium.plants.FirstOrDefault(p => p.plantID.Trim() == chosenID);
    
        if (chosenPlant == null)
        {
            Debug.LogWarning($"🐟 {gameObject.name} НЕ НАЙДЕН '{chosenID}' — НЕ ЕСТ!");
            return;
        }
    
        // ✅ ФИКС: УСТАНАВЛИВАЕМ plant для совместимости!
        this.plant = chosenPlant;
    
        Debug.Log($"🐟 {gameObject.name} → {chosenPlant.plantID} из '{fish.favoritePlantID}'");
    
        Transform point = chosenPlant.GetRandomFeedingPoint();
        if (point == null) return;

        GameObject target = new GameObject("TempTarget");
        target.transform.position = point.position + new Vector3(
            Random.Range(-feedingRadius, feedingRadius),
            Random.Range(-feedingRadius, feedingRadius), 0);
        target.transform.parent = transform.parent;
        _targetPoint = target.transform;
        _moving = true;
    }
}
