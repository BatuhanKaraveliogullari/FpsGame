using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGun : Gun
{
    private float otherTimeToFireToPleyer = 0f;

    public EnemyBullet enemyBullet;

    public EnemyController enemyController;

    public bool enemyIsJustSpawn = false;

    private void Start()
    {
        enemyController = gameObject.transform.GetComponentInParent<EnemyController>();
    }

    void Update()
    {
        if(enemyIsJustSpawn)
        {
            usedBullets = 0;

            enemyIsJustSpawn = false;
        }

        if (enemyController.enemyDetected && Time.time >= otherTimeToFireToPleyer && GameManager.instance.isStarted)
        {
            if (mag > usedBullets)
            {
                otherTimeToFireToPleyer = Time.time + 1f / fireRate;

                usedBullets++;

                Shoot();
            }
            else
            {
                StartCoroutine(ReloadTimerRoutine());
            }
        }
    }

    public override void Shoot()
    {
        base.Shoot();
        
        GetEnemyBullet();

        gunSound.Play();
    }

    public EnemyBullet GetEnemyBullet()
    {
        List<EnemyBullet> enemyBullets = new List<EnemyBullet>();

        for (int i = 0; i < 5; i++)
        {
            EnemyBullet enemybullet = gameObject.transform.GetChild(i).GetComponent<EnemyBullet>();

            if(enemybullet != null)
                enemyBullets.Add(enemybullet);
        }

        for (int i = 0; i < enemyBullets.Count; i++)
        {
            if(enemyBullets[i] != null)
            {
                if (!enemyBullets[i].transform.GetChild(0).gameObject.activeInHierarchy)
                {
                    ObjectPoolingManager.instance.SetEnemyBulletInitialPosition(enemyBullets[i], enemyBullets[i].transform.parent.gameObject.GetComponent<EnemyGun>());

                    enemyBullets[i].transform.GetChild(0).gameObject.SetActive(true);

                    return enemyBullets[i];
                }
            }
        }

        return null;
    }

    IEnumerator ReloadTimerRoutine()
    {
        yield return new WaitForSeconds(reloadTime);

        usedBullets = 0;
    }
}
