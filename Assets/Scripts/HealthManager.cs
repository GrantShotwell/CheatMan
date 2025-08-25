using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public Image healthBar;
    public float healthAmount = 50f;
    public float maximumHealth = 50f;
    public float currentMaxHP;

	public void TakeDamage(float damage)
    {
        healthAmount = Mathf.Clamp(healthAmount - damage, 0, maximumHealth);
        healthBar.fillAmount= healthAmount / maximumHealth;
    }

    public void GiveHealing(float healing)
    {
        healthAmount = Mathf.Clamp(healthAmount + healing, 0, maximumHealth);
        healthBar.fillAmount = healthAmount / maximumHealth;
    }
}
