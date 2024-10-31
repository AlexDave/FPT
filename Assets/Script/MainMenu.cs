using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;  // Для загрузки сцен
using UnityEngine.UI;              // Для работы с UI элементами

public class MenuMain : MonoBehaviour
{
    public GameObject mainPanel;
    public GameObject menuCamera;
    public GameObject startButton;
    public GameObject optionsButton;
    public GameObject exitButton;
    public TowerManager towerManager;
    public GameObject upgradePanel;  // Ссылка на панель улучшения

    public int upgradeCost = 10;  // Стоимость улучшения в монетах

    public void StartGame()
    {

        mainPanel.SetActive(false);
        menuCamera.SetActive(false);
        towerManager.StartFirstLevel();
    }

    public void GoToLastLevel()
    {
        // Замените на имя вашей сцены последнего уровня
        SceneManager.LoadScene("LastLevel");
    }

    public void SelectLevel(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }
    // Метод для улучшения игрока за монеты
    public void UpgradeMenu()
    {
        upgradePanel.SetActive(true);
        mainPanel.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

   
    
}
