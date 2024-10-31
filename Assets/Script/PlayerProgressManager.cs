using UnityEngine;
using System.IO;
using System.Collections.Generic;
public class PlayerProgressManager : MonoBehaviour
{
    public int level = 1;
    public float experience = 0f;
    public float experienceToNextLevel = 100f;
    public int coins = 0;
    public Player player;

    private ItemCatalog itemCatalog;
    private string saveFilePath;
    public List<Item> inventory = new List<Item>(); // Инвентарь игрока

    void Start()
    {
        saveFilePath = Path.Combine(Application.dataPath, "Data/playerProgress.json");
        LoadPlayerProgress();
    }

    public void AddExperience(float amount)
    {
        experience += amount;
        if (experience >= experienceToNextLevel)
        {
            LevelUp();
        }
        SavePlayerProgress();
    }

    private void LevelUp()
    {
        level++;
        experience = experience - experienceToNextLevel;
        experienceToNextLevel *= 1.5f;
        player.maxHealth += 10f;
        player.health = player.maxHealth;
        SavePlayerProgress();
        Debug.Log($"Поздравляем! Вы достигли уровня {level}");
    }

    // Метод для добавления предмета в инвентарь
    public void AddItemToInventory(string itemName, int level, float damage, float cooldown, float multiplier)
    {
        Item item = inventory.Find(i => i.itemName == itemName);
        if (item != null)
        {
            Debug.Log($"Предмет {itemName} уже есть в инвентаре.");
        }
        else
        {
            inventory.Add(new Item { itemName = itemName, level = level, damage = damage, cooldown = cooldown, multiplier = multiplier });
            SavePlayerProgress();
            Debug.Log($"Предмет {itemName} добавлен в инвентарь.");
        }
    }

    public void SavePlayerProgress()
    {
        PlayerData data = new PlayerData
        {
            level = this.level,
            experience = this.experience,
            experienceToNextLevel = this.experienceToNextLevel,
            maxHealth = player.maxHealth,
            health = player.health,
            attackSpeed = player.attackSpeed,
            coins = this.coins,
            inventory = this.inventory // Сохраняем инвентарь
        };

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(saveFilePath, json);
        Debug.Log("Прогресс игрока сохранен.");
    }

    public void LoadPlayerProgress()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            PlayerData data = JsonUtility.FromJson<PlayerData>(json);

            this.level = data.level;
            this.experience = data.experience;
            this.experienceToNextLevel = data.experienceToNextLevel;
            player.maxHealth = data.maxHealth;
            player.health = data.health;
            player.attackSpeed = data.attackSpeed;
            this.coins = data.coins;
            this.inventory = data.inventory; // Загружаем инвентарь

            player.UpdateHealthUI();
            Debug.Log("Прогресс игрока загружен.");
        }
        else
        {
            Debug.Log("Файл с прогрессом не найден, загружаются значения по умолчанию.");
        }
    }

    // Метод для использования предмета из инвентаря
    public void UseItem(string itemName)
    {
        Item item = inventory.Find(i => i.itemName == itemName);
        if (item != null)
        {
            float effectiveDamage = item.damage * item.multiplier; // Рассчитываем урон с учетом множителя
            Debug.Log($"Использован предмет: {itemName}. Уровень: {item.level}, Урон: {effectiveDamage}, Перезарядка: {item.cooldown} секунд.");
            // Логика использования предмета (например, атака или повышение характеристик)
            SavePlayerProgress();
        }
        else
        {
            Debug.Log($"Предмет {itemName} не найден.");
        }
    }

    // Метод для улучшения уровня предмета
    public void UpgradeItem(string itemName)
    {
        Item item = inventory.Find(i => i.itemName == itemName);
        if (item != null)
        {
            item.level++;
            item.damage += 5f; // Например, увеличиваем урон при улучшении
            item.cooldown -= 0.1f; // Уменьшаем время перезарядки
            item.multiplier += 0.2f; // Увеличиваем множитель
            SavePlayerProgress();
            Debug.Log($"Предмет {itemName} улучшен до уровня {item.level}. Урон: {item.damage}, Множитель: {item.multiplier}, Перезарядка: {item.cooldown} секунд.");
        }
        else
        {
            Debug.Log($"Предмет {itemName} не найден.");
        }
    }

    // Метод для добавления предмета в инвентарь из справочника
    public void AddItemToInventory(string itemName)
    {
        Item itemFromCatalog = itemCatalog.GetItem(itemName);
        if (itemFromCatalog != null)
        {
            Item itemInInventory = inventory.Find(i => i.itemName == itemName);
            if (itemInInventory != null)
            {
                Debug.Log($"Предмет {itemName} уже есть в инвентаре.");
            }
            else
            {
                // Добавляем предмет в инвентарь, клонируя его из справочника
                inventory.Add(new Item
                {
                    itemName = itemFromCatalog.itemName,
                    level = itemFromCatalog.level,
                    damage = itemFromCatalog.damage,
                    cooldown = itemFromCatalog.cooldown,
                    multiplier = itemFromCatalog.multiplier
                });
                SavePlayerProgress();
                Debug.Log($"Предмет {itemName} добавлен в инвентарь.");
            }
        }
    }
}