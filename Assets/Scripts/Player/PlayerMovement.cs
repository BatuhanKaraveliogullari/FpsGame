using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;

    public float speed = 12f;
    public float gravity = -9.81f;

    Vector3 velocity;

    public HealthBooster healthBooster;

    public HealthBarHandler healthBarHandler;

    public PlayerStats player;

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
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            Vector3 move = transform.right * x + transform.forward * z;

            controller.Move(move * speed * Time.deltaTime);

            velocity.y += gravity * Time.deltaTime;

            controller.Move(velocity * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(player != null)
        {
            int increaseAmount = (player.maxHealth - player.currentHealth);

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
