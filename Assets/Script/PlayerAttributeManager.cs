using UnityEngine;
using System.IO;

public class PlayerAttributeManager : MonoBehaviour
{
    private string saveFilePath;
    public PlayerData playerData; // Ссылка на PlayerData

    void Start()
    {
        saveFilePath = Path.Combine(Application.dataPath, "Data/playerProgress.json");
        LoadPlayerData(); // Загружаем данные игрока при старте
    }

    // Метод для загрузки данных игрока из JSON
    public void LoadPlayerData()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            playerData = JsonUtility.FromJson<PlayerData>(json);
            Debug.Log("Данные игрока загружены.");
        }
        else
        {
            Debug.Log("Файл с данными игрока не найден, загружаются значения по умолчанию.");
            playerData = new PlayerData(); // Инициализация с дефолтными значениями
        }
    }

    // Метод для сохранения данных игрока в JSON
    public void SavePlayerData()
    {
        string json = JsonUtility.ToJson(playerData, true);
        File.WriteAllText(saveFilePath, json);
        Debug.Log("Данные игрока сохранены.");
    }

    // Метод для прокачки определенной характеристики игрока, вызываемый из UI
    public void UpgradeAttribute(string attributeName)
    {
        int cost = 10; // Устанавливаем фиксированную стоимость улучшения

        if (playerData.coins >= cost)
        {
            playerData.coins -= cost;

            switch (attributeName.ToLower())
            {
                case "health":
                    playerData.maxHealth += 10f; // Увеличиваем максимальное здоровье
                    playerData.health = playerData.maxHealth; // Полное восстановление здоровья
                    Debug.Log("Здоровье улучшено. Текущее ХП " + playerData.health);
                    break;
                case "attackspeed":
                    playerData.attackSpeed += 0.1f; // Увеличиваем скорость атаки
                    Debug.Log("Скорость атаки улучшена.");
                    break;
                case "level":
                    playerData.level += 1; // Увеличиваем уровень
                    Debug.Log("Уровень повышен.");
                    break;
                default:
                    Debug.LogWarning("Неизвестная характеристика: " + attributeName);
                    break;
            }

            SavePlayerData(); // Сохранение изменений
        }
        else
        {
            Debug.Log("Недостаточно монет для улучшения.");
        }
    }
}
