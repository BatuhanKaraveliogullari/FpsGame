using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;//first person shooter objesinin yapısında bulunan bir component 

    public float speed = 12f;//charcter speed
    public float gravity = -9.81f;//yer çekimi

    Vector3 velocity;//vectorel hız variableı

    public HealthBarHandler healthBarHandler;//collision burada kontrol edilince eklene healthide buırada çağırdım

    public PlayerStats player;//health activiteleri için first person shooter orneği

    private void Start()
    {
        if(GameObject.Find("FirstPersonPlayer") != null)
        {
            player = GameObject.Find("FirstPersonPlayer").GetComponent<PlayerStats>();
        }
    }

    void Update()
    {
        PlayerMove();
    }

    void PlayerMove()
    {
        if (GameManager.instance.isStarted)
        {
            float x = Input.GetAxis("Horizontal");//a ve d den alınan -1 ile 1 arasındaki değerler
            float z = Input.GetAxis("Vertical");//w ve s den alınan -1 ile 1 arasındaki değerler

            Vector3 move = transform.right * x + transform.forward * z;//karakteri yatayda hareket ettiren vector.

            controller.Move(move * speed * Time.deltaTime);

            velocity.y += gravity * Time.deltaTime;//yerçekimini sağlayan vector.

            controller.Move(velocity * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(player != null)
        {
            int increaseAmount = (player.maxHealth - player.currentHealth);//booster karakterin canını başlangıçteki seviyeye tamamlar.

            Debug.Log(increaseAmount);

            if (other.gameObject.CompareTag("Booster"))
            {
                if(increaseAmount != 0)
                {
                    other.gameObject.SetActive(false);

                    if (healthBarHandler != null)
                    {
                        player.currentHealth += increaseAmount;

                        healthBarHandler.SetHealthBarValue(player.currentHealth / player.currentHealth);
                    }
                }
            }
        }
    }
}
