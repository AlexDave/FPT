using UnityEngine;

public class TowerManager : MonoBehaviour
{
    public LevelManager levelManager; // Ссылка на LevelManager

    private string jsonFilePath = "Data/TowerLevels.json"; // Путь к JSON файлу

    private TowerLevel[] towerLevels; // Массив уровней башен
    private TowerLevel currentTower; // Текущий уровень башни

    private int currentTowerIndex = 0; // Индекс текущей башни
    private int currentFloorIndex = 0; // Индекс текущего этажа
    private int currentLevelIndex = 0; // Индекс текущего уровня

    

    void Start()
    {
        // Загружаем данные башен из JSON-файла при старте
        towerLevels = TowerLevel.LoadTowerLevelsFromJSON(Application.dataPath + "/"  + jsonFilePath);

    }

    public void StartFirstLevel()
    {
        // Устанавливаем первую башню, этаж и уровень
        currentTowerIndex = 0;
        currentFloorIndex = 0;
        currentLevelIndex = 0;

        // Загружаем данные первого уровня первой башни
        LoadTower(0);
    }

    public void LoadTower(int indexTower)
    {
        currentTower = towerLevels[indexTower];
        
        // Инициализируем уровень через LevelManager
        if (levelManager != null)
        {
            levelManager.Initialize(currentTower.levelPrefab, currentTower.playerPrefab, currentTower.enemyPrefab, currentTower);
        }
        else
        {
            Debug.LogError("LevelManager не назначен в TowerManager.");
        }
    }

}
