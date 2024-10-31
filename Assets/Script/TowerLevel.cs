using UnityEngine;
using System.IO;

[CreateAssetMenu(fileName = "TowerLevel", menuName = "TowerLevel", order = 1)]
public class TowerLevel : ScriptableObject
{
    public string nameTower;          // Название башни
    public float multiplayer;         // Множитель сложности
    public GameObject levelPrefab;    // Префаб уровня
    public GameObject playerPrefab;
    public GameObject enemyPrefab;
    public FloorData[] floors;        // Массив данных о этажах

    public static TowerLevel[] LoadTowerLevelsFromJSON(string path)
    {
        if (!File.Exists(path))
        {
            Debug.LogError($"JSON file not found at path: {path}");
            return null;
        }

        string json = File.ReadAllText(path);
        Debug.Log($"Loaded JSON: {json}");

        TowerDataCollection towerDataCollection = JsonUtility.FromJson<TowerDataCollection>(json);

        if (towerDataCollection == null || towerDataCollection.towers == null)
        {
            Debug.LogError("Failed to parse JSON data.");
            return null;
        }

        TowerLevel[] towerLevels = new TowerLevel[towerDataCollection.towers.Length];

        for (int i = 0; i < towerDataCollection.towers.Length; i++)
        {
            TowerData data = towerDataCollection.towers[i];
            TowerLevel towerLevel = CreateInstance<TowerLevel>();
            towerLevel.nameTower = data.towerInfo.name;
            towerLevel.levelPrefab = Resources.Load<GameObject>(data.towerInfo.levelPrefab);
            towerLevel.playerPrefab = Resources.Load<GameObject>("Prefab/Player");
            towerLevel.enemyPrefab = Resources.Load<GameObject>("Prefab/Enemy");
            towerLevel.multiplayer = data.towerInfo.multiplayer;
            towerLevel.floors = data.floors;

            towerLevels[i] = towerLevel;
        }

        return towerLevels;
    }
}