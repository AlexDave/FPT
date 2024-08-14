using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathMenu : MonoBehaviour
{
    public GameObject deathMenuUI; // ������ �� ������ Canvas
    public GameObject mainMenuUI; // ������ �� ������ Canvas
    public LevelManager levelManager;

    void Start()
    {
        if (deathMenuUI != null)
        {
            deathMenuUI.SetActive(false); // �������� ���� ������ ��� ������
        }
        else
        {
            Debug.LogError("DeathMenuUI �� �������� � ����������.");
        }
    }

    // ����� ��� ����������� ���� ������
    public void ShowDeathMenu()
    {
        if (deathMenuUI != null)
        {
            deathMenuUI.SetActive(true); // ���������� Canvas
            Time.timeScale = 0f; // ������������� ����
        }
        else
        {
            Debug.LogError("DeathMenuUI �� �������� � ����������.");
        }
    }

    // ����� ��� ����������� �������� ������
    public void RestartLevel()
    {
        if (deathMenuUI != null)
        {
            deathMenuUI.SetActive(false);
        }

        // ��������������� �����
        Time.timeScale = 1f;

        if (levelManager != null)
        {
            // ���������� ������� ������ ������
            levelManager.currentLevelIndex = 0;

            // ��������� ������� ������ � ������ �����
            levelManager.StartLevel();
        }
        else
        {
            Debug.LogError("LevelManager �� �������� � ����������.");
        }
    }


    // ����� ��� ������ � ������� ����
    public void QuitToMainMenu()
    {
        Time.timeScale = 1f; // ��������������� �����


        if (levelManager != null)
        {
            levelManager.CleanUp(); // ������� ��� �������, ��������� LevelManager
        }

        if (mainMenuUI != null)
        {
            deathMenuUI.SetActive(false);
            mainMenuUI.SetActive(true);
        }
    }
}
