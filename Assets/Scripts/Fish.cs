using UnityEngine;

/// <summary>
/// Сущность рыбы — хранит тип и связь с аквариумом.
/// </summary>
public class Fish : MonoBehaviour
{
    [Header("ВИД РЫБЫ")]
    public bool bottomDweller;  // донная или нет
    public bool isAggressive;   // ✅ АГРЕССИВНАЯ!

    [Header("🆔 Кормежка")]
    [SerializeField] public string favoritePlantID = "";  // "Plant4,Plant3"
    [Range(30f, 300f)] public float starveTime = 120f;  // 2 мин без еды

    [HideInInspector]
    public AquariumController aquarium;
    
    [HideInInspector] public float lastEatTime;
    [HideInInspector] public bool isStarving;
    
    private float _starveTimer;

    void Start()
    {
        if (aquarium == null)
            aquarium = GetComponentInParent<AquariumController>();
            
        _starveTimer = 0f;
    }

    void Update()
    {
        if (starveTime <= 0) return;
        
        if (!isStarving)
        {
            _starveTimer += Time.deltaTime;
            if (_starveTimer >= starveTime)
            {
                isStarving = true;
                Debug.LogWarning($"💀 {gameObject.name} ГОЛОДАЕТ {_starveTimer:F0}s!");
            }
        }
    }

    public void OnEaten()
    {
        lastEatTime = Time.time;
        _starveTimer = 0f;
        isStarving = false;
        Debug.Log($"🍽️ {gameObject.name} НАЕЛАСЬ!");
    }

    public void CheckStarvation()
    {
        if (isStarving)
        {
            Debug.Log($"💀 {gameObject.name} УМИРАЕТ!");
            Destroy(gameObject);
        }
    }
}