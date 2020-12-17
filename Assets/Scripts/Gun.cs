using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gun : MonoBehaviour
{
    [Header("Gun Properties")]
    public float fireRate = 15f;
    public float reloadTime = 5f;
    public int mag = 20;
    public int damage = 10;

    [HideInInspector]
    public float bulletSpeed = 25f;

    [HideInInspector]
    public int usedBullets = 0;

    public AudioSource gunSound;

    public virtual void Shoot()
    {
    
    }
}
