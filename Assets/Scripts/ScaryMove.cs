using UnityEngine;
using Random = UnityEngine.Random;  // ✅ Алиас наверху

public class ScaryMove : MonoBehaviour
{
    [Header("Отпугивание")]
    [Range(0.5f, 3f)]
    public float detectRadius = 1.5f;
    
    [Header("Частота действия")]
    [Range(0f, 0.1f)] 
    public float attackChance = 0.02f;
    
    [Header("Логика")]
    [Range(1f, 10f)] 
    public float fleeCooldown = 10f;
    
    [Header("После испуга")]
    public float scaredCooldown = 5f;  // НЕ пугает 5 сек после flee

    private Fish _fish;
    private float _lastScareTime;
    private float _lastScaredTime;  // ✅ НОВОЕ: когда сам испугался

    void Start()
    {
        _fish = GetComponent<Fish>();
        Debug.Log($"{gameObject.name} Scary готов!");
    }

    void Update()
    {
        if (!_fish.isAggressive) return;
    
        
        MoveToPointBehavior ownMtp = GetComponent<MoveToPointBehavior>();
        if (ownMtp != null && ownMtp.isMoving) return;
        if (Time.time - _lastScaredTime < scaredCooldown)
        // ✅ НОВОЕ: НЕ пугает, если сам напуган недавно
        if (Time.time - _lastScaredTime < scaredCooldown) return;
    
        if (Time.time - _lastScareTime < fleeCooldown) return;

        if (Random.value > attackChance) return;

        Collider2D[] nearby = new Collider2D[20];
        int count = Physics2D.OverlapCircleNonAlloc(transform.position, detectRadius, nearby);
        for (int i = 0; i < count; i++)
        {
            Fish otherFish = nearby[i].GetComponent<Fish>();
            if (otherFish == null || otherFish == _fish) continue;

            Debug.Log($"🦈 {gameObject.name} пугает {nearby[i].name}");

            FishMovement otherMovement = nearby[i].GetComponent<FishMovement>();
            if (otherMovement != null)
            {
                Vector2 victimFleeDir = (nearby[i].transform.position - transform.position).normalized;
                otherMovement.FleeFromFish(Mathf.Sign(victimFleeDir.x));
            }
        
            _lastScareTime = Time.time;
            return;
        }
    }

    // ✅ НОВЫЙ ПУБЛИЧНЫЙ МЕТОД: вызывается извне при flee
    public void OnScared()
    {
        _lastScaredTime = Time.time;
        Debug.Log($"🦈 {gameObject.name} НАПУГАН — cooldown {scaredCooldown}s");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectRadius);
    }
}
