using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeAnimation : MonoBehaviour
{
    public Animation anim;//first person objesinde bulunan animasyon componenti

    void Update()
    {
        if(GameManager.instance.isStarted)
        {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))//hareket halinde çalışacak olan animasyonun kontrolü
            {
                if(anim != null)
                    anim.Play("Move");
            }
            else//idle halde çalışacak olan animasyonun kontrolü
            {
                if (anim != null)
                    anim.Play("Idle");
            }
        }
    }
}
