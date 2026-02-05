using DefaultNamespace;
using UnityEngine;

public class Fish : MonoBehaviour
{
    [Header("Movement")]
    public float swimSpeed = 1f;
    public int startDirection = 1;
    
    [Header("Y Offset (для асинхронности)")]
    public float yOffsetSeed = 0f;
    
    [Header("Damage (later)")]
    public float waterDamage = 3f;

    // Ссылка на аквариум
    public AquariumController aquarium;

    private float timer = 0f;
    private int direction = 1;
    private float yOffsetPhase;

    [SerializeField] private SpriteRenderer _spriteRenderer;

    void Start()
    {
        direction = startDirection;
        yOffsetPhase = yOffsetSeed + Random.Range(0f, Mathf.PI * 2f);  // Индивидуальный сдвиг Y
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= 5f)
        {
            timer = 0f;
            // позже тут будем портить воду
        }

        // Движение по X
        float moveX = direction * swimSpeed * Time.deltaTime;
        Vector3 newPos = transform.position + new Vector3(moveX, 0f, 0f);

        // Границы X
        if (aquarium != null)
        {
            float left = aquarium.leftLimit;
            float right = aquarium.rightLimit;
            if (newPos.x > right)
            {
                newPos.x = right;
                direction = -1;
            }
            else if (newPos.x < left)
            {
                newPos.x = left;
                direction = 1;
            }
        }

        // Границы Y + индивидуальные колебания
        float yOffset = Mathf.Sin(Time.time * 2f + yOffsetPhase) * 0.5f;
        if (aquarium != null)
        {
            float bottom = aquarium.bottomLimit;
            float top = aquarium.topLimit;
            newPos.y = Mathf.Clamp(yOffset, bottom, top);
        }
        else
        {
            newPos.y = yOffset;
        }

        transform.position = newPos;
        float tailScale = 1f + Mathf.Sin(Time.time * 4f + yOffsetPhase) * 0.1f;  // Асинхронный хвост
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(tailScale) * (direction > 0 ? 1 : -1);
        transform.localScale = scale;

        _spriteRenderer.sortingOrder = Mathf.RoundToInt(transform.position.y * -100);
    }
}
