using UnityEngine;
using Random = UnityEngine.Random; 

public class ScaryMove : MonoBehaviour
{
    [Header("Отпугивание")]
    [Range(0.5f, 3f)] public float detectRadius = 1.5f;
    [Range(0f, 0.1f)] public float attackChance = 0.02f;
    [Range(1f, 10f)] public float fleeCooldown = 10f;
    public float scaredCooldown = 5f;

    private Fish _fish;
    private float _lastScareTime;
    private float _lastScaredTime;  

    void Start()
    {
        _fish = GetComponent<Fish>();
        Debug.Log($"{gameObject.name} Scary готов!");
    }

    void Update()  // ✅ Убрали [Obsolete]
    {
        if (!_fish.isAggressive) return;
    
        MoveToPointBehavior ownMtp = GetComponent<MoveToPointBehavior>();
        if (ownMtp != null && ownMtp.isMoving) return;
        if (Time.time - _lastScaredTime < scaredCooldown) return;
        if (Time.time - _lastScareTime < fleeCooldown) return;

        if (Random.value > attackChance) return;
        
        Collider2D[] nearby = new Collider2D[20];
        int count = Physics2D.OverlapCircleNonAlloc(transform.position, detectRadius, nearby);
        
        for (int i = 0; i < count; i++)
        {
            
            if (!nearby[i].CompareTag("Fish")) continue;
            
            Fish otherFish = nearby[i].GetComponent<Fish>();
            if (otherFish == null || otherFish == _fish) continue;

            if (!otherFish.isAggressive)
            {
                Debug.Log($"🦈 {gameObject.name} пугает {nearby[i].name}");

                FishMovement otherMovement = nearby[i].GetComponent<FishMovement>();
                if (otherMovement != null)
                {
                    Vector2 victimFleeDir = (nearby[i].transform.position - transform.position).normalized;
                    otherMovement.FleeFromFish(Mathf.Sign(victimFleeDir.x));
                }
                
                MoveToPointBehavior victimMtp = nearby[i].GetComponent<MoveToPointBehavior>();
                if (victimMtp != null)
                {
                    victimMtp.enabled = false;
                    Debug.Log($"🍽️ {nearby[i].name} кормление ОСТАНОВЛЕНО!");
                }
            
                _lastScareTime = Time.time;
                return;
            }
        }
    }

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
