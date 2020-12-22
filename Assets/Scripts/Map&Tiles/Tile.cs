using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    //tileclassındaki objelerin propertyleri

    public int xIndex;//tileın x indeksi
    public int yIndex;//tileın y indeksi
    public int zIndex;//tileın z indeksi

    Map m_map;//tileın ait olduğu map
    public void Init(int x, int y, int z, Map map)//tileın propertylerine değer atama fonksiyonu
    {
        xIndex = x;
        yIndex = y;
        zIndex = z;
        m_map = map;
    }
}
