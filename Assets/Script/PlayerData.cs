using System.Collections.Generic;
[System.Serializable]
public class PlayerData
{
    public int level = 1;
    public float experience = 0f;
    public float experienceToNextLevel = 100f;
    public int coins = 0;
    public float maxHealth = 100f;
    public float health = 100f;
    public float attackSpeed = 1f;

    public List<Item> inventory = new List<Item>(); // Инвентарь игрока
}

