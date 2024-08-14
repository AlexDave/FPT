using System;
using UnityEngine;
using UnityEngine.UI;

public class DamageHandler : MonoBehaviour
{
    public float[] damages = { 10f, 20f, 30f, 40f, 50f }; // Массив значений урона
    public float currentDamage = 0f; // Текущий урон

    // Метод вызывается при нажатии на кнопку, принимает индекс из массива damages
    public void SetDamage(int index)
    {
        currentDamage = damages[index];
        Console.WriteLine(currentDamage);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Проверка нажатия левой кнопки мыши
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // Создаем луч от камеры в направлении курсора
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit)) // Проверяем попадание луча в объект
            {
                Enemy enemy = hit.collider.GetComponent<Enemy>(); // Получаем компонент Enemy у объекта, в который попал луч
                if (enemy != null) // Если объект - враг, наносим ему урон
                {
                    enemy.TakeDamage(currentDamage);
                }
            }
        }
    }
}