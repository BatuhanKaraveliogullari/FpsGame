using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gun : MonoBehaviour
{
    //gunların ata classıdır. sanal method bulundurur. 

    [Header("Gun Properties")]
    public float fireRate = 15f; 
    public float reloadTime = 5f;
    public int mag = 20;
    public int damage = 10;

    [HideInInspector]
    public float bulletSpeed = 25f; //bulleta verdiği speed bu değişken olabilir ancak ben sabit tuttum.

    [HideInInspector]
    public int usedBullets = 0;

    public AudioSource gunSound;//her gun classının bir patlama sesi vardır. 

    public virtual void Shoot()
    {
    
    }
}
