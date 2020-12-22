using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunSwitch : MonoBehaviour
{
    public int selctedGun = 0;//seçilen silah

    public Transform[] guns;//seçilecek olan gun arrayi

    public AudioSource gunSwitch;//silah değişim sesi

    public GameObject[] gunImages;//uı kontrolü için array

    void Start()
    {
        SelectGun();
    }

    void Update()
    {
        UserInput();

        UpdateGunIcons();
    }

    void UpdateGunIcons()//seçilen gunun ekranda diğerlerinde farklı görülmesini sağlayan method.canvas group componenti içerisindeki alpha değerleriyle oynayarak
    {
        switch (selctedGun)
        {
            case 0:
                if(gunImages[0] != null)
                    gunImages[0].GetComponent<CanvasGroup>().alpha = 1;
                if(gunImages[1] != null)
                    gunImages[1].GetComponent<CanvasGroup>().alpha = 0.5f;
                if(gunImages[2] != null)
                    gunImages[2].GetComponent<CanvasGroup>().alpha = 0.5f;
                break;       
            case 1:
                if (gunImages[0] != null)
                    gunImages[0].GetComponent<CanvasGroup>().alpha = 0.5f;
                if (gunImages[1] != null)
                    gunImages[1].GetComponent<CanvasGroup>().alpha = 1f;
                if (gunImages[2] != null)
                    gunImages[2].GetComponent<CanvasGroup>().alpha = 0.5f;
                break;        
            case 2:
                if (gunImages[0] != null)
                    gunImages[0].GetComponent<CanvasGroup>().alpha = 0.5f;
                if (gunImages[1] != null)
                    gunImages[1].GetComponent<CanvasGroup>().alpha = 0.5f;
                if (gunImages[2] != null)
                    gunImages[2].GetComponent<CanvasGroup>().alpha = 1f;
                break;
        }
    }

    void UserInput()//1,2 ve 3 tuşlarıyla gunswitch yapma ve olamsı gereken kontroller. mesela tuşlara basılırken gun switch yapılamaz
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && selctedGun != 0 && !Input.GetButton("Fire1") && !Input.GetButton("Fire2"))
        {
            if(gunSwitch != null)
                gunSwitch.Play();

            if(ObjectPoolingManager.instance != null)
                ObjectPoolingManager.instance.DeactivateBullets();

            selctedGun = 0;

            SelectGun();
        } 
        
        else if (Input.GetKeyDown(KeyCode.Alpha2) && selctedGun != 1 && !Input.GetButton("Fire1") && !Input.GetButton("Fire2"))
        {
            if (gunSwitch != null)
                gunSwitch.Play();

            if (ObjectPoolingManager.instance != null)
                ObjectPoolingManager.instance.DeactivateBullets();

            selctedGun = 1;

            SelectGun();
        }   
        
        else if (Input.GetKeyDown(KeyCode.Alpha3) && selctedGun != 2 && !Input.GetButton("Fire1") && !Input.GetButton("Fire2"))
        {
            if (gunSwitch != null)
                gunSwitch.Play();

            if (ObjectPoolingManager.instance != null)
                ObjectPoolingManager.instance.DeactivateBullets();

            selctedGun = 2;

            SelectGun();
        }
    }

    void SelectGun()
    {
        for (int j = 0; j < guns.Length; j++)//seçilmeyen gun objelerini hiyerarşide setactive false ediyoruz sadece seçilen true oluyor. oda aşağıdaki loopta
        {
            if(guns[j] != null)
                guns[j].gameObject.SetActive(false);
        }

        int i = 0;

        foreach (Transform gun in guns)
        {
            if (i == selctedGun)
            {
                if(gun.gameObject != null)
                {
                    gun.gameObject.SetActive(true);

                    gun.gameObject.GetComponent<PlayerGun>().isReloading = false;
                }
            }

            i++;
        }
    }
}
