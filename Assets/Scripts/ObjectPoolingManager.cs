using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolingManager : MonoBehaviour
{
    public static ObjectPoolingManager instance;

    [Header("Bullets")]
    public GameObject bulletPrefab;
    public List<GameObject> bulletObjects;

    [Header("Enemies")]
    public GameObject enemyPrefab;
    public List<GameObject> enemyObjects;
    public List<GameObject> diedEnemies;

    [Header("Enemy Bullets")]
    public GameObject enemyBulletPrefab;
    public List<GameObject> enemyBulletObjects;
    public EnemyGun enemyBulletParentPrefab;
    public EnemyBullet enemyBullet;

    [Header("Health Boosters")]
    public GameObject healthBoosterPrefab;
    public List<GameObject> healthBoosterObjects;

    [Header("Particals")]
    public ParticleSystem bulletImpactPrefab;
    public ParticleSystem bloodEffectPrefab;
    public ParticleSystem deathEffectPrefab;
    public ParticleSystem smokeEffectPrefab;
    public ParticleSystem smokeEffect;
    public List<ParticleSystem> bulletImpacts;
    public List<ParticleSystem> bloodEffects;
    public List<ParticleSystem> deathEffects;

    [Header("Parents")]
    public Transform bulletImpactsParent;
    public Transform bloodEffectsParent;
    public Transform deathEffectsParent;
    public Transform currentBulletParent;
    public Transform enemyParent;
    public Transform healthBoostersParent;
    public List<Transform> bulletParents = new List<Transform>(3);
    public List<EnemyGun> enemyBulletParent = new List<EnemyGun>(6);

    [Header("Pool Controls")]
    public EnemyController enemyController;
    public GunSwitch gunSwitch;

    public float spawnTimeOfEnemies = 1f;
    public float spawnTimeOfHealthBooster = 5f;
    public int enemyCount = 6;
    public int currentEnemyCount = 0;

    public Vector3 targetPos;

    public bool allSpawned = false;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        diedEnemies = new List<GameObject>();

        InstantiateBullets();

        InstantiateEnemy();

        InstantiateBulletImpact();

        InstantiateBloodEffect();

        InstantiateDeathEffect();

        if (GameObject.FindGameObjectWithTag("Enemy") != null)
        {
            enemyController = GameObject.FindGameObjectWithTag("Enemy").GetComponent<EnemyController>();

            InstantiateHealthBoosters();

            gunSwitch = GameObject.FindGameObjectWithTag("Player").GetComponent<GunSwitch>();
        }

        SetNewParent();
    }

    private void Update()
    {
        if (enemyController != null)
        {
            targetPos = enemyController.targetPlayer.position;
        }

        if (!allSpawned)
        {
            StartCoroutine(GetHealthBooster());
        }

        SetNewParent();
    }

    bool IsThereAnyEnemyOrHealthBar(float x, float y, float z)
    {
        Vector3 objectPosition = new Vector3(x, y, z);

        foreach (GameObject enemy in enemyObjects)
        {
            if (enemy.transform.position == objectPosition)
            {
                Debug.Log("There is enemy");

                return true;
            }
        }

        foreach (GameObject healthBar in healthBoosterObjects)
        {
            if (healthBar.transform.position == objectPosition)
            {
                Debug.Log("There is healthbar");

                return true;
            }
        }

        return false;
    }

    #region Bullet Actions
    private void InstantiateBullets()
    {
        bulletObjects = new List<GameObject>(15);

        for (int i = 0; i < 15; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab);

            bullet.name = "Bullet ("+ i +")";

            bullet.transform.GetChild(0).gameObject.SetActive(false);

            bulletObjects.Add(bullet);
        }

        SetBulletsNewParent(bulletObjects, currentBulletParent);
    }

    public GameObject GetBullet()
    {
        for (int i = 0; i < 15; i++)
        {
            if (!bulletObjects[i].transform.GetChild(0).gameObject.activeInHierarchy)
            {
                SetBulletInitialPosition(bulletObjects[i]);

                bulletObjects[i].transform.GetChild(0).gameObject.SetActive(true);

                bulletObjects[i].GetComponent<PlayerBullet>().isBorn = true;

                return bulletObjects[i];
            }
        }

        return null;
    }

    void SetBulletInitialPosition(GameObject bullet)
    {
        if (bullet != null)
        {
            bullet.transform.position = currentBulletParent.transform.position;
            bullet.transform.rotation = currentBulletParent.transform.rotation;
        }
    }

    void SetBulletsNewParent(List<GameObject> bullets, Transform bulletParent)
    {
        for (int i = 0; i < bullets.Count; i++)
        {
            bullets[i].transform.SetParent(bulletParent);
        }
    }

    void SetNewParent()
    {
        if (gunSwitch != null)
        {
            if (gunSwitch.selctedGun == 0)
            {
                currentBulletParent = bulletParents[0];

                SetBulletsNewParent(bulletObjects, currentBulletParent);
            }
            else if (gunSwitch.selctedGun == 1)
            {
                currentBulletParent = bulletParents[1];

                SetBulletsNewParent(bulletObjects, currentBulletParent);
            }
            else
            {
                currentBulletParent = bulletParents[2];

                SetBulletsNewParent(bulletObjects, currentBulletParent);
            }
        }
    }

    public void SetEnemyBulletInitialPosition(EnemyBullet enemyBullet, EnemyGun enemyGun)
    {
        if (enemyBullet != null)
        {
            enemyBullet.transform.position = enemyGun.transform.position;
            enemyBullet.transform.rotation = enemyGun.transform.rotation;
        }
    }

    public void DeactivateBullets()
    {
        foreach (GameObject bullet in bulletObjects)
        {
            if (bullet.transform.GetChild(0).gameObject.activeInHierarchy)
            {
                bullet.transform.GetChild(0).gameObject.SetActive(false);
            }
        }
    }
    #endregion

    #region Enemy Actions
    private void InstantiateEnemy()
    {
        enemyObjects = new List<GameObject>(enemyCount);

        for (int i = 0; i < enemyCount; i++)
        {
            GameObject enemy = Instantiate(enemyPrefab);

            SetNewEnemyProperties(enemy);

            enemyObjects.Add(enemy);

            currentEnemyCount++;
            
            EnemyGun enemyGun = Instantiate(enemyBulletParentPrefab);
            
            enemyGun.transform.SetParent(enemy.transform);

            enemyGun.transform.position = new Vector3(enemy.transform.position.x, enemy.transform.position.y + 1.1f, enemy.transform.position.z);

            enemyBulletParent.Add(enemyGun);

            enemyGun.name = "EnemyGun(" + i + ")";

            for (int j = 0; j < 2; j++)
            {
                enemyBulletObjects = new List<GameObject>(2);

                GameObject enemyBullet = Instantiate(enemyBulletPrefab);

                enemyBullet.name = "EnemyBullet(" + j + ")";

                enemyBullet.transform.SetParent(enemyGun.transform);

                enemyBullet.transform.GetChild(0).gameObject.SetActive(false);

                enemyBulletObjects.Add(enemyBullet);
            }
            
        }
    }

    public void SetNewEnemyProperties(GameObject enemy)
    {
        HealthBarHandler healthBar = enemy.transform.GetComponentInChildren<HealthBarHandler>();

        healthBar.SetHealthBarValue(1);

        for (int i = 0; i < 100; i++)
        {
            int ranX = Random.Range(1, Map.instance.width);
            int ranZ = Random.Range(1, Map.instance.depth);

            if (enemy != null && Map.instance.IsWithinBoundsforObjects(ranX, 0, ranZ) && !Map.instance.IsInDangerRangeOfWall(ranX, 2, ranZ) && !IsThereAnyEnemyOrHealthBar(ranX, 1.5f, ranZ))
            {
                enemy.transform.position = new Vector3(ranX, 1.5f, ranZ);

                enemy.name = "Enemy (" + ranX + " , " + 1.5f + " , " + ranZ + ")";

                if (enemyParent != null)
                    enemy.transform.SetParent(enemyParent);

                return;
            }
        }
    }

    public void RespawnEnemy()
    {
        if (currentEnemyCount < enemyCount)
        {
            StartCoroutine(RespawnEnemyRoutine());
        }
    }

    IEnumerator RespawnEnemyRoutine()
    {
        for (int i = 0; i < diedEnemies.Count; i++)
        {
            if (!diedEnemies[i].activeInHierarchy)
            {
                yield return new WaitForSeconds(spawnTimeOfEnemies);

                if (i < diedEnemies.Count)
                {
                    ActivateEnemy(diedEnemies[i]);

                    CheckEnemyCount(enemyObjects);

                    diedEnemies[i].GetComponent<EnemyController>().SetWalls();

                    diedEnemies.Remove(diedEnemies[i]);
                }
            }
        }
    }

    void ActivateEnemy(GameObject enemy)
    {
        enemy.SetActive(true);

        enemy.transform.GetChild(2).gameObject.GetComponent<EnemyGun>().enemyIsJustSpawn = true;
    }

    void CheckEnemyCount(List<GameObject> enemyObject)
    {
        List<GameObject> activeEnemy = new List<GameObject>();

        foreach (GameObject enemy in enemyObject)
        {
            if (enemy.activeInHierarchy)
            {
                activeEnemy.Add(enemy);
            }
            else
            {
                activeEnemy.Remove(enemy);
            }

            currentEnemyCount = activeEnemy.Count;
        }
    }
    #endregion

    #region Health Booster Actions
    private void InstantiateHealthBoosters()
    {
        SetNewHealthBoosterCount(enemyCount, enemyController.lookRadius);

        for (int i = 0; i < healthBoosterObjects.Capacity; i++)
        {
            GameObject healthBooster = Instantiate(healthBoosterPrefab);

            healthBooster.transform.SetParent(healthBoostersParent);

            healthBoosterObjects.Add(healthBooster);

            healthBooster.SetActive(false);
        }
    }

    IEnumerator GetHealthBooster()
    {
        allSpawned = true;

        for (int i = 0; i < healthBoosterObjects.Capacity; i++)
        {
            if (!healthBoosterObjects[i].activeInHierarchy)
            {
                SetNewHealthBoosterProperties(healthBoosterObjects[i]);

                yield return new WaitForSeconds(spawnTimeOfHealthBooster);

                healthBoosterObjects[i].SetActive(true);
            }
        }

        allSpawned = false;
    }

    void SetNewHealthBoosterCount(int enemyCount, float lookRadius)
    {
        if (enemyCount <= 5)
        {
            if (lookRadius <= 5)
            {
                healthBoosterObjects = new List<GameObject>(1);
            }
            else
            {
                healthBoosterObjects = new List<GameObject>(2);
            }
        }
        else
        {
            if (lookRadius <= 5)
            {
                healthBoosterObjects = new List<GameObject>(2);
            }
            else
            {
                healthBoosterObjects = new List<GameObject>(3);
            }
        }
    }

    void SetNewHealthBoosterProperties(GameObject healthBooster)
    {
        for (int i = 0; i < 100; i++)
        {
            int ranX = Random.Range(1, Map.instance.width);
            int ranZ = Random.Range(1, Map.instance.depth);

            if (healthBooster != null && Map.instance.IsWithinBoundsforObjects(ranX, 0, ranZ) && !Map.instance.IsInDangerRangeOfWall(ranX, 2, ranZ) && !IsThereAnyEnemyOrHealthBar(ranX, 1.5f, ranZ))
            {
                healthBooster.transform.position = new Vector3(ranX, 1.5f, ranZ);

                healthBooster.name = "HealthBooster (" + ranX + " , " + 1.5f + " , " + ranZ + ")";

                return;
            }
        }
    }
    #endregion

    #region Effect Actions
    void InstantiateBulletImpact()
    {
        bulletImpacts = new List<ParticleSystem>(15);

        for (int i = 0; i < 15; i++)
        {
            ParticleSystem bulletImpact = Instantiate(bulletImpactPrefab);

            bulletImpacts.Add(bulletImpact);

            bulletImpact.name = "Bullet Impact(" + i + ")";

            bulletImpact.transform.SetParent(bulletImpactsParent);
        }
    }  

    public void InstantiateSmokeEffect()
    {
        if(smokeEffect == null)
        {
            Vector3 finishPoint = new Vector3(Map.instance.width - 1, Map.instance.height, Map.instance.depth - 1);

            smokeEffect = Instantiate(smokeEffectPrefab, finishPoint, Quaternion.identity);
        }
    }

    public ParticleSystem GetBulletImpact()
    {
        for (int i = 0; i < bulletImpacts.Count; i++)
        {
            if(!bulletImpacts[i].isPlaying)
            {
                return bulletImpacts[i];
            }
        }

        return null;
    }

    void InstantiateBloodEffect()
    {
        bloodEffects = new List<ParticleSystem>(15);

        for (int i = 0; i < 15; i++)
        {
            ParticleSystem bloodEffect = Instantiate(bloodEffectPrefab);

            bloodEffects.Add(bloodEffect);

            bloodEffect.name = "Blood Effect(" + i + ")";

            bloodEffect.transform.SetParent(bloodEffectsParent);
        }
    }

    public ParticleSystem GetBloodEffect()
    {
        for (int i = 0; i < bloodEffects.Count; i++)
        {
            if (!bloodEffects[i].isPlaying)
            {
                return bloodEffects[i];
            }
        }

        return null;
    }

    void InstantiateDeathEffect()
    {
        deathEffects = new List<ParticleSystem>(enemyCount);

        for (int i = 0; i < enemyCount; i++)
        {
            ParticleSystem deathEffect = Instantiate(deathEffectPrefab);

            deathEffects.Add(deathEffect);

            deathEffect.name = "Death Effect(" + i + ")";

            deathEffect.transform.SetParent(deathEffectsParent);
        }
    }

    public ParticleSystem GetDeathEffect()
    {
        for (int i = 0; i < deathEffects.Count; i++)
        {
            if (!deathEffects[i].isPlaying)
            {
                return deathEffects[i];
            }
        }

        return null;
    }
    #endregion
}
