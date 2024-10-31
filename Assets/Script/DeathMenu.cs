using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathMenu : MonoBehaviour
{
    public GameObject deathMenuUI; // Ссылка на объект
    public GameObject mainMenuUI; // Ссылка на объект
    public TowerManager towerManager;
    public LevelManager levelManager;
    public GameObject menuCamera;


    void Start()
    {
        if (deathMenuUI != null)
        {
            deathMenuUI.SetActive(false); // Скрываем меню смерти при старте
        }
        else
        {
            Debug.LogError("DeathMenuUI не назначен в инспекторе.");
        }
    }

    // Метод для отображения меню смерти
    public void ShowDeathMenu()
    {

        if (deathMenuUI != null)
        {
            deathMenuUI.SetActive(true); // Активируем
            Time.timeScale = 0f; // Останавливаем игру
        }
        else
        {
            Debug.LogError("DeathMenuUI не назначен в инспекторе.");
        }
    }

    // Метод для перезапуска текущего уровня
    public void RestartLevel()
    {
        if (deathMenuUI != null)
        {
            deathMenuUI.SetActive(false);
        }

        // Восстанавливаем время
        Time.timeScale = 1f;
        towerManager.StartFirstLevel();
    }


    // Метод для выхода в главное меню
    public void QuitToMainMenu()
    {
        Time.timeScale = 1f; // Восстанавливаем время


        if (levelManager != null)
        {
            levelManager.CleanUp(); // Очищаем все объекты, созданные LevelManager
        }

        if (mainMenuUI != null)
        {
            deathMenuUI.SetActive(false);
            mainMenuUI.SetActive(true);
            menuCamera.SetActive(true);
        }
    }
}
