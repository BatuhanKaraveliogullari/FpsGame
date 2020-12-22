using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolingManager : MonoBehaviour
{
    #region Singleton
    public static ObjectPoolingManager instance; //hiyerarşide static obje olduğu için bir static örnek tanımı

    private void Awake()
    {
        instance = this;
    }
    #endregion 

    [Header("Bullets")]
    public GameObject bulletPrefab; //poolun içine depolanacak bullet objesi
    public List<GameObject> bulletObjects; //bullet objesinin poolu

    [Header("Enemies")]
    public GameObject enemyPrefab; //poolun içine depolanacak enemy objesi
    public List<GameObject> enemyObjects; //enemy objesinin poolu
    public List<GameObject> diedEnemies; //öldürülen enemy obje poolu respawn için kullanılacaklar

    [Header("Enemy Bullets")]
    public GameObject enemyBulletPrefab; //poolun içine depolacak enemy bullet objesi
    public List<GameObject> enemyBulletObjects; //enemy bullet objesinin poolu

    [Header("Health Boosters")]
    public GameObject healthBoosterPrefab; //poolun içine depolacak health booster objesi
    public List<GameObject> healthBoosterObjects; //health booster objesinin poolu

    [Header("Particals")]
    public ParticleSystem bulletImpactPrefab; //bulletin çarpma efektinin objesi
    public ParticleSystem bloodEffectPrefab; //kan efektinin objesi
    public ParticleSystem deathEffectPrefab; //ölüm efektinin objesi
    public ParticleSystem smokeEffectPrefab; //oyun sonu duman efektinin objesi
    public ParticleSystem smokeEffect; //Instantiate edilen objeyi üzerine atamak için bir obje
    public List<ParticleSystem> bulletImpacts; //bullet effektinin poolu
    public List<ParticleSystem> bloodEffects; //kan efektinin poolu
    public List<ParticleSystem> deathEffects; //ölüm efektinin poolu

    [Header("Parents")] //hierarşiyi düzenli tutmak adına yapılmış objeler parents
    public Transform bulletImpactsParent; //bullet efektin parentı
    public Transform bloodEffectsParent; //kan efektinin parentı
    public Transform deathEffectsParent; //ölüm efektinin parentı
    public Transform currentBulletParent; //silah değişimi olduğu için bulletların hiyerarşideki yerleri değişeceği için aktif dinamik bir parent
    public Transform enemyParent; //enemy parentları
    public Transform healthBoostersParent; //health boosterların parentları
    public List<Transform> bulletParents = new List<Transform>(3); //bulletların 3 adet parentı yanı pistol,rifle,heavy olmak üzere
    public EnemyGun enemyBulletParentPrefab; //enemy bullet parentları enemylerdeki her bir enemy gun olduğu için enemy Instantiate ederken enemy gunları onunla birlikte instantiate etmek için obje
    public List<EnemyGun> enemyBulletParent = new List<EnemyGun>(); //enemy bulletları enemy gun classının childı olarak tutmak için oluşturulmuş bir list


    [Header("Pool Controls")]
    public EnemyController enemyController; //static bir class üzerine target playerın positionını almak için oluşturulmuş bir ornek
    public GunSwitch gunSwitch; //bulletların parentlarını set etmek adına silah değişimlerini kontrol edeceğim bir öbje

    public float spawnTimeOfEnemies = 1f; //enemy spawn time
    public float spawnTimeOfHealthBooster = 5f; //health booster spawn time
    public int enemyCount = 6; //oyunda olabilecek max enemy count
    public int currentEnemyCount = 0; //anlık oyun için enemy count

    public Vector3 targetPos; //player position controlu bunu static olarak tutmak istediğim için örneklendirdim.

    public bool allSpawned = false; //health booster objelerini spawnını kontrol etmek amaçlı boolen 

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
            enemyController = GameObject.FindGameObjectWithTag("Enemy").GetComponent<EnemyController>(); //health boosterın yapay zekası için lazım olan parametrelerin tutulacağı objenin bulunması

            InstantiateHealthBoosters(); //health boosterın ayrı olmasının nedeni ise health booster içinde ayrı küçük bir yapay zeka olmasıdır.

            gunSwitch = GameObject.FindGameObjectWithTag("Player").GetComponent<GunSwitch>(); //bullet parentını set etmek için gun switch null objesinden güncel değer çekmek için bir atama
        }

        SetNewParent(); 
    }

    private void Update()
    {
        if (enemyController != null) //target playerın güncel değerini enemycontroler verdiği için bir kontrol
        {
            targetPos = enemyController.targetPlayer.position; //player positionı güncel takip etmek için yapılan bir atama
        }

        if (!allSpawned) 
        {
            StartCoroutine(GetHealthBooster()); 
        }

        SetNewParent();
    }

    bool IsThereAnyEnemyOrHealthBar(float x, float y, float z) //enemy ve health booster spawnı için spawnlanacağı yerde enemy veya health booster olup olmadığı kontrolü
    {
        Vector3 objectPosition = new Vector3(x, y, z); //variableın x,y,z sinin vectore düşümü

        foreach (GameObject enemy in enemyObjects) //oyundaki anlık enemies ve positionlarının kontrolü
        {
            if (enemy.transform.position == objectPosition)
            {
                Debug.Log("There is enemy");

                return true;
            }
        }

        foreach (GameObject healthBar in healthBoosterObjects) //oyundaki health boosterların positionlarının kontrolü
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
    private void InstantiateBullets() //player bullet poolama foksiyonu
    {
        bulletObjects = new List<GameObject>(15);

        for (int i = 0; i < 15; i++)
        {
            if(bulletPrefab != null)
            {
                GameObject bullet = Instantiate(bulletPrefab);

                bullet.name = "Bullet (" + i + ")";

                bullet.transform.GetChild(0).gameObject.SetActive(false); // objeyi değil onun childını set active false yapıp kontrol ediyorum obje üzerindeki kodları güvenli kontrol etmek için

                bulletObjects.Add(bullet);//oluşturulan objeler poola dahil edildi
            }
        }

        SetBulletsNewParent(bulletObjects, currentBulletParent); 
    }

    public GameObject GetBullet() //public bir game object dönüt sağlayan method,objeti active etmek için.
    {
        for (int i = 0; i < 15; i++)
        {
            if(bulletObjects[i] != null)
            {
                if (!bulletObjects[i].transform.GetChild(0).gameObject.activeInHierarchy) //hiyerarşideki objelerin kullanılabilir olduğunu kontrol etmek için
                {
                    SetBulletInitialPosition(bulletObjects[i]);

                    bulletObjects[i].transform.GetChild(0).gameObject.SetActive(true); 

                    bulletObjects[i].GetComponent<PlayerBullet>().isBorn = true;

                    return bulletObjects[i];
                }
            }
        }

        return null;
    }

    void SetBulletInitialPosition(GameObject bullet) //bulletın başlangıç konumunu setlemek için method
    {
        if (bullet != null && currentBulletParent != null)
        {
            bullet.transform.position = currentBulletParent.transform.position; // bulletın başlangıç positionı anlık bullet parentının olduğu yer o yerde scenede first person ın üzerinde
            bullet.transform.rotation = currentBulletParent.transform.rotation;
        }
    }

    void SetBulletsNewParent(List<GameObject> bullets, Transform bulletParent) //bulletların anlık parent setlemesi
    {
        for (int i = 0; i < bullets.Count; i++)
        {
            if(bullets[i] != null)
                bullets[i].transform.SetParent(bulletParent);
        }
    }

    void SetNewParent() //anlik bulletlarin parent değişimi , kontrolleri ve gerekli atamalar
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

    public void SetEnemyBulletInitialPosition(EnemyBullet enemyBullet, EnemyGun enemyGun) //enemy bulletının işlevi bittikten sonra veya başlamadan önce başlangıç konumunun set edilmesi
    {
        if (enemyBullet != null && enemyGun != null)
        {
            enemyBullet.transform.position = enemyGun.transform.position; //her enemy kendi enemy gunına sahip ve bulletlar her bir enemynin üzerindeki enemy gunı başlangıç noktası olarak alır ve gider.
            enemyBullet.transform.rotation = enemyGun.transform.rotation;
        }
    }

    public void DeactivateBullets() //silah değişimlerinde bulletların her birinin default olarak set active false olması adına bir kontrol
    {
        foreach (GameObject bullet in bulletObjects)
        {
            if(bullet != null)
            {
                if (bullet.transform.GetChild(0).gameObject.activeInHierarchy)
                {
                    bullet.transform.GetChild(0).gameObject.SetActive(false);
                }
            }
        }
    }
    #endregion

    #region Enemy Actions
    void InstantiateEnemy() //enemylerin oluşturulup poolanması enemy gun ve enemy bulletlarla birlikte
    {
        enemyObjects = new List<GameObject>(enemyCount); //inspectordan girilen anamy sayısı

        for (int i = 0; i < enemyCount; i++)
        {
            if(enemyPrefab != null)
            {
                GameObject enemy = Instantiate(enemyPrefab);

                SetNewEnemyProperties(enemy);

                enemyObjects.Add(enemy);

                currentEnemyCount++;

                if(enemyBulletParentPrefab != null)
                {
                    EnemyGun enemyGun = Instantiate(enemyBulletParentPrefab); //her bir enemy için oluşturulan bir adet enemy gun

                    enemyGun.transform.SetParent(enemy.transform); //yukarıda oluşturulan enemynin childi yapılıyor

                    enemyGun.transform.position = new Vector3(enemy.transform.position.x, enemy.transform.position.y + 1.1f, enemy.transform.position.z);

                    enemyBulletParent.Add(enemyGun);

                    enemyGun.name = "EnemyGun(" + i + ")";

                    for (int j = 0; j < 5; j++)
                    {
                        enemyBulletObjects = new List<GameObject>(5); // her bur enemy gun yani enemynin 5 adet enemy bullet objesi var

                        if(enemyBulletPrefab != null)
                        {
                            GameObject enemyBullet = Instantiate(enemyBulletPrefab); //her bir enemy gun için oluşturulan enemy bulletlar

                            enemyBullet.name = "EnemyBullet(" + j + ")";

                            enemyBullet.transform.SetParent(enemyGun.transform); //yukarıda oluşturulan enemy gunı burada parent olarak kullanarak düzgün bir instantiate oluşturuluyor.

                            enemyBullet.transform.GetChild(0).gameObject.SetActive(false); //yukarıdakilerin aksine burada bullet olduğu için set active komutu kullanılıyor

                            enemyBulletObjects.Add(enemyBullet);
                        }
                    }
                }
            }
        }
    }

    public void SetNewEnemyProperties(GameObject enemy) //enemy her türlü özellikleri burada veriliyor
    {
        HealthBarHandler healthBar = enemy.transform.GetComponentInChildren<HealthBarHandler>();

        if(healthBar != null) 
            healthBar.SetHealthBarValue(1); // burada enemy üzerindeki bar fulleniyor öldüğünde 0 landığı için

        for (int i = 0; i < 100; i++)
        {
            int ranX = Random.Range(1, Map.instance.width);//random x değeri verme
            int ranZ = Random.Range(1, Map.instance.depth);//random z değeri verme

            if (enemy != null && Map.instance.IsWithinBoundsforObjects(ranX, 0, ranZ) && !Map.instance.IsInDangerRangeOfWall(ranX, 2, ranZ) && !IsThereAnyEnemyOrHealthBar(ranX, 1.5f, ranZ)) //burada çeşitli kontroller var bu kontrolleri fonksiyonların olduğu clasın içinde açıklanacak
            {
                enemy.transform.position = new Vector3(ranX, 1.5f, ranZ); //enemynin random değerlerini alıp ataması burda y nin 1,5 olması sabit değer olduğundan kaynaklı tilelların üzerinde durması adına

                enemy.name = "Enemy (" + ranX + " , " + 1.5f + " , " + ranZ + ")";

                if (enemyParent != null)
                    enemy.transform.SetParent(enemyParent);

                return; // bu fonksiyonun for içinde olması kabul görmeyen değerler varsa tekrardan değer almak adına bu return ise  kabul gören bir property varsa loopdan çıkması adına
            }
        }
    }

    public void RespawnEnemy()
    {
        if (currentEnemyCount < enemyCount) //anlık enemy sayısı ve oyunda olması istenen enemy sayısı kıyaslanıyor ve fark varsa coroutine fanksiyonunu çalıştırıyor. 
        {
            StartCoroutine(RespawnEnemyRoutine());
        }
    }

    IEnumerator RespawnEnemyRoutine()//enemylar poolandığı için ölümleri set active ile kontrol ediliyor.
    {
        for (int i = 0; i < diedEnemies.Count; i++)
        {
            if(diedEnemies[i] != null)
            {
                if (!diedEnemies[i].activeInHierarchy)//ölen enemy öldüğü yerde bu liste dahil oluyor. hiyerarşide active olup olmnadığı kontrol ediliyor.
                {
                    yield return new WaitForSeconds(spawnTimeOfEnemies);

                    if (i < diedEnemies.Count)//bu diedenemy list active bir list olduğu için anlık değişimleri ölçmek adına böyle bir kontrol ekledim. bu sayede forun içindeyken list.count değişirse fonksiyon hata vermesin diye yapılan bir kontrol
                    {
                        ActivateEnemy(diedEnemies[i]);

                        CheckEnemyCount(enemyObjects);

                        diedEnemies[i].GetComponent<EnemyController>().SetWalls();

                        diedEnemies.Remove(diedEnemies[i]);//respawned olan enemy bu listeden çıkması gerektiği için remove ediyoruz.
                    }
                }
            }
        }
    }

    void ActivateEnemy(GameObject enemy) //ölen enemyi canlandırmak için kullanılan fonksiyon
    {
        if(enemy != null)
        {
            enemy.SetActive(true);

            enemy.transform.GetChild(2).gameObject.GetComponent<EnemyGun>().enemyIsJustSpawn = true;
        }
    }

    void CheckEnemyCount(List<GameObject> enemyObject)//bu fonksiyonda oyun içerisindeki enemy listini update etmek adına kullanılıyor, oyun içerisindeki active enemylerin sayısını alıp anlık enemy sayısını sağlam bir şekilde update etmek için kullanılıyor.
    {
        List<GameObject> activeEnemy = new List<GameObject>();//oyun içerisindeki activeenemy listi local bir değer

        foreach (GameObject enemy in enemyObject)
        {
            if(enemy != null)
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
    }
    #endregion

    #region Health Booster Actions
    private void InstantiateHealthBoosters()//health boosterların oluşturulup poollanma fonksiyonu
    {
        SetNewHealthBoosterCount(enemyCount, enemyController.lookRadius);//oyundaki health booster sayısını ayarlayan fonksiyon

        for (int i = 0; i < healthBoosterObjects.Capacity; i++)
        {
            if(healthBoosterPrefab != null)
            {
                GameObject healthBooster = Instantiate(healthBoosterPrefab);

                healthBooster.transform.SetParent(healthBoostersParent);

                healthBoosterObjects.Add(healthBooster);

                healthBooster.SetActive(false);
            }
        }
    }

    IEnumerator GetHealthBooster() //allSpawn boolenına göre sürekli ve birkere çalışan bir IEnumerator methodu spawn time a göre gerek sıklıkta çalışan bir method
    {
        allSpawned = true; //burda bu boolenı trueluyoruz çünkü fonksiyon update te çalışıyor ve bir kere çağırılması gerekiyor.

        for (int i = 0; i < healthBoosterObjects.Capacity; i++)
        {
            if(healthBoosterObjects[i] != null)
            {
                if (!healthBoosterObjects[i].activeInHierarchy)
                {
                    SetNewHealthBoosterProperties(healthBoosterObjects[i]);

                    yield return new WaitForSeconds(spawnTimeOfHealthBooster);

                    healthBoosterObjects[i].SetActive(true);
                }
            }
        }

        allSpawned = false;//burada false ediyoruz çünkü artık çağırılabilir ve içine girilebilir olması gerekir
    }

    void SetNewHealthBoosterCount(int enemyCount, float lookRadius) //enemy count ve lookradiusa göre otomatik bir health booster logici
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
            int ranX = Random.Range(1, Map.instance.width);//random x değeri verme
            int ranZ = Random.Range(1, Map.instance.depth);//random z değeri verme

            if (healthBooster != null && Map.instance.IsWithinBoundsforObjects(ranX, 0, ranZ) && !Map.instance.IsInDangerRangeOfWall(ranX, 2, ranZ) && !IsThereAnyEnemyOrHealthBar(ranX, 1.5f, ranZ))//buradaki çeşitli kontroller var bulundukları classlarda açıklıyorum.
            {
                healthBooster.transform.position = new Vector3(ranX, 1.5f, ranZ);//burda y nin 1,5f olmasının nedeni aynı enemyde olduğu gibi

                healthBooster.name = "HealthBooster (" + ranX + " , " + 1.5f + " , " + ranZ + ")";

                return;//bu return durumuda random değerlerin uygulanabilirliğine göre çalışacak
            }
        }
    }
    #endregion

    #region Effect Actions
    void InstantiateBulletImpact()//bullet efektinin oluşturulması ve poollanması
    {
        bulletImpacts = new List<ParticleSystem>(15);

        for (int i = 0; i < 15; i++)
        {
            if(bulletImpactPrefab != null)
            {
                ParticleSystem bulletImpact = Instantiate(bulletImpactPrefab);

                bulletImpacts.Add(bulletImpact);

                bulletImpact.name = "Bullet Impact(" + i + ")";

                bulletImpact.transform.SetParent(bulletImpactsParent);
            }
        }
    }  

    public void InstantiateSmokeEffect()//bullet efektinin oluşturulması ve poollanması
    {
        if(smokeEffect == null)
        {
            Vector3 finishPoint = new Vector3(Map.instance.width - 1, Map.instance.height, Map.instance.depth - 1);//burada finish pointi İnstantiate ediyoruz bu değer random olabilir ben bu scene uygun bir plan yapmıştım.

            smokeEffect = Instantiate(smokeEffectPrefab, finishPoint, Quaternion.identity);//burada yukarıda açıkladığım gibi smokeEffect public olarak başka classlarda çağırılabilmesi için Instantiate edilen smoke null ve public bir objeye atanıyor
        }
    }

    public ParticleSystem GetBulletImpact()//bullet mantığıyla ynı mantıkta çalışan bir fonksiyon ve bulletın aksine bu method partical geridönüş sağlıyor.
    {
        for (int i = 0; i < bulletImpacts.Count; i++)
        {
            if(bulletImpacts[i] != null)
            {
                if (!bulletImpacts[i].isPlaying)//method partical geri dönüş yaptığı için çalışıp çalışmadığını kontrol ediyor.
                {
                    return bulletImpacts[i];
                }
            }
        }

        return null;
    }

    void InstantiateBloodEffect()//kan efektinin oluşturulması ve poollanması
    {
        bloodEffects = new List<ParticleSystem>(15);

        for (int i = 0; i < 15; i++)
        {
            if(bloodEffectPrefab != null)
            {
                ParticleSystem bloodEffect = Instantiate(bloodEffectPrefab);

                bloodEffects.Add(bloodEffect);

                bloodEffect.name = "Blood Effect(" + i + ")";

                bloodEffect.transform.SetParent(bloodEffectsParent);
            }
        }
    }

    public ParticleSystem GetBloodEffect()//bullet impact ile aynı mantık gerek olduğu zaman çağırılıyor ve partical obje dönüş yapıyor.
    {
        for (int i = 0; i < bloodEffects.Count; i++)
        {
            if(bloodEffects[i] != null)
            {
                if (!bloodEffects[i].isPlaying)//method partical geri dönüş yaptığı için çalışıp çalışmadığını kontrol ediyor.
                {
                    return bloodEffects[i];
                }
            }
        }

        return null;
    }

    void InstantiateDeathEffect()//ölüm efektinin oluşturulması ve poollanması
    {
        deathEffects = new List<ParticleSystem>(enemyCount);

        for (int i = 0; i < enemyCount; i++)
        {
            if(deathEffectPrefab != null)
            {
                ParticleSystem deathEffect = Instantiate(deathEffectPrefab);

                deathEffects.Add(deathEffect);

                deathEffect.name = "Death Effect(" + i + ")";

                deathEffect.transform.SetParent(deathEffectsParent);
            }
        }
    }

    public ParticleSystem GetDeathEffect()//bullet impact ile aynı mantık gerek olduğu zaman çağırılıyor ve partical obje dönüş yapıyor.
    {
        for (int i = 0; i < deathEffects.Count; i++)
        {
            if(deathEffects[i] != null)
            {
                if (!deathEffects[i].isPlaying)//method partical geri dönüş yaptığı için çalışıp çalışmadığını kontrol ediyor.
                {
                    return deathEffects[i];
                }
            }
        }

        return null;
    }
    #endregion
}
