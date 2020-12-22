using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerGun : Gun
{
    public Camera fpsCam;//hit variableını oluşturmak adınayapılmış örneklendirilmiş bir camera variableı

    public RaycastHit hit;//bulletın target variableını kontrol etmek adına oluşturulmış bir raycasthit variableı

    public Text bulletCounter;//bulletların sayısını scenede gösteren variable

    public Image reloadImage;//gunu image propertysi

    public bool isReloading = false;//reload control boolu

    float otherTimeToFire = 0f;//firerate control variableı

    readonly float range = 100f;//hit variable için constant bir variable

    private void Start()
    {
        if (reloadImage != null)
        {
            reloadImage.type = Image.Type.Filled;

            reloadImage.fillMethod = Image.FillMethod.Radial360;

            reloadImage.fillOrigin = (int)Image.Origin360.Top;
        }
    }

    private void Update()
    {
        ShootControl();
    }

    void ShootControl()//ateş etmeyi control eden method
    {
        if (Input.GetButton("Fire1") && Time.time >= otherTimeToFire && !isReloading)//sol clickle ateş kontrolu ve basılı tutulduğunda çalışan method birkaç parametreyle birlikte
        {
            if (mag > usedBullets)
            {
                otherTimeToFire = Time.time + 1f / fireRate;

                Shoot();
            }
            else
            {
                StartCoroutine(ReloadTimerRoutine());
            }
        }
        else if (Input.GetButtonDown("Fire2") && usedBullets != 0 && !isReloading)//keyfi reload, sağ click ile çalışan method
        {
            StartCoroutine(ReloadTimerRoutine());
        }
    }

    public override void Shoot()//polymorphism örneği base classtaki sanal methoda averride edilmiş method
    {
        if(GameManager.instance.isStarted)
        {
            if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))//hit variable içini dolu olduğunu gösteren boolen
            {
                Debug.Log(hit.transform.name);

                ObjectPoolingManager.instance.GetBullet();

                if(gunSound != null)
                {
                    gunSound.Play();
                }

                usedBullets++;

                if(bulletCounter != null)
                {
                    bulletCounter.text = usedBullets.ToString();
                }
            }
        }
    }

    IEnumerator ReloadTimerRoutine()
    {
        isReloading = true;

        Debug.Log("reloading....." + gameObject.name);

        float currentAmount = 0.01f;//imageın dolduren local bir variable

        for (int i = 0; i < 100; i++)//imageı reload eden for loop ama toplamda bekleme süresi aynı oluyor
        {
            yield return new WaitForSeconds(reloadTime / 100);

            if(reloadImage != null)
            {
                reloadImage.fillAmount = currentAmount;
            }

            currentAmount += 0.01f;
        }

        usedBullets = 0;

        if (bulletCounter != null)
        {
            bulletCounter.text = usedBullets.ToString();
        }

        Debug.Log("Realoding is done" + gameObject.name);

        isReloading = false;
    }
}
