using UnityEngine;

/// <summary>
/// Двигает рыбу в аквариуме на основе данных Fish и AquariumController.
/// </summary>
public class FishMovement : MonoBehaviour
{
    [Header("Скорость и базовое поведение")]
    public float swimSpeed;        // базовая скорость, задаётся в Inspector
    public int startDirection;     // 1 = вправо, -1 = влево
    public float yOffsetSeed;      // фазовый сдвиг для Y колебаний

    // 🔹 приватные поля для внутренней логики движения
    private float _baseSwimSpeed;
    private float _speedVariation;
    private int _direction;
    private float _yOffsetPhase;

    private Fish _fish;
    private AquariumController _aquarium;

    void Start()
    {
        _fish = GetComponent<Fish>();
        _aquarium = _fish.aquarium;

        _direction = startDirection;
        _yOffsetPhase = yOffsetSeed + Random.Range(0f, Mathf.PI * 2f);

        _baseSwimSpeed = swimSpeed;
        _speedVariation = Random.Range(0.7f, 1.3f);
    }

    void Update()
    {
        if (_aquarium == null) return;

        // текущая скорость = базовая × индивидуальность × дыхание
        float currentSpeed = _baseSwimSpeed * _speedVariation * 
                             (1f + Mathf.Sin(Time.time * 0.5f) * 0.1f);

        // движение по X
        float moveX = _direction * currentSpeed * Time.deltaTime;
        Vector3 newPos = transform.position + new Vector3(moveX, 0f, 0f);

        // отскок от стен
        if (newPos.x > _aquarium.rightLimit)
        {
            newPos.x = _aquarium.rightLimit;
            _direction = -1;
        }
        else if (newPos.x < _aquarium.leftLimit)
        {
            newPos.x = _aquarium.leftLimit;
            _direction = 1;
        }

        // движение по Y
        float yOffset;
        if (_fish.bottomDweller)
        {
            yOffset = _aquarium.bottomLimit + 0.9f +
                      Mathf.Sin(Time.time * 1f + _yOffsetPhase) * 0.1f;
        }
        else
        {
            yOffset = Mathf.Sin(Time.time * 2f + _yOffsetPhase) * 0.5f;
            yOffset = Mathf.Clamp(yOffset, _aquarium.bottomLimit, _aquarium.topLimit);
        }
        newPos.y = yOffset;

        transform.position = newPos;

        // разворот спрайта по направлению
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * (_direction > 0 ? 1 : -1);
        transform.localScale = scale;
    }
}