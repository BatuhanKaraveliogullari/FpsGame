using System.Collections;
using UnityEngine;

public class PlayerBullet : Bullet
{
    public PlayerGun playerGun;

    public GunSwitch gunSwitch;

    public Vector3 direction;

    IEnumerator life;

    public bool isBorn = false;

    public bool isCalculated = false;

    private void Start()
    {
        playerGun = GameObject.FindWithTag("PlayerGun").GetComponent<PlayerGun>();

        gunSwitch = GameObject.FindGameObjectWithTag("Player").GetComponent<GunSwitch>();

        life = DestroyGameObject();
    }

    private void Update()
    {
        if(transform.GetChild(0).gameObject.activeInHierarchy)
        {
            transform.position = Vector3.MoveTowards(transform.position, playerGun.hit.point, playerGun.bulletSpeed * Time.deltaTime);
        }

        if(isBorn)
        {
            life = DestroyGameObject();

            StartCoroutine(life);
        }

        SwitchGun();
    }

    public void SwitchGun()
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
                StopCoroutine(life);

                life = DestroyGameObject();

                if (transform.GetChild(0).gameObject.activeInHierarchy)
                {
                    transform.GetChild(0).gameObject.SetActive(false);

                    Debug.Log(transform.name + "died." + collision.gameObject.name);
                }

                CollisionBulletParticals();

                if (playerGun.hit.transform != null)
                {
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

                transform.position = playerGun.transform.position;
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
            else if(collision.gameObject.CompareTag("Bullet"))
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
        isBorn = false;

        for (int i = 0; i < 100; i++)
        {
            yield return new WaitForSeconds(lifeTime / 100);

            Debug.Log(transform.name + "living");
        }

        if (transform.GetChild(0).gameObject.activeInHierarchy)
        {
            transform.GetChild(0).gameObject.SetActive(false);

            Debug.Log(transform.name + "died. with life time");
        }

        transform.position = playerGun.transform.position;
    }

    void CollisionBulletParticals()
    {
        ParticleSystem bulletImpact = ObjectPoolingManager.instance.GetBulletImpact();

        bulletImpact.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        if(!bulletImpact.isPlaying)
        {
            bulletImpact.Play();
        }
    }

    void BloodParticals()
    {
        ParticleSystem bloodEffect = ObjectPoolingManager.instance.GetBloodEffect();

        bloodEffect.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        if (!bloodEffect.isPlaying)
        {
            bloodEffect.Play();
        }
    }
}
