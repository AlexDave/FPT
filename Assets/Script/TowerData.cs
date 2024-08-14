[System.Serializable]
public class TowerData
{
    public string levelName;
    public string levelPrefab;
    public string playerPrefab;
    public string enemyPrefab;
    public float enemyHealthMultiplier;
    public int numberOfEnemies;
}

[System.Serializable]
public class TowerDataCollection
{
    public TowerData[] levels;
}