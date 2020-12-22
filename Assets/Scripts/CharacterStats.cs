using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    //base stats classıdır.

    public int maxHealth = 100;//set edilmiş max karakter healthi

    public int currentHealth;//oyun içinde ki anlık health

    public HealthBarHandler healthBar;//health bar health ile birlikte çalıştığı için bir örnek obje

    private void Start()
    {
        RespawnHealthControl();

        if(healthBar != null)
        {
            healthBar.SetHealthBarValue(currentHealth / currentHealth);//başlangıçteki healthbar setlemesi
        }
    }

    public void RespawnHealthControl()//spawn sonrası health setlemesi
    {
        currentHealth = Mathf.Clamp(maxHealth, 0, maxHealth);
    }

    public void TakeDamage (int damage)//karakterin damage alması
    {
        damage = Mathf.Clamp(damage, 0, int.MaxValue);//damagenin pozitif olma kontrolü

        currentHealth -= damage;

        if(currentHealth <= 0)//ölme durumu
        {
            Die();

            RespawnHealthControl();
        }
    }

    public virtual void Die()//sanal method.
    {
        
    }
}
