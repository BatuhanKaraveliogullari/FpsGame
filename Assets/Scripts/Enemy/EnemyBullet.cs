using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : Bullet
{
    public Vector3 targetPosition;

    public bool enterUpdateOnce = false;

    void Update()
    {
        if(transform.GetChild(0).gameObject.activeInHierarchy && !enterUpdateOnce)
        {
            enterUpdateOnce = true;

            SetTargetPosition();

            Invoke("DeactiveEnemyBullet", lifeTime);
        }

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, 25 * Time.deltaTime);
    }

    void DeactiveEnemyBullet()
    {
        transform.GetChild(0).gameObject.SetActive(false);

        enterUpdateOnce = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            transform.GetChild(0).gameObject.SetActive(false);

            PlayerStats player = collision.gameObject.GetComponent<PlayerStats>();

            HealthBarHandler healthBar = collision.gameObject.GetComponentInChildren<HealthBarHandler>();

            Gun gun = transform.parent.gameObject.GetComponent<EnemyGun>();

            if(healthBar != null && player != null && gun.gameObject != null)
            {
                player.TakeDamage(gun.damage);

                healthBar.SetHealthBarValue(healthBar.GetHealthBarValue() - (float)((float)gun.damage / (float)player.maxHealth));
            }

            enterUpdateOnce = false;
        }
        else if(collision.gameObject.CompareTag("Ground"))
        {
            transform.GetChild(0).gameObject.SetActive(false);

            enterUpdateOnce = false;
        }
        else if(collision.gameObject.CompareTag("Wall"))
        {
            transform.GetChild(0).gameObject.SetActive(false);

            enterUpdateOnce = false;
        }
    }

    public void SetTargetPosition()
    {
        targetPosition = ObjectPoolingManager.instance.targetPos;
    }
}
