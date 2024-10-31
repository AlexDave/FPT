using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
        private GameObject currentLevel;
    private GameObject previousLevel;
    private GameObject player;
    private float enemyMultiplier = 1.0f;
    private int enemiesKilled = 0;

    private Vector3 previousFinishPosition; // Позиция точки Finish предыдущего уровня
    private Vector3 previousFloorSize; // Размер "Floor" предыдущего уровня
    private Vector3 previousFloorScale; // Масштаб "Floor" предыдущего уровня

    private const float waitTime = 2f;
    private const float playerHeightOffset = 2.2f;
    private const int numberOfEnemies = 5;
    private const float minimumDistance = 1f;
    private const float gridSize = 2.0f;


    public DeathMenu deathMenu;
    private Transform enemySpawnArea;
    private List<GameObject> existingEnemies = new List<GameObject>();
    private int currentLevelIndex = 0; // Индекс текущего уровня
    private int currentFloorIndex = 0; // Индекс текущего уровня

    private GameObject enemyPrefab;
    private TowerLevel currentTowerLevel;
    private FloorData currentFloorData;
    private LevelData currentLevelData;
   


    public void Initialize(GameObject levelPrefab, GameObject playerPrefab, GameObject enemyPrefab, TowerLevel currentTowerLevel)
    {

        CleanUp(); // Удаляем старые объекты перед началом нового уровня

        this.enemyPrefab = enemyPrefab;
        this.currentTowerLevel = currentTowerLevel;
        this.currentFloorData = currentTowerLevel.floors[0];
        this.currentLevelData = currentFloorData.levels[0];

        // Используем информацию из TowerLevel для спавна уровня, игрока и врагов
        SpawnLevel(levelPrefab);
        SpawnPlayer(playerPrefab);
        UpdateUILevel();
        SpawnEnemies(enemyPrefab, currentLevelData.countOfEnemy);
        SubscribeToEnemyEvents(); // Подписка на события смерти врагов
    }

    void SpawnLevel(GameObject levelPrefab)
    {
        if (levelPrefab != null)
        {
            // Если это не первый уровень, корректируем позицию нового уровня
            if (currentLevel != null)
            {
                // Сохраняем позицию точки Finish и размеры "Floor" текущего уровня
                Transform finishPoint = currentLevel.transform.Find("Finish");
                Transform floorTransform = currentLevel.transform.Find("Floor");

                if (finishPoint != null)
                {
                    previousFinishPosition = finishPoint.position;
                }
                else
                {
                    Debug.LogWarning("Finish point not found in the current level.");
                }

                if (floorTransform != null)
                {
                    previousFloorSize = new Vector3(0,0,floorTransform.localScale.z * 5);
                    
                }
                else
                {
                    Debug.LogWarning("Floor object not found in the current level.");
                }
            }

            // Устанавливаем позицию нового уровня
            Vector3 spawnPosition = previousFinishPosition;

            if (previousFloorSize != Vector3.zero)
            {
                spawnPosition += previousFloorSize;
            }

            if(currentLevelIndex >= currentFloorData.levels.Length)
            {
                currentLevelIndex = 0;
                currentFloorIndex++;

                this.currentFloorData = currentTowerLevel.floors[currentFloorIndex];
                this.currentLevelData = currentFloorData.levels[currentLevelIndex];
                
            }
            else
            {
                this.currentLevelData = currentFloorData.levels[currentLevelIndex];
            }

            if (currentFloorIndex > currentTowerLevel.floors.Length)
            {
                Debug.Log("Прошел башню");
                return;
            }

            
            currentLevel = Instantiate(levelPrefab, spawnPosition, Quaternion.identity);
            enemySpawnArea = currentLevel.transform.Find("SpawnEnemy");
                        
        }
    }

    void SpawnPlayer(GameObject playerPrefab)
    {
        if (playerPrefab != null)
        {
            player = Instantiate(playerPrefab);
            player.transform.position = currentLevel.transform.Find("Spawn").position;
            player.transform.position += new Vector3(0, playerHeightOffset, 0);
        }
    }

    void SpawnEnemies(GameObject enemyPrefab, int numberOfEnemies)
    {
        existingEnemies = new List<GameObject>();
        if (enemyPrefab == null || enemySpawnArea == null) return;

        Player playerComponent = player.GetComponent<Player>();

        BoxCollider spawnAreaCollider = enemySpawnArea.GetComponent<BoxCollider>();
        if (spawnAreaCollider == null)
        {
            Debug.LogWarning("Enemy spawn area does not have a BoxCollider.");
            return;
        }

        Vector3 spawnAreaCenter = spawnAreaCollider.transform.position;
        Vector3 spawnAreaSize = spawnAreaCollider.size;

        List<Vector3> potentialSpawnPoints = GenerateGridPoints(spawnAreaCenter, spawnAreaSize);

        int enemiesSpawned = 0;

        while (enemiesSpawned < numberOfEnemies && potentialSpawnPoints.Count > 0)
        {
            Vector3 spawnPosition = potentialSpawnPoints[Random.Range(0, potentialSpawnPoints.Count)];
            if (IsPositionValid(spawnPosition, enemyPrefab))
            {
                GameObject enemySpawned = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
                existingEnemies.Add(enemySpawned);
                enemiesSpawned++;

                Enemy enemyScript = enemySpawned.GetComponent<Enemy>();
                if (enemyScript != null)
                {
                    enemyScript.SetHealth(enemyScript.health * enemyMultiplier);
                    enemyScript.SetShieldHealth(enemyScript.shieldHealth * enemyMultiplier);
                    enemyScript.Initialize(playerComponent, spawnAreaCenter, spawnAreaSize); // Передача ссылки на игрока
                }
            }
            potentialSpawnPoints.Remove(spawnPosition);
        }
    }

    void SubscribeToEnemyEvents()
    {
        foreach (GameObject enemyObject in existingEnemies)
        {
            Enemy enemyScript = enemyObject.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                enemyScript.OnEnemyDeath += OnEnemyKilled; // Подписка на событие смерти
            }
            else
            {
                Debug.LogWarning($"Enemy script not found on the instantiated enemy: {enemyObject.name}");
            }
        }
    }

    List<Vector3> GenerateGridPoints(Vector3 center, Vector3 size)
    {
        List<Vector3> points = new List<Vector3>();
        float halfWidth = size.x / 2;
        float halfDepth = size.z / 2;

        for (float x = center.x - halfWidth; x < center.x + halfWidth; x += gridSize)
        {
            for (float z = center.z - halfDepth; z < center.z + halfDepth; z += gridSize)
            {
                points.Add(new Vector3(x, center.y, z));
            }
        }

        return points;
    }

    bool IsPositionValid(Vector3 position, GameObject enemyPrefab)
    {
        float enemyRadius = enemyPrefab.GetComponent<Collider>().bounds.extents.magnitude;
        foreach (GameObject enemy in existingEnemies)
        {
            if (Vector3.Distance(position, enemy.transform.position) < minimumDistance + enemyRadius)
            {
                return false;
            }
        }

        // Проверка на отступ от стен
        if (IsNearWall(position, enemyRadius))
        {
            return false;
        }

        return true;
    }

    bool IsNearWall(Vector3 position, float radius)
    {
        BoxCollider spawnAreaCollider = enemySpawnArea.GetComponent<BoxCollider>();
        Vector3 halfSize = spawnAreaCollider.size / 2;
        Vector3 center = spawnAreaCollider.transform.position;

        // Учитываем радиус врага при проверке близости к краям зоны спавна
        Vector3 min = center - halfSize + new Vector3(radius, 0, radius);
        Vector3 max = center + halfSize - new Vector3(radius, 0, radius);

        return position.x <= min.x ||
               position.x >= max.x ||
               position.z <= min.z ||
               position.z >= max.z;
    }


    void OnEnemyKilled()
    {
        enemiesKilled++;
        if (enemiesKilled >= currentLevelData.countOfEnemy)
        {
            SpawnNextLevel();
            enemiesKilled = 0; // Сбрасываем счетчик убитых врагов
        }
    }

    IEnumerator MovePlayerToWaypoint(Transform player, Transform waypoint, GameObject previousLevel)
    {
        float elapsedTime = 0f;
        Vector3 startingPos = player.position;
        Vector3 targetPos = waypoint.position + new Vector3(0, playerHeightOffset, 0);

        while (elapsedTime < waitTime)
        {
            player.position = Vector3.Lerp(startingPos, targetPos, (elapsedTime / waitTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        player.position = targetPos;  // Гарантируем, что игрок достиг точки


        SpawnEnemies(enemyPrefab, currentLevelData.countOfEnemy); // Спавн врагов
        SubscribeToEnemyEvents(); // Подписка на события смерти
        Destroy(previousLevel); // Удаляем старый уровень

    }

    void SpawnNextLevel()
    {
        previousLevel = currentLevel;
        enemyMultiplier = currentTowerLevel.multiplayer * currentFloorData.floorInfo.multiplayer;

        currentLevelIndex++;

        // Сохраняем позицию точки Finish текущего уровня
        Transform finishPoint = currentLevel.transform.Find("Finish");
        if (finishPoint != null)
        {
            previousFinishPosition = finishPoint.position;
        }

        SpawnLevel(currentTowerLevel.levelPrefab); // Спавн нового уровня
        UpdateUILevel();
        Debug.Log("Этаж:" + currentFloorIndex);
        Debug.Log("Уровень:" + currentLevelIndex);

        Transform nextWaypoint = currentLevel.transform.Find("Spawn"); // Перемещаем игрока на начало нового уровня

        StartCoroutine(MovePlayerToWaypoint(player.transform, nextWaypoint, previousLevel));
        
    }

    public void PlayerDeath()
    {
        if (deathMenu != null)
        {
            deathMenu.ShowDeathMenu();
        }
    }

    public void CleanUp()
    {
        if (currentLevel != null)
        {
            Destroy(currentLevel);
        }
        if (player != null)
        {
            Destroy(player);
        }
        foreach (var enemy in existingEnemies)
        {
            if (enemy != null)
            {
                Destroy(enemy);
            }
        }
        existingEnemies.Clear();
        enemiesKilled = 0;
        currentLevelIndex = 0;
    }

    private void OnDestroy()
    {
        foreach (Enemy enemy in FindObjectsOfType<Enemy>())
        {
            enemy.OnEnemyDeath -= OnEnemyKilled;
        }
    }

    private void UpdateUILevel()
    {
        Player playerComponent = player.GetComponent<Player>();
        playerComponent.SetLevelUI(currentFloorIndex, currentLevelIndex);
    }
}
