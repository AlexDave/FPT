using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public float maxHealth = 100f;
    public float health = 100f;
    public float attackSpeed = 1f;

    public float currentDamage = 0f; // Текущее значение урона

    public Button[] damageButtons; // Массив кнопок для задания значений урона
    public float[] damageValues; // Массив значений урона, соответствующих кнопкам

    public Text healthText; // Ссылка на текстовый элемент для отображения жизней

    private LevelManager levelManager;

    void Start()
    {
        health = maxHealth;
        SetDamage(10);
        UpdateHealthUI(); // Обновляем UI при старте

        levelManager = FindObjectOfType<LevelManager>();
        if (levelManager == null)
        {
            Debug.LogError("LevelManager не найден в сцене.");
        }

        // Проверяем и назначаем слушателей для кнопок, если они установлены
        if (damageButtons.Length == damageValues.Length)
        {
            for (int i = 0; i < damageButtons.Length; i++)
            {
                int index = i; // Сохраняем текущее значение i
                damageButtons[i].onClick.AddListener(() => SetDamage(damageValues[index]));
            }
        }
        else
        {
            Debug.LogError("Массивы damageButtons и damageValues должны иметь одинаковую длину.");
        }
    }

    void Update()
    {
        // Обрабатываем ввод атаки
        if (Input.GetMouseButtonDown(0)) // Левая кнопка мыши
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // Создаем луч от камеры
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit)) // Проверяем, попал ли луч в объект
            {
                Enemy enemy = hit.collider.GetComponent<Enemy>(); // Получаем компонент Enemy
                if (enemy != null) // Если объект, в который попал луч, является врагом, наносим урон
                {
                    enemy.TakeDamage(currentDamage);
                }
            }
        }
    }

    public void TakeDamage(float amount)
    {
        Debug.Log("Получение урона " + amount);
        health -= amount;
        health = Mathf.Clamp(health, 0, maxHealth); // Ограниваем здоровье, чтобы не было отрицательных значений
        UpdateHealthUI(); // Обновляем UI после получения урона
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (levelManager != null)
        {
            levelManager.PlayerDeath(); // Уведомляем LevelManager о смерти игрока
        }
        // Останавливаем игру
        Time.timeScale = 0f;
    }

    // Метод для установки текущего урона с нажатия кнопки
    public void SetDamage(float damage)
    {
        currentDamage = damage;
        Debug.Log($"Текущий урон установлен на: {currentDamage}");
    }

    // Метод для обновления текста UI с жизнями
    void UpdateHealthUI()
    {
        if (healthText != null)
        {
            healthText.text = $"Health: {health} / {maxHealth}";
        }
        else
        {
            Debug.LogError("HealthText не установлен в инспекторе.");
        }
    }
}
