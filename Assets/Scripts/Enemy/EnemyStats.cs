using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : CharacterStats
{
    public EnemyController enemy;

    public override void Die()
    {
        base.Die();

        DeathParticals();

        gameObject.SetActive(false);

        enemy.currentwalls.Clear();

        healthBar.SetHealthBarValue(1);

        ObjectPoolingManager.instance.currentEnemyCount--;

        ObjectPoolingManager.instance.diedEnemies.Add(gameObject);

        ObjectPoolingManager.instance.SetNewEnemyProperties(gameObject);
        
        ObjectPoolingManager.instance.RespawnEnemy();

        GameManager.instance.killedEnemy++;

        GameManager.instance.UpdateScoreText();
    }

    void DeathParticals()
    {
        ParticleSystem deathEffect = ObjectPoolingManager.instance.GetDeathEffect();

        if(deathEffect != null)
        {
            deathEffect.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);

            if (!deathEffect.isPlaying)
            {
                deathEffect.Play();
            }
        }
    }
}
