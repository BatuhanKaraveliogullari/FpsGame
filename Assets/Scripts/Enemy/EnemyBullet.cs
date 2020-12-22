using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : Bullet
{
    public Vector3 targetPosition;//bulletın gideceği hedef

    public bool enterUpdateOnce = false;//bir tane hedef nokta bulmak adına updatede boolen kontrolü

    void Update()
    {
        if(transform.GetChild(0).gameObject.activeInHierarchy && !enterUpdateOnce)//enemy bulletın ateş edildiğini childının activeliğinin kontrol ediyoruz.
        {
            enterUpdateOnce = true;

            SetTargetPosition();

            Invoke("DeactiveEnemyBullet", lifeTime);
        }

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, 25 * Time.deltaTime);//enemey bullet movement
    }

    void DeactiveEnemyBullet()//eğer bullet bir yere çarpmaz ise kendi life timemin execution
    {
        transform.GetChild(0).gameObject.SetActive(false);

        enterUpdateOnce = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))//burada bulletın playera çarptığı zamanı kontrol ediyoruz
        {
            transform.GetChild(0).gameObject.SetActive(false);

            PlayerStats player = collision.gameObject.GetComponent<PlayerStats>();//playerın healthınde değişiklik olacağı için player stat classından bir öbje örneği oluşturdum.

            HealthBarHandler healthBar = collision.gameObject.GetComponentInChildren<HealthBarHandler>();//aynı şekilde playera ait olan health bar değişeçeği için o classtan bir örnek öbej oluşturdum

            Gun gun = transform.parent.gameObject.GetComponent<EnemyGun>();//ve buradan alacağım silaha ait değerler olduğu için damage gibi onu da oluşturdum.

            if(healthBar != null && player != null && gun.gameObject != null)//burada da üç farklı objeyi kullanarak playerın canını azaltmayı amaçladım. bunları burda örneklendirme sebebim ise oyun geliştirilip enemylere farklı özellikler verilirse bunları doğru bir şekilde çekmek.
            {
                player.TakeDamage(gun.damage);//player damage veriyorum.

                healthBar.SetHealthBarValue(healthBar.GetHealthBarValue() - (float)((float)gun.damage / (float)player.maxHealth));//playerın health barını kontrol ediyorum.
            }

            enterUpdateOnce = false;
        }
        else if(collision.gameObject.CompareTag("Ground"))//burada da bulletın farklı bir zemine çaprmasını kontrol ediyorum.
        {
            transform.GetChild(0).gameObject.SetActive(false);

            enterUpdateOnce = false;
        }
        else if(collision.gameObject.CompareTag("Wall"))//burada da bulletın farklı bir zemine çaprmasını kontrol ediyorum.
        {
            transform.GetChild(0).gameObject.SetActive(false);

            enterUpdateOnce = false;
        }
    }

    public void SetTargetPosition()//bu fonsiyonda bir adet target position alıyorum çğnkü bulletın playerı takip etmesini istemiyorum bir doğrultuda gtimesinin istiyorum.
    {
        targetPosition = ObjectPoolingManager.instance.targetPos;
    }
}
