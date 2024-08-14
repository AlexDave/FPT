using UnityEngine;
using System.Collections.Generic;
using System.IO;

[CreateAssetMenu(fileName = "TowerLevel", menuName = "TowerLevel", order = 1)]
public class TowerLevel : ScriptableObject
{
    public string levelName;
    public GameObject levelPrefab;
    public GameObject playerPrefab;
    public GameObject enemyPrefab;
    public float enemyHealthMultiplier;
    public int numberOfEnemies;

    public static TowerLevel[] LoadTowerLevelsFromJSON(string path)
    {
        if (!File.Exists(path))
        {
            Debug.LogError("JSON file not found at path: " + path);
            return null;
        }

        string json = File.ReadAllText(path);
        Debug.Log(json);   
        TowerDataCollection levelDataCollection = JsonUtility.FromJson<TowerDataCollection>(json);

        TowerLevel[] towerLevels = new TowerLevel[levelDataCollection.levels.Length];

        for (int i = 0; i < levelDataCollection.levels.Length; i++)
        {
            TowerData data = levelDataCollection.levels[i];
            TowerLevel towerLevel = ScriptableObject.CreateInstance<TowerLevel>();
            towerLevel.levelName = data.levelName;
            towerLevel.levelPrefab = Resources.Load<GameObject>(data.levelPrefab);
            towerLevel.playerPrefab = Resources.Load<GameObject>(data.playerPrefab);
            towerLevel.enemyPrefab = Resources.Load<GameObject>(data.enemyPrefab);
            towerLevel.enemyHealthMultiplier = data.enemyHealthMultiplier;
            towerLevel.numberOfEnemies = data.numberOfEnemies;

            towerLevels[i] = towerLevel;
        }

        Debug.Log(towerLevels[1].ToString());
        return towerLevels;
    }
    public override string ToString()
    {
        return $"Level Name: {levelName}\n" +
               $"Level Prefab: {levelPrefab}\n" +
               $"Player Prefab: {playerPrefab}\n" +
               $"Enemy Prefab: {enemyPrefab}\n" +
               $"Enemy Health Multiplier: {enemyHealthMultiplier}\n" +
               $"Number of Enemies: {numberOfEnemies}";
    }
}
