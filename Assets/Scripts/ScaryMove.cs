using System;
using UnityEngine;

public class ScaryMove : MonoBehaviour
{
    [Header("Отпугивание")]
    [Range(0.5f, 3f)]
    public float detectRadius = 1.5f;
    
    [Header("Логика")]
    [Range(1f, 10f)] 
    public float fleeCooldown = 10f;

    private Fish _fish;
    private float _lastScareTime;

    void Start()
    {
        _fish = GetComponent<Fish>();
        Debug.Log($"{gameObject.name} Scary готов!");
    }

    [Obsolete("Obsolete")]
    void Update()
    {
        if (!_fish.isAggressive) return;
    
        // 🔥 ФИКС: Агро НЕ пугают, пока САМИ едят!
        MoveToPointBehavior ownMtp = GetComponent<MoveToPointBehavior>();
        if (ownMtp != null && ownMtp.isMoving) return;
    
        if (Time.time - _lastScareTime < fleeCooldown) return;

        Collider2D[] nearby = new Collider2D[20];
        int count = Physics2D.OverlapCircleNonAlloc(transform.position, detectRadius, nearby);
        for (int i = 0; i < count; i++)
        {
            Fish otherFish = nearby[i].GetComponent<Fish>();
            if (otherFish == null || otherFish == _fish) continue;  // 🔥 ФИКС: НЕ СЕБЯ!

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

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectRadius);
    }
}