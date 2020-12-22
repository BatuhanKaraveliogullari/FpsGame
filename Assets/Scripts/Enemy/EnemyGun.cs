using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGun : Gun
{
    private float otherTimeToFireToPleyer = 0f;//enemy gun firerate

    public EnemyBullet enemyBullet;//ateşlenecek bulletın obje örneklemesi

    public EnemyController enemyController;//eteşleme kontrolü için oradan çekeceğim verilerin obje örneklemesi

    public bool enemyIsJustSpawn = false;//enemy spawnlanır spawnlanmaz alınacak veyadeğişecek özelliklerin konrtrolü

    private void Start()
    {
        enemyController = gameObject.transform.GetComponentInParent<EnemyController>();//bu component this componentin üzerinde bulunduğu bir component olduğu için start methodunda çağırılır.
    }

    void Update()
    {
        if(enemyIsJustSpawn)
        {
            usedBullets = 0;//enemy her yeni spawnlanduığında kullanılan bullet sıfırlanır.

            enemyIsJustSpawn = false;
        }

        if (enemyController.enemyDetected && Time.time >= otherTimeToFireToPleyer && GameManager.instance.isStarted)//
        {
            if (mag > usedBullets)//şarjörün bitip bitmediği kontrol edilir
            {
                otherTimeToFireToPleyer = Time.time + 1f / fireRate;//burada fire rate süresi ayarlanır.

                usedBullets++;

                Shoot();
            }
            else
            {
                StartCoroutine(ReloadTimerRoutine());
            }
        }
    }

    public override void Shoot()//polymorphism örneği
    {
        base.Shoot();
        
        GetEnemyBullet();

        gunSound.Play();
    }

    public EnemyBullet GetEnemyBullet()//bullet ateşlenir.
    {
        List<EnemyBullet> enemyBullets = new List<EnemyBullet>();

        for (int i = 0; i < 5; i++)
        {
            EnemyBullet enemybullet = gameObject.transform.GetChild(i).GetComponent<EnemyBullet>();//daha rahat kontrol etmek adına inspectorda enemy gun objesi altında oluşturulmuş enemy bulletlar burada local bir liste atılır.

            if(enemybullet != null)
                enemyBullets.Add(enemybullet);
        }

        for (int i = 0; i < enemyBullets.Count; i++)//burada ise çağırılır.
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

    IEnumerator ReloadTimerRoutine()//şarjör bitimiyle relaod komutudur.
    {
        yield return new WaitForSeconds(reloadTime);

        usedBullets = 0;
    }
}
