using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameObject menuUI;        // Ссылка на UI-объект меню
    public LevelManager levelManager; // Ссылка на LevelManager

    public void StartGame()
    {
        menuUI.SetActive(false);  // Деактивируем меню
        levelManager.StartLevel(); // Запускаем уровень через LevelManager
    }

    public void OpenOptions()
    {
        // Код для открытия меню настроек
        Debug.Log("Options Menu opened");
    }

    public void ExitGame()
    {
        // Выход из игры
        Debug.Log("Exit Game");
        Application.Quit();
    }
}
