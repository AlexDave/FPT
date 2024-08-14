using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public GameObject startGamePanel;    // Панель начала игры
    public GameObject upgradePanel;      // Панель прокачки
    public GameObject buttonPrefab;      // Префаб кнопки
    public Transform weaponButtonsParent; // Родитель для кнопок оружия
    public Transform healthButtonParent;  // Родитель для кнопки прокачки здоровья

    public int coins = 0;                // Текущие монеты игрока
    public int[] weaponLevels = new int[5]; // Уровни прокачки оружия
    public int healthLevel = 1;          // Уровень прокачки здоровья

    private Camera uiCamera;             // Камера UI
    private string saveFilePath;

    void Start()
    {
        LoadGame();
        UpdateUI();

        SetupStartGamePanel();
        FindUICamera();
    }

    void SetupStartGamePanel()
    {
        startGamePanel.SetActive(true);
        upgradePanel.SetActive(false);

        // Кнопка "Начать игру"
        Button startButton = startGamePanel.transform.Find("StartGameButton").GetComponent<Button>();
        startButton.onClick.AddListener(StartGame);

        // Кнопка "Панель прокачки"
        Button upgradeButton = startGamePanel.transform.Find("UpgradePanelButton").GetComponent<Button>();
        upgradeButton.onClick.AddListener(OpenUpgradePanel);
    }

    void FindUICamera()
    {
        uiCamera = Camera.main; // Предполагается, что это камера UI
        if (uiCamera != null)
        {
            uiCamera.gameObject.SetActive(true); // Убедитесь, что камера активна при старте
        }
    }

    void StartGame()
    {
        startGamePanel.SetActive(false);
        var levelManager = FindObjectOfType<LevelManager>();
        if (levelManager != null)
        {
            levelManager.StartLevel();
        }
        else
        {
            Debug.LogError("LevelManager не найден в сцене.");
        }

        if (uiCamera != null)
        {
            uiCamera.gameObject.SetActive(false);
        }
    }

    void OpenUpgradePanel()
    {
        startGamePanel.SetActive(false);
        upgradePanel.SetActive(true);
        CreateUpgradeButtons();
    }

    void CreateUpgradeButtons()
    {
        // Очистка старых кнопок
        foreach (Transform child in weaponButtonsParent)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in healthButtonParent)
        {
            Destroy(child.gameObject);
        }

        // Создание кнопок для прокачки оружия
        for (int i = 0; i < weaponLevels.Length; i++)
        {
            int weaponIndex = i; // Захватываем индекс для использования в лямбда-выражении
            GameObject button = Instantiate(buttonPrefab, weaponButtonsParent);
            Button buttonComponent = button.GetComponent<Button>();
            Text buttonText = button.GetComponentInChildren<Text>();

            int upgradeCost = weaponLevels[i] * 100;
            buttonText.text = $"Upgrade Weapon {i + 1}\nLevel {weaponLevels[i]}\nCost: {upgradeCost}";
            buttonComponent.onClick.RemoveAllListeners();
            buttonComponent.onClick.AddListener(() => UpgradeWeapon(weaponIndex));

            // Расположите кнопку так же, как и в префабе (если нужно, добавьте позиционирование)
            RectTransform rectTransform = button.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(0, -i * 60); // Пример
        }

        // Создание кнопки для прокачки здоровья
        GameObject healthButton = Instantiate(buttonPrefab, healthButtonParent);
        Button healthButtonComponent = healthButton.GetComponent<Button>();
        Text healthButtonText = healthButton.GetComponentInChildren<Text>();

        int healthUpgradeCost = healthLevel * 150;
        healthButtonText.text = $"Upgrade Health\nLevel {healthLevel}\nCost: {healthUpgradeCost}";
        healthButtonComponent.onClick.RemoveAllListeners();
        healthButtonComponent.onClick.AddListener(UpgradeHealth);

        // Создание кнопки "Назад"
        GameObject backButton = Instantiate(buttonPrefab, healthButtonParent);
        Button backButtonComponent = backButton.GetComponent<Button>();
        Text backButtonText = backButton.GetComponentInChildren<Text>();

        backButtonText.text = "Back";
        backButtonComponent.onClick.RemoveAllListeners();
        backButtonComponent.onClick.AddListener(BackToStartMenu);
    }

    void UpdateUI()
    {
        // Обновление текста на кнопках непосредственно
        foreach (Transform child in weaponButtonsParent)
        {
            Button button = child.GetComponent<Button>();
            Text buttonText = button.GetComponentInChildren<Text>();

            int weaponIndex = weaponButtonsParent.GetSiblingIndex(); // Получаем индекс кнопки
            if (weaponIndex >= 0 && weaponIndex < weaponLevels.Length)
            {
                int upgradeCost = weaponLevels[weaponIndex] * 100;
                buttonText.text = $"Upgrade Weapon {weaponIndex + 1}\nLevel {weaponLevels[weaponIndex]}\nCost: {upgradeCost}";
            }
        }

        foreach (Transform child in healthButtonParent)
        {
            if (child.name == "HealthButton") // Убедитесь, что кнопка здоровья имеет нужное имя
            {
                Button healthButton = child.GetComponent<Button>();
                Text healthButtonText = healthButton.GetComponentInChildren<Text>();

                int healthUpgradeCost = healthLevel * 150;
                healthButtonText.text = $"Upgrade Health\nLevel {healthLevel}\nCost: {healthUpgradeCost}";
            }
        }
    }

    void UpgradeWeapon(int weaponIndex)
    {
        int upgradeCost = weaponLevels[weaponIndex] * 100;
        if (coins >= upgradeCost)
        {
            coins -= upgradeCost;
            weaponLevels[weaponIndex]++;
            UpdateUI(); // Обновляем текст на кнопках сразу
            SaveGame();
        }
        else
        {
            Debug.Log("Not enough coins to upgrade weapon.");
        }
    }

    void UpgradeHealth()
    {
        int upgradeCost = healthLevel * 150;
        if (coins >= upgradeCost)
        {
            coins -= upgradeCost;
            healthLevel++;
            UpdateUI(); // Обновляем текст на кнопках сразу
            SaveGame();
        }
        else
        {
            Debug.Log("Not enough coins to upgrade health.");
        }
    }

    void BackToStartMenu()
    {
        upgradePanel.SetActive(false);
        startGamePanel.SetActive(true);
    }

    void SaveGame()
    {
        SaveData data = new SaveData
        {
            coins = this.coins,
            weaponLevels = this.weaponLevels,
            healthLevel = this.healthLevel
        };

        string json = JsonUtility.ToJson(data);
        System.IO.File.WriteAllText(saveFilePath, json);
    }

    void LoadGame()
    {
        if (System.IO.File.Exists(saveFilePath))
        {
            string json = System.IO.File.ReadAllText(saveFilePath);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            this.coins = data.coins;
            this.weaponLevels = data.weaponLevels;
            this.healthLevel = data.healthLevel;
        }
    }
}

[System.Serializable]
public class SaveData
{
    public int coins;
    public int[] weaponLevels;
    public int healthLevel;
}
