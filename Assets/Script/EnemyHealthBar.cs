using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    public Slider healthSlider; // Ссылка на UI Slider

    // Инициализация полоски здоровья
    public void Initialize(float maxHealth)
    {
        healthSlider.maxValue = maxHealth;
        healthSlider.value = maxHealth;
    }

    // Обновление значения на полоске здоровья
    public void UpdateHealth(float currentHealth)
    {
        healthSlider.value = currentHealth;
    }
}
