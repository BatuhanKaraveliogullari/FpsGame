using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public float lookRadius = 10f;

    public Transform targetPlayer;

    public List<Transform> walls;
    public List<Transform> currentwalls;

    public bool enemyDetected = false;
    public bool playerInvisible = false;

    void Start()
    {
        targetPlayer = GameObject.Find("FirstPersonPlayer").GetComponent<Transform>();

        SetWalls();
    }

    void Update()
    {
        AIManager();
    }

    public void SetWalls()
    {
        walls.Clear();

        if(Map.instance != null)
        {
            for (int i = 0; i < Map.instance.walls.Length; i++)
            {
                float distanceOfWall = Vector3.Distance(new Vector3(Map.instance.walls[i].x, Map.instance.walls[i].y, Map.instance.walls[i].z), transform.position);

                if (distanceOfWall < lookRadius)
                {
                    Transform wall = GameObject.Find("Wall (" + i + ")").GetComponent<Transform>();

                    walls.Add(wall);
                }
            }
        }
    }

    void FaceTarget()
    {
        Vector3 direction = (targetPlayer.position - transform.position).normalized;

        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
    }

    void AIManager()
    {
        playerInvisible = false;

        if(GameManager.instance.isStarted)
        {
            AreaControl(targetPlayer.position.x, targetPlayer.position.z, WhichWallIsBetweenEnemyAndPlayer());

            float distance = Vector3.Distance(targetPlayer.position, transform.position);

            if (distance <= lookRadius && !playerInvisible)
            {
                FaceTarget();

                enemyDetected = true;
            }
            else
            {
                enemyDetected = false;
            }
        }
    }

    void AreaControl(float x, float z, List<Transform> walls)
    {
        bool underFirstLine;
        bool underSecondLine;
        bool underThirdLine;

        Vector3 w1;
        Vector3 w2;

        float value1;
        float value2;
        float value3;
        float value4;

        foreach (Transform wall in walls)
        {
            if(wall != null)
            {
                if (wall.GetComponent<Wall>().angle == 0 || wall.GetComponent<Wall>().angle == 180)
                {
                    w1 = new Vector3(wall.position.x - 3f, 2, wall.position.z);
                    w2 = new Vector3(wall.position.x + 3f, 2, wall.position.z);

                    value1 = ((x - transform.position.x) * (transform.position.z - w1.z)) - ((z - transform.position.z) * (transform.position.x - w1.x));
                    value2 = ((x - transform.position.x) * (transform.position.z - w2.z)) - ((z - transform.position.z) * (transform.position.x - w2.x));
                    value3 = ((x - w1.x) * (w1.z - w2.z)) - ((z - w1.z) * (w1.x - w2.x));
                    value4 = ((transform.position.x - w1.x) * (w1.z - w2.z)) - ((transform.position.z - w1.z) * (w1.x - w2.x));

                    if (value4 > 0)
                    {
                        if (value1 > 0)
                            underFirstLine = true;
                        else
                            underFirstLine = false;
                        if (value2 < 0)
                            underSecondLine = true;
                        else
                            underSecondLine = false;
                        if (value3 < 0)
                            underThirdLine = true;
                        else
                            underThirdLine = false;
                    }
                    else
                    {
                        if (value1 < 0)
                            underFirstLine = true;
                        else
                            underFirstLine = false;
                        if (value2 > 0)
                            underSecondLine = true;
                        else
                            underSecondLine = false;
                        if (value3 > 0)
                            underThirdLine = true;
                        else
                            underThirdLine = false;
                    }
                }
                else if (wall.GetComponent<Wall>().angle == 90)
                {
                    w1 = new Vector3(wall.position.x, 2, wall.position.z - 3f);
                    w2 = new Vector3(wall.position.x, 2, wall.position.z + 3f);

                    value1 = ((x - transform.position.x) * (transform.position.z - w1.z)) - ((z - transform.position.z) * (transform.position.x - w1.x));
                    value2 = ((x - transform.position.x) * (transform.position.z - w2.z)) - ((z - transform.position.z) * (transform.position.x - w2.x));
                    value3 = ((x - w1.x) * (w1.z - w2.z)) - ((z - w1.z) * (w1.x - w2.x));
                    value4 = ((transform.position.x - w1.x) * (w1.z - w2.z)) - ((transform.position.z - w1.z) * (w1.x - w2.x));

                    if (value4 > 0)
                    {
                        if (value1 > 0)
                            underFirstLine = true;
                        else
                            underFirstLine = false;
                        if (value2 < 0)
                            underSecondLine = true;
                        else
                            underSecondLine = false;
                        if (value3 < 0)
                            underThirdLine = true;
                        else
                            underThirdLine = false;
                    }
                    else
                    {
                        if (value1 < 0)
                            underFirstLine = true;
                        else
                            underFirstLine = false;
                        if (value2 > 0)
                            underSecondLine = true;
                        else
                            underSecondLine = false;
                        if (value3 > 0)
                            underThirdLine = true;
                        else
                            underThirdLine = false;
                    }
                }
                else
                {
                    w1 = new Vector3(wall.position.x - (3f * Mathf.Cos(Mathf.Deg2Rad * wall.GetComponent<Wall>().angle)), 2, wall.position.z - (3f * Mathf.Sin(Mathf.Deg2Rad * wall.GetComponent<Wall>().angle)));
                    w2 = new Vector3(wall.position.x + (3f * Mathf.Cos(Mathf.Deg2Rad * wall.GetComponent<Wall>().angle)), 2, wall.position.z + (3f * Mathf.Sin(Mathf.Deg2Rad * wall.GetComponent<Wall>().angle)));

                    value1 = ((x - transform.position.x) * (transform.position.z - w1.z)) - ((z - transform.position.z) * (transform.position.x - w1.x));
                    value2 = ((x - transform.position.x) * (transform.position.z - w2.z)) - ((z - transform.position.z) * (transform.position.x - w2.x));
                    value3 = ((x - w1.x) * (w1.z - w2.z)) - ((z - w1.z) * (w1.x - w2.x));
                    value4 = ((transform.position.x - w1.x) * (w1.z - w2.z)) - ((transform.position.z - w1.z) * (w1.x - w2.x));

                    if (value4 > 0)
                    {
                        if (value1 > 0)
                            underFirstLine = true;
                        else
                            underFirstLine = false;
                        if (value2 < 0)
                            underSecondLine = true;
                        else
                            underSecondLine = false;
                        if (value3 < 0)
                            underThirdLine = true;
                        else
                            underThirdLine = false;
                    }
                    else
                    {
                        if (value1 < 0)
                            underFirstLine = true;
                        else
                            underFirstLine = false;
                        if (value2 > 0)
                            underSecondLine = true;
                        else
                            underSecondLine = false;
                        if (value3 > 0)
                            underThirdLine = true;
                        else
                            underThirdLine = false;
                    }
                }

                if (underFirstLine && underSecondLine && underThirdLine)
                {
                    Debug.Log("Player ınvisible from" + transform.name + "because of" + wall.name);

                    Debug.Log(wall.transform.position);

                    playerInvisible = true;

                    break;
                }
                else
                {
                    playerInvisible = false;
                }
            }
        }
    }

    List<Transform> WhichWallIsBetweenEnemyAndPlayer()
    {
        Vector3 w1;
        Vector3 w2;

        float value4;
        float value5;

        foreach (Transform wall in walls)
        {
            if(wall != null)
            {
                if (wall.GetComponent<Wall>().angle == 0 || wall.GetComponent<Wall>().angle == 180)
                {
                    w1 = new Vector3(wall.position.x - 3, 2, wall.position.z);
                    w2 = new Vector3(wall.position.x + 3, 2, wall.position.z);

                    value4 = ((transform.position.x - w1.x) * (w1.z - w2.z)) - ((transform.position.z - w1.z) * (w1.x - w2.x));
                    value5 = ((targetPlayer.position.x - w1.x) * (w1.z - w2.z)) - ((targetPlayer.position.z - w1.z) * (w1.x - w2.x));

                    if (value4 < 0)
                    {
                        if (value5 > 0)
                        {
                            if (!currentwalls.Contains(wall))
                            {
                                currentwalls.Add(wall);
                            }
                        }
                    }
                    else
                    {
                        if (value5 < 0)
                        {
                            if (!currentwalls.Contains(wall))
                            {
                                currentwalls.Add(wall);
                            }
                        }
                    }
                }
                else if (wall.GetComponent<Wall>().angle == 90)
                {
                    w1 = new Vector3(wall.position.x, 2, wall.position.z - 3);
                    w2 = new Vector3(wall.position.x, 2, wall.position.z + 3);

                    value4 = ((transform.position.x - w1.x) * (w1.z - w2.z)) - ((transform.position.z - w1.z) * (w1.x - w2.x));
                    value5 = ((targetPlayer.position.x - w1.x) * (w1.z - w2.z)) - ((targetPlayer.position.z - w1.z) * (w1.x - w2.x));

                    if (value4 > 0)
                    {
                        if (value5 < 0)
                        {
                            if (!currentwalls.Contains(wall))
                            {
                                currentwalls.Add(wall);
                            }
                        }
                    }
                    else
                    {
                        if (value5 > 0)
                        {
                            if (!currentwalls.Contains(wall))
                            {
                                currentwalls.Add(wall);
                            }
                        }
                    }
                }
                else
                {
                    w1 = new Vector3(wall.position.x - (3 * Mathf.Cos(Mathf.Deg2Rad * wall.GetComponent<Wall>().angle)), 2, wall.position.z - (3 * Mathf.Sin(Mathf.Deg2Rad * wall.GetComponent<Wall>().angle)));
                    w2 = new Vector3(wall.position.x + (3 * Mathf.Cos(Mathf.Deg2Rad * wall.GetComponent<Wall>().angle)), 2, wall.position.z + (3 * Mathf.Sin(Mathf.Deg2Rad * wall.GetComponent<Wall>().angle)));

                    value4 = ((transform.position.x - w1.x) * (w1.z - w2.z)) - ((transform.position.z - w1.z) * (w1.x - w2.x));
                    value5 = ((targetPlayer.position.x - w1.x) * (w1.z - w2.z)) - ((targetPlayer.position.z - w1.z) * (w1.x - w2.x));

                    if (value4 < 0)
                    {
                        if (value5 > 0)
                        {
                            if (!currentwalls.Contains(wall))
                            {
                                currentwalls.Add(wall);
                            }
                        }
                    }
                    else
                    {
                        if (value5 < 0)
                        {
                            if (!currentwalls.Contains(wall))
                            {
                                currentwalls.Add(wall);
                            }
                        }
                    }
                }
            }
        }

        return currentwalls;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }
}
