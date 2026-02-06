using UnityEngine;

public class Fish : MonoBehaviour
{
    [Header("Movement")]
    public float swimSpeed = 1f;
    public int startDirection = 1;
    
    [Header("Y Offset (для асинхронности)")]
    public float yOffsetSeed = 0f;

    // Ссылка на аквариум
    public AquariumController aquarium;
    private int direction = 1;
    private float yOffsetPhase;

    void Start()
    {
        direction = startDirection;
        yOffsetPhase = yOffsetSeed + Random.Range(0f, Mathf.PI * 2f);
        if (aquarium == null)
        {
            aquarium = GetComponentInParent<AquariumController>();
        }
    }

    void Update()
    {
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
        // Только разворот (без хвоста)
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * (direction > 0 ? 1 : -1);
        transform.localScale = scale;
    }
}