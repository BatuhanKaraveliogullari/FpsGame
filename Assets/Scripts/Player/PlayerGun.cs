using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerGun : Gun
{
    public Camera fpsCam;

    public RaycastHit hit;

    public Text bulletCounter;

    public Image reloadImage;

    public bool isReloading = false;

    float otherTimeToFire = 0f;

    readonly float range = 100f;

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

    void ShootControl()
    {
        if (Input.GetButton("Fire1") && Time.time >= otherTimeToFire && !isReloading)
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
        else if (Input.GetButtonDown("Fire2") && usedBullets != 0 && !isReloading)
        {
            StartCoroutine(ReloadTimerRoutine());
        }
    }

    public override void Shoot()
    {
        if(GameManager.instance.isStarted)
        {
            if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
            {
                Debug.Log(hit.transform.name);

                ObjectPoolingManager.instance.GetBullet();

                gunSound.Play();

                usedBullets++;

                bulletCounter.text = usedBullets.ToString();
            }
        }
    }

    IEnumerator ReloadTimerRoutine()
    {
        isReloading = true;

        Debug.Log("reloading....." + gameObject.name);

        float currentAmount = 0.01f;

        for (int i = 0; i < 100; i++)
        {
            yield return new WaitForSeconds(reloadTime / 100);

            reloadImage.fillAmount = currentAmount;

            currentAmount += 0.01f;
        }

        usedBullets = 0;

        bulletCounter.text = usedBullets.ToString();

        Debug.Log("Realoding is done" + gameObject.name);

        isReloading = false;
    }
}
