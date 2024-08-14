using UnityEngine;
using System;
using System.Collections;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    public float health = 50f;
    public float attackRate = 1f;
    public float detectionRange = 10f;
    public float moveSpeed = 2f; // Скорость движения врага
    public float avoidDistance = 2f; // Дистанция для обхода других врагов
    public float edgeThreshold = 0.5f; // Допустимое расстояние до границы зоны спавна
    public float movementTimeout = 2f; // Максимальное время для достижения цели
    public float collisionRadius = 1f; // Радиус для обнаружения столкновений с другими врагами
    public LayerMask wallLayer; // Слой стен для распознавания
    public float shieldHealth = 50f; // Максимальное здоровье щита
    public float shieldRechargeTime = 5f; // Время восстановления щита после его использования
    public float initialAttackDelay = 2f; // Задержка перед началом атаки

    private bool hasShield = false;
    private float currentShieldHealth;
    private Vector3 spawnCenter;
    private Vector3 spawnSize;
    private Player player;

    private enum EnemyType
    {
        RegularDamage,
        StrongDamage,
        Shielded,
        MovingAndAttacking
    }

    [SerializeField] private EnemyType enemyType;
    [SerializeField] private int regularDamage = 10;
    [SerializeField] private int strongDamage = 30;

    public event Action OnEnemyDeath;

    public void Initialize(Player player, Vector3 spawnCenter, Vector3 spawnSize)
    {
        this.player = player;
        this.spawnCenter = spawnCenter;
        this.spawnSize = spawnSize;

        if (spawnCenter == null || spawnSize == null)
        {
            Debug.LogError("spawnCenter or spawnSize is null.");
            return;
        }

        enemyType = (EnemyType)UnityEngine.Random.Range(0, Enum.GetValues(typeof(EnemyType)).Length);
        currentShieldHealth = shieldHealth; // Инициализация текущего здоровья щита

        StopAllCoroutines();
        StartCoroutine(StartAttackingAfterDelay(initialAttackDelay));
    }

    private IEnumerator StartAttackingAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        switch (enemyType)
        {
            case EnemyType.RegularDamage:
                StartCoroutine(RegularDamageRoutine());
                break;
            case EnemyType.StrongDamage:
                StartCoroutine(StrongDamageRoutine());
                break;
            case EnemyType.Shielded:
                StartCoroutine(ShieldedRoutine());
                break;
            case EnemyType.MovingAndAttacking:
                StartCoroutine(MovingAndAttackingRoutine());
                break;
        }
    }

    private IEnumerator RegularDamageRoutine()
    {
        while (true)
        {
            AttackPlayer(regularDamage);
            yield return new WaitForSeconds(attackRate);
        }
    }

    private IEnumerator StrongDamageRoutine()
    {
        while (true)
        {
            AttackPlayer(regularDamage);
            yield return new WaitForSeconds(attackRate);

            if (Time.time % 3f < 1f)
            {
                AttackPlayer(strongDamage);
            }
        }
    }

    private IEnumerator ShieldedRoutine()
    {
        while (true)
        {
            AttackPlayer(regularDamage);
            yield return new WaitForSeconds(attackRate);

            if (Time.time % 5f < 1f && !hasShield)
            {
                ActivateShield();
            }
        }
    }

    private IEnumerator MovingAndAttackingRoutine()
    {
        while (true)
        {
            Vector3 targetPosition = GetRandomPointInBox(spawnCenter, spawnSize);
            yield return StartCoroutine(MoveToPosition(targetPosition));
            yield return new WaitForSeconds(1f);
            AttackPlayer(regularDamage);
        }
    }

    private IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        Vector3 startPosition = transform.position;
        float startTime = Time.time;

        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            if (Time.time - startTime > movementTimeout)
            {
                break;
            }

            Collider[] hitColliders = Physics.OverlapSphere(transform.position, collisionRadius);
            Vector3 moveDirection = (targetPosition - transform.position).normalized;

            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider != null && hitCollider != this.GetComponent<Collider>())
                {
                    Vector3 avoidDirection = transform.position - hitCollider.transform.position;
                    avoidDirection.y = 0; // Зафиксируем перемещение по оси Y
                    moveDirection += avoidDirection.normalized * (avoidDistance / Vector3.Distance(transform.position, hitCollider.transform.position));
                }
            }

            if (IsNearEdge(transform.position))
            {
                Vector3 directionFromCenter = (transform.position - spawnCenter).normalized;
                targetPosition -= directionFromCenter * moveSpeed * Time.deltaTime;
            }

            if (CheckWallProximity())
            {
                moveDirection = CorrectPathAroundWall();
            }

            Vector3 newPosition = transform.position + moveDirection * moveSpeed * Time.deltaTime;
            newPosition.y = startPosition.y;

            newPosition.x = Mathf.Clamp(newPosition.x, spawnCenter.x - spawnSize.x / 2, spawnCenter.x + spawnSize.x / 2);
            newPosition.z = Mathf.Clamp(newPosition.z, spawnCenter.z - spawnSize.z / 2, spawnCenter.z + spawnSize.z / 2);

            transform.position = newPosition;
            yield return null;
        }
    }

    private bool IsNearEdge(Vector3 position)
    {
        return Mathf.Abs(position.x - spawnCenter.x) > spawnSize.x / 2 - edgeThreshold ||
               Mathf.Abs(position.z - spawnCenter.z) > spawnSize.z / 2 - edgeThreshold;
    }

    private bool CheckWallProximity()
    {
        Vector3 direction = transform.forward;
        return Physics.Raycast(transform.position, direction, avoidDistance, wallLayer);
    }

    private Vector3 CorrectPathAroundWall()
    {
        Vector3 directionToTarget = (transform.forward).normalized;
        Vector3 leftDirection = Quaternion.Euler(0, -90, 0) * directionToTarget;
        Vector3 rightDirection = Quaternion.Euler(0, 90, 0) * directionToTarget;

        if (Physics.Raycast(transform.position, leftDirection, avoidDistance, wallLayer))
        {
            return rightDirection;
        }

        if (Physics.Raycast(transform.position, rightDirection, avoidDistance, wallLayer))
        {
            return leftDirection;
        }

        return directionToTarget;
    }

    private void Update()
    {
        if (health <= 0)
        {
            Die();
        }
    }

    public void TakeDamage(float amount)
    {
        if (hasShield)
        {
            float damageToShield = Mathf.Min(amount, currentShieldHealth);
            currentShieldHealth -= damageToShield;
            amount -= damageToShield;

            if (currentShieldHealth <= 0)
            {
                // Щит исчерпан, начинаем восстановление
                StartCoroutine(RestoreShieldAfterDelay());
                hasShield = false;
                currentShieldHealth = 0;
            }
        }

        health -= amount;
        if (health <= 0)
        {
            Die();
        }
    }

    private IEnumerator RestoreShieldAfterDelay()
    {
        yield return new WaitForSeconds(shieldRechargeTime); // Время восстановления щита
        currentShieldHealth = shieldHealth;
        hasShield = true;
        Debug.Log("Shield restored!");
    }

    private void Die()
    {
        OnEnemyDeath?.Invoke();
        Destroy(gameObject);
    }

    private void AttackPlayer(float damage)
    {
        Debug.Log("Attack " + damage);
        if (player != null)
        {
            player.TakeDamage(damage);
        }
    }

    private void ActivateShield()
    {
        hasShield = true;
        currentShieldHealth = shieldHealth; // Устанавливаем максимальное здоровье щита
        Debug.Log("Shield activated!");
    }

    private Vector3 GetRandomPointInBox(Vector3 center, Vector3 size)
    {
        float x = Random.Range(center.x - size.x / 2, center.x + size.x / 2);
        float y = center.y;
        float z = Random.Range(center.z - size.z / 2, center.z + size.z / 2);

        return new Vector3(x, y, z);
    }

    // Новый метод для установки множителя скорости
    public void SetSpeedMultiplier(float multiplier)
    {
        moveSpeed *= multiplier;
    }

    // Убедитесь, что это публичные методы
    public void SetHealth(float newHealth)
    {
        health = newHealth;
    }

    public void SetShieldHealth(float newShieldHealth)
    {
        shieldHealth = newShieldHealth;
        currentShieldHealth = newShieldHealth; // Сначала устанавливаем текущее здоровье щита равным новому максимальному
    }
}
