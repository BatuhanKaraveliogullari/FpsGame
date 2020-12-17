using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeAnimation : MonoBehaviour
{
    public Animation anim;

    void Update()
    {
        if(GameManager.instance.isStarted)
        {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
            {
                anim.Play("Move");
            }
            else
            {
                anim.Play("Idle");
            }
        }
    }
}
