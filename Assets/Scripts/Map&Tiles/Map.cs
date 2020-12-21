﻿using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Map : MonoBehaviour
{
    #region Singleton
    public static Map instance;

    private void Awake()
    {
        instance = this;
    }
    #endregion

    [System.Serializable] public class StartingWalls
    {
        public Wall wallPrefab;

        public int x;
        public float y;
        public int z;

        [Range(0,180)]
        public float angle;
    }

    [Header("Maze Walls")]
    public StartingWalls[] walls;
    public List<Wall> wallObjects;

    [Header("Parents")]
    public Transform wallParent;
    public Transform groundParent;

    [Header("Dimensions Of Tiles")]
    public int width;
    public int height;
    public int depth;

    [Header("Prefabs")]
    public GameObject whiteTile;
    public GameObject blackTile;
    public GameObject sideWallPrefab;
    public GameObject playerPrefab;

    Tile[,,] m_tiles;

    int counter = 0;

    void Start()
    {
        m_tiles = new Tile[width, height, depth];

        SetupTiles();

        SetupWalls();

        SetupSideWalls();
    }

    #region Making Map
    private void MakeTiles(GameObject prefab, int x, int y, int z)
    {
        if (prefab != null && IsWithinBounds(x, y, z))
        {
            GameObject tile = Instantiate(prefab, new Vector3(x, y, z), Quaternion.identity) as GameObject;

            if(tile != null)
            {
                tile.name = "Tile (" + x + "," + y + "," + z + ")";

                m_tiles[x, y, z] = tile.GetComponent<Tile>();

                tile.transform.SetParent(groundParent);

                m_tiles[x, y, z].Init(x, y, z, this);
            }
        }
    }

    void SetupSideWalls()
    {
        if(sideWallPrefab != null)
        {
            int k = 1;

            for (int i = 2; i < width; i += 5)
            {
                GameObject sideWall = Instantiate(sideWallPrefab, new Vector3(i, 2, 0), Quaternion.Euler(0, 0, 0), wallParent);

                sideWall.name = "Bottom Side Wall (" + k + ")";

                k++;
            }

            for (int i = 2; i < depth; i += 5)
            {
                GameObject sideWall = Instantiate(sideWallPrefab, new Vector3(50, 2, i), Quaternion.Euler(0, -90, 0), wallParent);

                sideWall.name = "Right Side Wall (" + k + ")";

                k++;
            }

            for (int i = 47; i > 0; i -= 5)
            {
                GameObject sideWall = Instantiate(sideWallPrefab, new Vector3(i, 2, 50), Quaternion.Euler(0, 0, 0), wallParent);

                sideWall.name = "Top Side Wall (" + k + ")";

                k++;
            }

            for (int i = 48; i > 0; i -= 5)
            {
                GameObject sideWall = Instantiate(sideWallPrefab, new Vector3(-1, 2, i), Quaternion.Euler(0, 90, 0), wallParent);

                sideWall.name = "Left Side Wall (" + k + ")";

                k++;
            }
        }
    }

    void SetupTiles()
    {
        if (depth % 2 == 0)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    for (int k = 1; k < depth; k++)
                    {
                        if (m_tiles[i, j, k] == null)
                        {
                            if (counter % 2 == 0)
                            {
                                MakeTiles(whiteTile, i, j, k);
                            }
                            else
                            {
                                MakeTiles(blackTile, i, j, k);
                            }

                            counter++;
                        }
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    for (int k = 0; k < depth; k++)
                    {
                        if (m_tiles[i, j, k] == null)
                        {
                            if (counter % 2 == 0)
                            {
                                MakeTiles(whiteTile, i, j, k);
                            }
                            else
                            {
                                MakeTiles(blackTile, i, j, k);
                            }

                            counter++;
                        }
                    }
                }
            }
        }
    }

    void SetupWalls()
    {
        int i = 0;

        foreach (StartingWalls wall in walls)
        {
            if (wall != null)
            {
                Wall wallObject = Instantiate(wall.wallPrefab, new Vector3(wall.x, wall.y, wall.z), Quaternion.Euler(new Vector3(0, (-1 * wall.angle), 0)));

                wallObject.angle = wall.angle;

                wallObject.name = "Wall (" + i + ")";

                wallObject.transform.SetParent(wallParent);

                wallObjects.Add(wallObject);

                i++;
            }
        }
    }
    #endregion

    #region Checklist
    bool IsWithinBounds(int x, int y, int z)
    {
        return ((x >= 0 && x < width) && (y >= 0 && y < height) && (z >= 0 && z < depth));
    }

    public bool IsWithinBoundsforObjects(int x, int y, int z)
    {
        return ((x > 0 && x < width) && (y >= 0 && y < height) && (z > 0 && z < depth));
    }

    public bool IsInDangerRangeOfWall(float x, float y, float z)
    {
        foreach (StartingWalls wall in walls)
        {
            if(wall != null)
            {
                Vector3 wallPosition = new Vector3(wall.x, wall.y, wall.z);
                Vector3 objectPosition = new Vector3(x, y, z);

                Vector3 w1;
                Vector3 w2;

                float value; 
                float distance = Vector3.Distance(wallPosition, objectPosition);

                if (wall.angle == 0 || wall.angle == 180)
                {
                    w1 = new Vector3(wall.x - 3f, 2, wall.z);
                    w2 = new Vector3(wall.x + 3f, 2, wall.z);

                    value = ((x - w1.x) * (w1.z - w2.z)) - ((z - w1.z) * (w1.x - w2.x));

                    if (value == 0)
                    {
                        if (distance >= -5f && distance <= 5f)
                        {
                            return true;
                        }
                    }
                }
                else if(wall.angle == 90)
                {
                    w1 = new Vector3(wall.x, 2, wall.z - 3f);
                    w2 = new Vector3(wall.x, 2, wall.z + 3f);

                    value = ((x - w1.x) * (w1.z - w2.z)) - ((z - w1.z) * (w1.x - w2.x));

                    if (value == 0)
                    {
                        if (distance >= -5f && distance <= 5f)
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    w1 = new Vector3(wall.x - (3f * Mathf.Cos(Mathf.Deg2Rad * wall.angle)), 2, wall.z - (3f * Mathf.Sin(Mathf.Deg2Rad * wall.angle)));
                    w2 = new Vector3(wall.x + (3f * Mathf.Cos(Mathf.Deg2Rad * wall.angle)), 2, wall.z + (3f * Mathf.Sin(Mathf.Deg2Rad * wall.angle)));

                    value = ((x - w1.x) * (w1.z - w2.z)) - ((z - w1.z) * (w1.x - w2.x));

                    if (value == 0)
                    {
                        if (distance >= -5f && distance <= 5f)
                        {
                            return true;
                        }
                    }
                }
            }
        }

        return false;
    }
    #endregion
}
