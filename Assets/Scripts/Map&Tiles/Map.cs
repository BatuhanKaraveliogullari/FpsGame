using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Map : MonoBehaviour
{
    #region Singleton
    public static Map instance;//hiyerarşide static obje olduğu için bir static örnek tanımı

    private void Awake()
    {
        instance = this;
    }
    #endregion

    [System.Serializable] public class StartingWalls //daha düzenli bir inspector ve maze oluşturmak adına walların propertylerini düzgün verebilmek için class içinde class oluşturdum.
    {
        public Wall wallPrefab;

        public int x;
        public float y;//mazedeki walların positionları
        public int z;

        [Range(0,180)]
        public float angle;//mazedaki walların konumsal açıları
    }

    [Header("Maze Walls")]
    public StartingWalls[] walls;//map classında ve inspectorda kontrol etmek adına örneklendirildi.
    List<StartingWalls> dangerousWalls;//objeler için tehlikeli duvarlar listi

    [Header("Parents")]
    public Transform wallParent;//inspectorun düzeni açısından wallların ve ground tilelarının parentları
    public Transform groundParent;

    [Header("Dimensions Of Tiles")]
    public int width;
    public int height;//map groundunu oluşturmak adına verilen ölçüler
    public int depth;

    [Header("Prefabs")]
    public GameObject whiteTile;
    public GameObject blackTile;//groundun rahat ve tilelardan oluştuğu görülebilmesi açısından black ve white tilelardan oluşturulacak bunlarda prefableri
    public GameObject sideWallPrefab;//oluşturulacak kenar duvarlar için prefab

    Tile[,,] m_tiles;//bu objede 3 dimentionlı bir array oluşturup bütün tilelları bu arrayın içine topladım.

    int counter = 0;//bu variable da ardarda beyaz veya siyah gelmesi adına oluşturuldu

    void Start()
    {
        m_tiles = new Tile[width, height, depth];

        SetupTiles();
         
        SetupWalls();

        SetupSideWalls();

        UnityEngine.Debug.Log("Entredxxx");
    }

    #region Making Map
    private void MakeTiles(GameObject prefab, int x, int y, int z)//Tileların istenen konumda ve istenen game object ile oluşturma konumu
    {
        if (prefab != null && IsWithinBounds(x, y, z))
        {
            GameObject tile = Instantiate(prefab, new Vector3(x, y, z), Quaternion.identity) as GameObject;

            if(tile != null)
            {
                tile.name = "Tile (" + x + "," + y + "," + z + ")";

                m_tiles[x, y, z] = tile.GetComponent<Tile>();//oluşturduğumuz gameobjecte tile componentini atıyoruz çünkü tile özelliği vermemiz gerekiyor

                tile.transform.SetParent(groundParent);

                m_tiles[x, y, z].Init(x, y, z, this);//burdada tile clasındaki Init yani özellikleri atayan fonksiyonu çağırıp içindeki null propertyleri dolduruyoruz.
            }
        }
    }

    void SetupSideWalls()//Kenar duvarların oluşturulması fonksiyonu
    {
        if(sideWallPrefab != null)
        {
            int k = 1;

            for (int i = 2; i < width; i += 5)//alt duvarların oluşturulması tepe den baktığımızda 
            {
                GameObject sideWall = Instantiate(sideWallPrefab, new Vector3(i, 2, 0), Quaternion.Euler(0, 0, 0), wallParent);

                sideWall.name = "Bottom Side Wall (" + k + ")";

                k++;
            }

            for (int i = 2; i < depth; i += 5)//sağ duvarların oluşturulması tepe den baktığımızda sağ duvarlar olduğu için duvarın açısı 90 derece olmalıdır
            {
                GameObject sideWall = Instantiate(sideWallPrefab, new Vector3(width, 2, i), Quaternion.Euler(0, -90, 0), wallParent);

                sideWall.name = "Right Side Wall (" + k + ")";

                k++;
            }

            for (int i = width - 3; i > 0; i -= 5)//üst duvarların oluşturulması tepe  den bakıldığında
            {
                GameObject sideWall = Instantiate(sideWallPrefab, new Vector3(i, 2, depth), Quaternion.Euler(0, 0, 0), wallParent);

                sideWall.name = "Top Side Wall (" + k + ")";

                k++;
            }

            for (int i = depth - 2; i > 0; i -= 5)//sol duvarların oluşturulması tepeden bakıldığında sol duvarlar olduğu için duvarın açısı 90 derece olmalıdır
            {
                GameObject sideWall = Instantiate(sideWallPrefab, new Vector3(-1, 2, i), Quaternion.Euler(0, 90, 0), wallParent);

                sideWall.name = "Left Side Wall (" + k + ")";

                k++;
            }
        }
    }

    void SetupTiles()//burada scenede dama tablası gibi görünen ground oluşturuyor. depth i tek veçift olarak ayırmamın nedeni ise tablanın dama gibi görünmesini sağlamaktı aksi bir durumda şerit halinde siyah ve beyaz oluşuyordu.
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
                                MakeTiles(whiteTile, i, j, k);//buraya kadar konumu alıp en sonunda alınan konumu ve rengi tile atayıp oluşturuyoruz.
                            }
                            else
                            {
                                MakeTiles(blackTile, i, j, k);//buraya kadar konumu alıp en sonunda alınan konumu ve rengi tile atayıp oluşturuyoruz.
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
                                MakeTiles(whiteTile, i, j, k);//buraya kadar konumu alıp en sonunda alınan konumu ve rengi tile atayıp oluşturuyoruz.
                            }
                            else
                            {
                                MakeTiles(blackTile, i, j, k);//buraya kadar konumu alıp en sonunda alınan konumu ve rengi tile atayıp oluşturuyoruz.
                            }

                            counter++;
                        }
                    }
                }
            }
        }
    }

    void SetupWalls()//inspectordan girilen maze walllarını oluşturan method
    {
        int i = 0;

        foreach (StartingWalls wall in walls)
        {
            if (wall != null)
            {
                Wall wallObject = Instantiate(wall.wallPrefab, new Vector3(wall.x, wall.y, wall.z), Quaternion.Euler(new Vector3(0, (-1 * wall.angle), 0)));//burada açıyı -1 ile çarpmamın sebebi localde konumun farklı değere almasıdır. bunu ispectordan deneyerek buldum.

                wallObject.angle = wall.angle;//bu angle atama işlevini wall abjesine public bir class atadığım için yaptım. iç içe oluşturduğum classı diğer classlardan çağıramadığım için atamayı burada yaptım.

                wallObject.name = "Wall (" + i + ")";

                wallObject.transform.SetParent(wallParent);

                i++;
            }
        }
    }
    #endregion

    #region Checklist
    bool IsWithinBounds(int x, int y, int z)//bu bounds diye isimlendirilen isim tilelların oluşturduğu bölge olmaktadır.bu methodu sadece tilellar için kullanılmaktadır.
    {
        return ((x >= 0 && x < width) && (y >= 0 && y < height) && (z >= 0 && z < depth));//burada width,height ve depthin sınırları içerisinde olup olmadığı kontrol ediliyor.
    }

    public bool IsWithinBoundsforObjects(int x, int y, int z)//bunun yukarıdakinden farkı objelar için kullanılmaktadır. bu boolen amacı ise sınır bölgelerinde obje oluşturulmamasından dolayıdır.Dolayısıyla bu fonksiyon çağırıldığında y değeri otomatik olarak 0 olmaktadır.Yani aslında sadece x ve z değerlendiriliyor.
    {
        return ((x > 0 && x < width) && (y >= 0 && y < height) && (z > 0 && z < depth));
    }

    public bool IsInDangerRangeOfWall(float x, float y, float z)//bu en öenmli boolen oluşturulacak objenin duvarın içinde olup olmadığını kontrol ediyor.
    {
        dangerousWalls = new List<StartingWalls>();

        /*
         * burada öncelikle oluşturulan bütün duvarların positionları wallPosition variable ı içerisinde tutulmakta ve bu foreach loopuyla sürekli değişmektedir.
         * objectPosition ise spawnını kontrol etmek istediğimiz objenin positionları ve burada objenin y değeri sabit olarak 2 alınır çünkü duvarın y değeri sabit ve 2 dir.
         * ardından duvar üzerinde onun açısına göre iki adet nokta alıyorum bunlar uç noktalarıdır. ve açıya göre değişmektedir.
         * yani duvarın açısı 180 ise duvarın uç noktaları x değerinin -3 ve +3 ü olmaktadir. 90 derece ise z değerinin -3 ve +3 değerlerinin alınmasıyla oluşur. tabi farklı açılı bir duvar varsada işin içine con ve sin giriyor o noktaları trigonometri ile hesaplıyor.
         * ve bu nokaları w1 ve w2 variablellarının içine atıyorum. ardından bu iki noktayla bir doğru denklemi oluşturuyorum. iki noktası bilinen doğru denklemiyle.
         * oluşturmuş olduğum bu doğru üzerinde bizim objemizin değerlerini yazıyorum x ve z değerlerini yani eğer bu değer 0 çıkıyorsa biliyorum ki oluşacak olan bu obje bu doğrunun üzerindedir.
         * ancak bu doğru sonsuz olduğu için harıtadaki duvarın doğrultusunda bütün bçlgeleri kapsamaması adına bir sınır değer oluşturdum oda distance eğer distance belirli bir değerde küçük veya büyük olursa oluşacak objenin duvarın üzerinde olduğunuz farz ediyor.
         * ve o duvar tehlikeli duvarlar listesine ekleniyor en sondada o liste nin içinde en az bir eleman varsa o nokta tehlikeli nokta olarak değer kaZANIYOR;
         * sonuçta bu objenin positionları doğru üzerinde olsada distance 5 fazla veya -5 ten az ise o obje yine spawnlanıyor. 
        */
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
                        if (distance >= -5f && distance <= 5f) // işte tam bu aralıklar duvarın olduğu veya duvar için tehlikeli olan aralıklardır.
                        {
                            dangerousWalls.Add(wall);
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
                        if (distance >= -5f && distance <= 5f)// işte tam bu aralıklar duvarın olduğu veya duvar için tehlikeli olan aralıklardır.
                        {
                            dangerousWalls.Add(wall);
                        }
                    }
                }
                else
                {
                    w1 = new Vector3(wall.x - (3f * Mathf.Cos(Mathf.Deg2Rad * wall.angle)), 2, wall.z - (3f * Mathf.Sin(Mathf.Deg2Rad * wall.angle)));
                    w2 = new Vector3(wall.x + (3f * Mathf.Cos(Mathf.Deg2Rad * wall.angle)), 2, wall.z + (3f * Mathf.Sin(Mathf.Deg2Rad * wall.angle)));

                    value = ((x - w1.x) * (w1.z - w2.z)) - ((z - w1.z) * (w1.x - w2.x));

                    if (value >= -5f && value <= 5f) // burasıda açılı duvarlarda duvarın kalınlığından dolayı bazı tilelları kestiği için rahatsız edici bir görüntü oluşturmamak adına diğerlerinden farklı bir kontrol vardır.
                    {
                        if (distance >= -5f && distance <= 5f)// işte tam bu aralıklar duvarın olduğu veya duvar için tehlikeli olan aralıklardır.
                        {
                            dangerousWalls.Add(wall);
                        }
                    }
                }
            }
        }

        if(dangerousWalls.Count == 0)//burada listin dolu veya boş olduğunu kontrol edip sonuca göre return ediyorum.
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    #endregion
}
