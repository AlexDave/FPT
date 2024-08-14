using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameObject menuUI;        // ������ �� UI-������ ����
    public LevelManager levelManager; // ������ �� LevelManager

    public void StartGame()
    {
        menuUI.SetActive(false);  // ������������ ����
        levelManager.StartLevel(); // ��������� ������� ����� LevelManager
    }

    public void OpenOptions()
    {
        // ��� ��� �������� ���� ��������
        Debug.Log("Options Menu opened");
    }

    public void ExitGame()
    {
        // ����� �� ����
        Debug.Log("Exit Game");
        Application.Quit();
    }
}
