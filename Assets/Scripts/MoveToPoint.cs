using UnityEngine;
using Random = UnityEngine.Random; // Чтобы не путать с System.Random
using System.Linq;

public class MoveToPointBehavior : MonoBehaviour
{
    [Header("Настройки")]
    public Plant plant; // Ссылка на растение — теперь динамически выбирается
    public float speed = 1f; // Скорость движения рыбы
    public float feedingRadius = 0.2f; // Радиус "шумного" приближения к точке кормежки
    public float moveInterval = 5f; // Интервал между новыми точками
    public float stayDurationMin = 3f; // Мин. время стоянки на точке
    public float stayDurationMax = 5f; // Макс. время стоянки на точке

    private AquariumController _aquarium; // Контроллер аквариума
    private Transform _targetPoint; // Временная цель для движения
    private bool _moving; // Флаг, что рыба движется
    private float _stayTimer; // Таймер паузы на точке
    private float _timer; // Таймер до следующего движения

    public bool isMoving => _moving; // Публичное свойство для проверки движения

    void Start()
    {
        _aquarium = GetComponentInParent<AquariumController>(); // Находим контроллер аквариума сверху в иерархии
        _timer = moveInterval; // Инициализируем таймер
    }

    void Update()
    {
        if (_aquarium == null || _aquarium.plants == null || _aquarium.plants.Length == 0) return; 
        // Если нет растений — выходим, двигаться некуда

        // 1. ПАУЗА (stayTimer)
        if (_stayTimer > 0f)
        {
            _stayTimer -= Time.deltaTime; // Считаем паузу
            if (_stayTimer <= 0f)
            {
                _stayTimer = 0f;
                _moving = false; // Рыба перестает быть в движении

                // Если это компонент Fish — вызываем событие "поела"
                Fish fish = GetComponent<Fish>();
                if (fish != null) fish.OnEaten();
            }
            return; // Пока стоим, дальше не двигаемся
        }

        // 2. Ждем интервал → выбираем новую точку
        if (!_moving)
        {
            _timer -= Time.deltaTime;
            if (_timer <= 0f)
            {
                _timer = moveInterval;
                ChooseTargetPoint(); // Выбираем новую цель
            }
            return;
        }

        // 3. Движение к точке
        if (_targetPoint != null)
        {
            Vector3 dir = (_targetPoint.position - transform.position); // Направление к цели
            float dist = dir.magnitude; // Расстояние до цели

            if (dist > 0.3f) // Если не дошли
            {
                dir.Normalize(); // Делаем вектор единичной длины
                Vector3 newPos = transform.position + dir * (speed * Time.deltaTime); // Новая позиция

                // Ограничиваем движение по границам аквариума
                newPos.x = Mathf.Clamp(newPos.x, _aquarium.leftLimit + 0.5f, _aquarium.rightLimit - 0.5f);
                newPos.y = Mathf.Clamp(newPos.y, _aquarium.bottomLimit + 0.5f, _aquarium.topLimit - 0.5f);

                transform.position = newPos; // Применяем позицию
            }
            else // Дошли до точки
            {
                transform.position = _targetPoint.position;
                _stayTimer = Random.Range(stayDurationMin, stayDurationMax); // Случайная пауза
                Destroy(_targetPoint.gameObject); // Удаляем временную цель
                _targetPoint = null;

                Debug.Log($"🐠 {gameObject.name} ПРИБЫЛ — stay {_stayTimer:F1}s");
            }
        }
    }

    /// 🔥 Выбираем новую цель из любимых растений рыбы
    void ChooseTargetPoint()
    {
        Fish fish = GetComponent<Fish>();
        if (fish == null || fish.favoritePlants == null || fish.favoritePlants.Count == 0)
        {
            // Рыба не имеет любимых растений — просто не ест, плавает дальше
            Debug.LogWarning($"🐟 {gameObject.name} БЕЗ ЛЮБИМЫХ — НЕ ЕСТ!");
            return;
        }

        // Проверяем аквариум и наличие растений
        if (_aquarium == null || _aquarium.plants == null || _aquarium.plants.Length == 0)
        {
            // Растений нет, выходим, рыба будет вести себя как обычно
            return;
        }

        // Случайное любимое растение
        string chosenID = fish.favoritePlants[Random.Range(0, fish.favoritePlants.Count)];

        // Ищем растение в аквариуме
        Plant chosenPlant = _aquarium.plants.FirstOrDefault(p => p != null && p.plantID.Trim() == chosenID.Trim());
        if (chosenPlant == null)
        {
            Debug.LogWarning($"🐟 {gameObject.name} НЕ НАЙДЕН '{chosenID}' — НЕ ЕСТ!");
            return;
        }

        // Сохраняем plant для совместимости
        this.plant = chosenPlant;
        Debug.Log($"🐟 {gameObject.name} → {chosenPlant.plantID} из '{string.Join(",", fish.favoritePlants)}'");

        // Получаем случайную точку кормежки на растении
        Transform point = chosenPlant.GetRandomFeedingPoint();
        if (point == null) return; // Если точка не найдена, выходим

        // Создаем временную цель рядом с точкой
        GameObject target = new GameObject("TempTarget");
        target.transform.position = point.position + new Vector3(
            Random.Range(-feedingRadius, feedingRadius),
            Random.Range(-feedingRadius, feedingRadius),
            0);
        target.transform.parent = transform.parent;

        _targetPoint = target.transform;
        _moving = true;
    }
}
