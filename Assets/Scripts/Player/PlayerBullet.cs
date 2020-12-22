using System.Collections;
using UnityEngine;

public class PlayerBullet : Bullet
{
    public PlayerGun playerGun;//gun switchlerini kontrol etmek adına örneklenmiş obje

    public GunSwitch gunSwitch;//gunswitch classından çekilecek veriler için örneklendirme

    IEnumerator life;//coroutine fonksiyonlarını start ve stoplamak için örneklendirilmiş öbje

    public bool isBorn = false;//lifetimeı kontrol etmek adına boolen

    private void Start()
    {
        playerGun = GameObject.FindWithTag("PlayerGun").GetComponent<PlayerGun>();//bulletların bulunduğu atalarını bulan satır. oyun içerinde sadece bir obje active olacağı için player gunlardan,active olanı bulacak.

        gunSwitch = GameObject.FindGameObjectWithTag("Player").GetComponent<GunSwitch>();

        life = DestroyGameObject();
    }

    private void Update()
    {
        if(transform.GetChild(0).gameObject.activeInHierarchy && playerGun != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, playerGun.hit.point, playerGun.bulletSpeed * Time.deltaTime);//child objesinin activate durumuna göre bullet movement
        }

        if(isBorn)
        {
            life = DestroyGameObject();

            StartCoroutine(life);//atamayı yapıp coroutine başlatıyoruz.
        }

        SwitchGun();
    }

    public void SwitchGun()//gunswitchten aldığımız değerde bir değişiklik varsa bu objeleri değişen gun childı yapmak için çalışan bir method
    {
        if (gunSwitch.selctedGun == 0)
        {
            playerGun = null;

            playerGun = GameObject.Find("Pistol").GetComponent<PlayerGun>();
        }

        else if (gunSwitch.selctedGun == 1)
        {
            playerGun = null;

            playerGun = GameObject.Find("Rifle").GetComponent<PlayerGun>();
        }

        else if (gunSwitch.selctedGun == 2)
        {
            playerGun = null;

            playerGun = GameObject.Find("Heavy").GetComponent<PlayerGun>();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(gameObject.transform.GetChild(0).gameObject.activeInHierarchy)
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                StopCoroutine(life);//eğer çarpışma gerçekleştiyse coroutinin çalışmasına gerek kalmıyor ve stopluyoruz. bunu ypamamızın sebebi ise bulletun setactive false olunca coroutine yarım kalıyor tekrar kullanılıncada kaldığı yerden devam ediyor ve buda bulletın diğer kullanışlarda hedefe çarpmadan ölmesine sebeb oluyor.

                life = DestroyGameObject();//stopladığımız zaman life objesinin içi boşalıyor ve tekrar atama yapmamız gerekiyor.

                if (transform.GetChild(0).gameObject.activeInHierarchy)
                {
                    transform.GetChild(0).gameObject.SetActive(false);

                    Debug.Log(transform.name + "died." + collision.gameObject.name);
                }

                CollisionBulletParticals();

                if (playerGun.hit.transform != null)
                {
                    //vurduğumuz enemynin propertilerini null bir objeye atıyoruz.

                    EnemyStats enemy = playerGun.hit.transform.GetComponent<EnemyStats>();

                    HealthBarHandler healthBar = playerGun.hit.transform.GetComponentInChildren<HealthBarHandler>();

                    if (healthBar != null)
                    {
                        healthBar.SetHealthBarValue(healthBar.GetHealthBarValue() - (float)((float)playerGun.damage / (float)enemy.maxHealth));
                    }

                    if (enemy != null)
                    {
                        enemy.TakeDamage(playerGun.damage);

                        BloodParticals();
                    }
                }

                transform.position = playerGun.transform.position;//burdada işlevi bittikten sonra başlangıç konumuna dönmesini sağlıyoruz. set active false ken bile ortalıkta dolanmamış oluyor.
            }
            else if (collision.gameObject.CompareTag("Ground"))
            {
                StopCoroutine(life);

                life = DestroyGameObject();

                if (transform.GetChild(0).gameObject.activeInHierarchy)
                {
                    transform.GetChild(0).gameObject.SetActive(false);

                    Debug.Log(transform.name + "died." + collision.gameObject.name);
                }

                CollisionBulletParticals();

                transform.position = playerGun.transform.position;
            }
            else if (collision.gameObject.CompareTag("Wall"))
            {
                StopCoroutine(life);

                life = DestroyGameObject();

                if (transform.GetChild(0).gameObject.activeInHierarchy)
                {
                    transform.GetChild(0).gameObject.SetActive(false);

                    Debug.Log(transform.name + "died." + collision.gameObject.name);
                }

                CollisionBulletParticals();

                transform.position = playerGun.transform.position;
            }
            else if (collision.gameObject.CompareTag("Booster"))
            {
                StopCoroutine(life);

                life = DestroyGameObject();

                if (transform.GetChild(0).gameObject.activeInHierarchy)
                {
                    transform.GetChild(0).gameObject.SetActive(false);

                    Debug.Log(transform.name + "died." + collision.gameObject.name);
                }

                transform.position = playerGun.transform.position;
            }
            else if(collision.gameObject.CompareTag("Bullet"))//bulletın bulleta çarpmasını burada kontrol ediyoruz. bullet bulleta çarpınca ignore ediyoruz ve yoluna devam ediyor.
            {
                Physics.IgnoreCollision(collision.gameObject.GetComponent<Collider>(), GetComponent<Collider>());
            }
            else
            {
                StopCoroutine(life);

                life = DestroyGameObject();

                if (transform.GetChild(0).gameObject.activeInHierarchy)
                {
                    transform.GetChild(0).gameObject.SetActive(false);

                    Debug.Log(transform.name + "died." + collision.gameObject.name);
                }

                transform.position = playerGun.transform.position;
            }
        }
    }

    IEnumerator DestroyGameObject()
    {
        isBorn = false;//bu boolen aynı zamanda fonksiyona bir kere girilmesini sağlıyor.

        for (int i = 1; i <= 100; i++)//bunu for loopuyla yapmamın sebebi coroutine istediğim zaman yarıda kesmek istememdir.i 100 olursa yani obje hiç bir yere çarpmamış olacak if blockunun içibne girip setactivefalse oluyor.
        {
            yield return new WaitForSeconds(lifeTime / 100);

            Debug.Log(transform.name + "living" + lifeTime/100);

            if(i==100)
            {
                if (transform.GetChild(0).gameObject.activeInHierarchy)
                {
                    transform.GetChild(0).gameObject.SetActive(false);

                    Debug.Log(transform.name + "died. with life time");
                }

                transform.position = playerGun.transform.position;
            }
        }
    }

    void CollisionBulletParticals()//bulletın çaprma efekti
    {
        ParticleSystem bulletImpact = ObjectPoolingManager.instance.GetBulletImpact();//pooladığım bullet efektini get edip partical objete atayıp kontrol ediyorum.

        if(bulletImpact != null)
        {
            bulletImpact.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);

            if (!bulletImpact.isPlaying)
            {
                bulletImpact.Play();
            }
        }
    }

    void BloodParticals()//bullet kan efekti.
    {
        ParticleSystem bloodEffect = ObjectPoolingManager.instance.GetBloodEffect();//pooladığım kan efektini get edip partical objete atayıp kontrol ediyorum.

        if (bloodEffect != null)
        {
            bloodEffect.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);

            if (!bloodEffect.isPlaying)
            {
                bloodEffect.Play();
            }
        }
    }
}
