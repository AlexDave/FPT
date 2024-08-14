using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public float maxHealth = 100f;
    public float health = 100f;
    public float attackSpeed = 1f;

    public float currentDamage = 0f; // ������� �������� �����

    public Button[] damageButtons; // ������ ������ ��� ������� �������� �����
    public float[] damageValues; // ������ �������� �����, ��������������� �������

    public Text healthText; // ������ �� ��������� ������� ��� ����������� ������

    private LevelManager levelManager;

    void Start()
    {
        health = maxHealth;
        SetDamage(10);
        UpdateHealthUI(); // ��������� UI ��� ������

        levelManager = FindObjectOfType<LevelManager>();
        if (levelManager == null)
        {
            Debug.LogError("LevelManager �� ������ � �����.");
        }

        // ��������� � ��������� ���������� ��� ������, ���� ��� �����������
        if (damageButtons.Length == damageValues.Length)
        {
            for (int i = 0; i < damageButtons.Length; i++)
            {
                int index = i; // ��������� ������� �������� i
                damageButtons[i].onClick.AddListener(() => SetDamage(damageValues[index]));
            }
        }
        else
        {
            Debug.LogError("������� damageButtons � damageValues ������ ����� ���������� �����.");
        }
    }

    void Update()
    {
        // ������������ ���� �����
        if (Input.GetMouseButtonDown(0)) // ����� ������ ����
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // ������� ��� �� ������
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit)) // ���������, ����� �� ��� � ������
            {
                Enemy enemy = hit.collider.GetComponent<Enemy>(); // �������� ��������� Enemy
                if (enemy != null) // ���� ������, � ������� ����� ���, �������� ������, ������� ����
                {
                    enemy.TakeDamage(currentDamage);
                }
            }
        }
    }

    public void TakeDamage(float amount)
    {
        Debug.Log("��������� ����� " + amount);
        health -= amount;
        health = Mathf.Clamp(health, 0, maxHealth); // ���������� ��������, ����� �� ���� ������������� ��������
        UpdateHealthUI(); // ��������� UI ����� ��������� �����
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (levelManager != null)
        {
            levelManager.PlayerDeath(); // ���������� LevelManager � ������ ������
        }
        // ������������� ����
        Time.timeScale = 0f;
    }

    // ����� ��� ��������� �������� ����� � ������� ������
    public void SetDamage(float damage)
    {
        currentDamage = damage;
        Debug.Log($"������� ���� ���������� ��: {currentDamage}");
    }

    // ����� ��� ���������� ������ UI � �������
    void UpdateHealthUI()
    {
        if (healthText != null)
        {
            healthText.text = $"Health: {health} / {maxHealth}";
        }
        else
        {
            Debug.LogError("HealthText �� ���������� � ����������.");
        }
    }
}
