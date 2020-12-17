using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunSwitch : MonoBehaviour
{
    public int selctedGun = 0;

    public Transform[] guns;

    public Bullet bullet;

    public AudioSource gunSwitch;

    void Start()
    {
        SelectGun();
    }

    void Update()
    {
        UserInput();
    }

    void UserInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && selctedGun != 0)
        {
            gunSwitch.Play();

            ObjectPoolingManager.instance.DeactivateBullets();

            selctedGun = 0;

            SelectGun();
        } 
        
        else if (Input.GetKeyDown(KeyCode.Alpha2) && selctedGun != 1)
        {
            gunSwitch.Play();

            ObjectPoolingManager.instance.DeactivateBullets();

            selctedGun = 1;

            SelectGun();
        }   
        
        else if (Input.GetKeyDown(KeyCode.Alpha3) && selctedGun != 2)
        {
            gunSwitch.Play();

            ObjectPoolingManager.instance.DeactivateBullets();

            selctedGun = 2;

            SelectGun();
        }
    }

    void SelectGun()
    {
        for (int j = 0; j < guns.Length; j++)
        {
            guns[j].gameObject.SetActive(false);
        }

        int i = 0;

        foreach (Transform gun in guns)
        {
            if (i == selctedGun)
            {
                gun.gameObject.SetActive(true);

                gun.GetComponent<PlayerGun>().isReloading = false;
            }

            i++;
        }
    }
}
