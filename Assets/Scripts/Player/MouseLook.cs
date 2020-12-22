using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public Transform playerBody;//playerı ekranda döndüremek adına

    float xRotation = 0f;//döndürme parametresi

    public float mouseSensitivity = 100f;//hassasiyet

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;//mouseyi scenenin ortasına kitmek ve disappear lımak için
    }

    private void LateUpdate()
    {
        if(GameManager.instance.isStarted)
        {
            MouseControl();
        }
    }

    void MouseControl()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;//mouse hareklerini zamana ve hassasiyete oranlu bir float variableının içine atıp kontrolu sağlıyoruz.
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);//player ters dönememesi açısından -90 ve +90 derecelerri arasında değer aldırıyorum.yani oyunda kameranın aşağı yukarı hareketini sağlıyor.

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);//buda sağa sola hareketini sağlıyor.
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
