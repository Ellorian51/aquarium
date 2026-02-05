using UnityEngine;

public class AquariumController : MonoBehaviour
{
    public float leftLimit = -2.2f;
    public float rightLimit = 2.2f;
    public float bottomLimit = -1.3f;
    public float topLimit = 1.3f;

    [Header("Circle Bounds")]
    public float circleRadius = 2.0f;  // Радиус видимой круглой зоны

    private CircleCollider2D circleCollider;

    void Start()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        if (circleCollider == null) circleCollider = gameObject.AddComponent<CircleCollider2D>();
        circleCollider.radius = circleRadius;
        circleCollider.isTrigger = true;  // Не физика, только проверка
    }

    // Метод для проверки позиции в круге (используем в Fish позже)
    public bool IsInsideCircle(Vector3 position)
    {
        return Vector2.Distance(transform.position, position) <= circleRadius;
    }
}