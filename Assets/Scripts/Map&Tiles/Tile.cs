using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Tile : MonoBehaviour
{
    public int xIndex;
    public int yIndex;
    public int zIndex;

    Map m_map;
    public void Init(int x, int y, int z, Map map)
    {
        xIndex = x;
        yIndex = y;
        zIndex = z;
        m_map = map;
    }
}
