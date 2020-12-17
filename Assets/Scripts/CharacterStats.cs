using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    public HealthBarHandler healthBar;

    private void Start()
    {
        RespawnHealthControl();

        if(healthBar != null)
        {
            healthBar.SetHealthBarValue(currentHealth / currentHealth);
        }
    }

    public void RespawnHealthControl()
    {
        currentHealth = Mathf.Clamp(maxHealth, 0, maxHealth);
    }

    public void TakeDamage (int damage)
    {
        damage = Mathf.Clamp(damage, 0, int.MaxValue);

        currentHealth -= damage;

        if(currentHealth <= 0)
        {
            Die();

            RespawnHealthControl();
        }
    }

    public virtual void Die()
    {
        
    }
}
