using UnityEngine;

public class ScaryMove : MonoBehaviour
{
    [Header("Отпугивание")]
    [Range(0.5f, 3f)]
    public float detectRadius = 1.5f;
    
    [Header("Логика")]
    [Range(0.1f, 2f)]
    public float fleeCooldown = 0.5f;

    private Fish _fish;
    private float _lastScareTime;

    void Start()
    {
        _fish = GetComponent<Fish>();
        Debug.Log($"{gameObject.name} Scary готов!");
    }

    void Update()
    {
        // Только агро рыбы пугают
        if (!_fish.isAggressive) return;
        
        // Cooldown между пугами
        if (Time.time - _lastScareTime < fleeCooldown) return;

        // Ищем жертв в радиусе
        Collider2D[] nearby = new Collider2D[10];
        int count = Physics2D.OverlapCircleNonAlloc(transform.position, detectRadius, nearby);
        for (int i = 0; i < count; i++)
        {
            Fish otherFish = nearby[i].GetComponent<Fish>();
            if (otherFish == null || otherFish.gameObject == gameObject) continue;

            // ✅ ПУГАЕМ ДРУГУЮ РЫБУ!
            FishMovement otherMovement = nearby[i].GetComponent<FishMovement>();
            if (otherMovement != null)
            {
                // ЖЕРТВА убегает ОТ НАС (агрессора)
                Vector2 victimFleeDir = (nearby[i].transform.position - transform.position).normalized;
                otherMovement.FleeFromFish(Mathf.Sign(victimFleeDir.x));
                
                Debug.Log($"{gameObject.name} ПУГАЕТ {nearby[i].name}!");
            }
            
            _lastScareTime = Time.time;
            return;  // пугаем ближайшую
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectRadius);
    }
}