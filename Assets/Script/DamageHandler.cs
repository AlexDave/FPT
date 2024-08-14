using System;
using UnityEngine;
using UnityEngine.UI;

public class DamageHandler : MonoBehaviour
{
    public float[] damages = { 10f, 20f, 30f, 40f, 50f }; // ������ �������� �����
    public float currentDamage = 0f; // ������� ����

    // ����� ���������� ��� ������� �� ������, ��������� ������ �� ������� damages
    public void SetDamage(int index)
    {
        currentDamage = damages[index];
        Console.WriteLine(currentDamage);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // �������� ������� ����� ������ ����
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // ������� ��� �� ������ � ����������� �������
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit)) // ��������� ��������� ���� � ������
            {
                Enemy enemy = hit.collider.GetComponent<Enemy>(); // �������� ��������� Enemy � �������, � ������� ����� ���
                if (enemy != null) // ���� ������ - ����, ������� ��� ����
                {
                    enemy.TakeDamage(currentDamage);
                }
            }
        }
    }
}