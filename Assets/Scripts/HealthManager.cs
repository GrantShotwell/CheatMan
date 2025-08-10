using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public Image healthBar;
    //[SerializeField] float maxHealth = 100;
    public float healthAmount = 50f;
    public float currentMaxHP;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentMaxHP = healthAmount;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(float damage)
    {
        healthAmount -= damage;
        healthBar.fillAmount= healthAmount / 100f;
    }

    public void heal(float healingAmount)
    {
        healthAmount += healingAmount;
        healthAmount = Mathf.Clamp(healthAmount, 0, 100);

        healthBar.fillAmount = healthAmount / 100f;
    }
}
