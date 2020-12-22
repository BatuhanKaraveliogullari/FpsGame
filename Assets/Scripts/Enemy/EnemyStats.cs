using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : CharacterStats
{
    public EnemyController enemy;//bu calssın üzerinde olduğu öbejenin başka classlarındaki değişiklikleriçin tanımlanmış olan öbje

    public override void Die()//polymorphism örneği ölüm komutu
    {
        base.Die();

        DeathParticals();

        gameObject.SetActive(false);

        enemy.currentwalls.Clear();

        healthBar.SetHealthBarValue(1);

        if(ObjectPoolingManager.instance != null)
            ObjectPoolingManager.instance.currentEnemyCount--;
            ObjectPoolingManager.instance.diedEnemies.Add(gameObject);
            ObjectPoolingManager.instance.SetNewEnemyProperties(gameObject);
            ObjectPoolingManager.instance.RespawnEnemy();
        
        if (GameManager.instance != null)
            GameManager.instance.killedEnemy++;
            GameManager.instance.UpdateScoreText();
    }

    void DeathParticals()
    {
        ParticleSystem deathEffect = ObjectPoolingManager.instance.GetDeathEffect();//daha rahat kontrol edebilmek adına get edilen effect bir partical objeye atanır.

        if(deathEffect != null)
        {
            deathEffect.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);//enemynin olduğu yerde ölmesi için konum set edilir.

            if (!deathEffect.isPlaying)
            {
                deathEffect.Play();
            }
        }
    }
}
