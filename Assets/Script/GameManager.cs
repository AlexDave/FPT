using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public GameObject startGamePanel;    // ������ ������ ����
    public GameObject upgradePanel;      // ������ ��������
    public GameObject buttonPrefab;      // ������ ������
    public Transform weaponButtonsParent; // �������� ��� ������ ������
    public Transform healthButtonParent;  // �������� ��� ������ �������� ��������

    public int coins = 0;                // ������� ������ ������
    public int[] weaponLevels = new int[5]; // ������ �������� ������
    public int healthLevel = 1;          // ������� �������� ��������

    private Camera uiCamera;             // ������ UI
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

        // ������ "������ ����"
        Button startButton = startGamePanel.transform.Find("StartGameButton").GetComponent<Button>();
        startButton.onClick.AddListener(StartGame);

        // ������ "������ ��������"
        Button upgradeButton = startGamePanel.transform.Find("UpgradePanelButton").GetComponent<Button>();
        upgradeButton.onClick.AddListener(OpenUpgradePanel);
    }

    void FindUICamera()
    {
        uiCamera = Camera.main; // ��������������, ��� ��� ������ UI
        if (uiCamera != null)
        {
            uiCamera.gameObject.SetActive(true); // ���������, ��� ������ ������� ��� ������
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
            Debug.LogError("LevelManager �� ������ � �����.");
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
        // ������� ������ ������
        foreach (Transform child in weaponButtonsParent)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in healthButtonParent)
        {
            Destroy(child.gameObject);
        }

        // �������� ������ ��� �������� ������
        for (int i = 0; i < weaponLevels.Length; i++)
        {
            int weaponIndex = i; // ����������� ������ ��� ������������� � ������-���������
            GameObject button = Instantiate(buttonPrefab, weaponButtonsParent);
            Button buttonComponent = button.GetComponent<Button>();
            Text buttonText = button.GetComponentInChildren<Text>();

            int upgradeCost = weaponLevels[i] * 100;
            buttonText.text = $"Upgrade Weapon {i + 1}\nLevel {weaponLevels[i]}\nCost: {upgradeCost}";
            buttonComponent.onClick.RemoveAllListeners();
            buttonComponent.onClick.AddListener(() => UpgradeWeapon(weaponIndex));

            // ����������� ������ ��� ��, ��� � � ������� (���� �����, �������� ����������������)
            RectTransform rectTransform = button.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(0, -i * 60); // ������
        }

        // �������� ������ ��� �������� ��������
        GameObject healthButton = Instantiate(buttonPrefab, healthButtonParent);
        Button healthButtonComponent = healthButton.GetComponent<Button>();
        Text healthButtonText = healthButton.GetComponentInChildren<Text>();

        int healthUpgradeCost = healthLevel * 150;
        healthButtonText.text = $"Upgrade Health\nLevel {healthLevel}\nCost: {healthUpgradeCost}";
        healthButtonComponent.onClick.RemoveAllListeners();
        healthButtonComponent.onClick.AddListener(UpgradeHealth);

        // �������� ������ "�����"
        GameObject backButton = Instantiate(buttonPrefab, healthButtonParent);
        Button backButtonComponent = backButton.GetComponent<Button>();
        Text backButtonText = backButton.GetComponentInChildren<Text>();

        backButtonText.text = "Back";
        backButtonComponent.onClick.RemoveAllListeners();
        backButtonComponent.onClick.AddListener(BackToStartMenu);
    }

    void UpdateUI()
    {
        // ���������� ������ �� ������� ���������������
        foreach (Transform child in weaponButtonsParent)
        {
            Button button = child.GetComponent<Button>();
            Text buttonText = button.GetComponentInChildren<Text>();

            int weaponIndex = weaponButtonsParent.GetSiblingIndex(); // �������� ������ ������
            if (weaponIndex >= 0 && weaponIndex < weaponLevels.Length)
            {
                int upgradeCost = weaponLevels[weaponIndex] * 100;
                buttonText.text = $"Upgrade Weapon {weaponIndex + 1}\nLevel {weaponLevels[weaponIndex]}\nCost: {upgradeCost}";
            }
        }

        foreach (Transform child in healthButtonParent)
        {
            if (child.name == "HealthButton") // ���������, ��� ������ �������� ����� ������ ���
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
            UpdateUI(); // ��������� ����� �� ������� �����
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
            UpdateUI(); // ��������� ����� �� ������� �����
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
