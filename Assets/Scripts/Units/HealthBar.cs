using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    float maxHealth;
    float currentHealth;
    public Slider healthSlider;
    public Image healthBar;
    public Gradient healthGradient;

    public void SetMaxHealth(int max_health)
    {
        maxHealth = max_health;
    }

    public void SetHealth(int health)
    {
        currentHealth = health;

        float healthValue = currentHealth / maxHealth;
        healthSlider.value = healthValue;
        healthBar.color = healthGradient.Evaluate(healthValue / healthSlider.maxValue);
    }
}
