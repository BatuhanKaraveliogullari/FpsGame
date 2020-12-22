using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public float lookRadius = 10f;//enemynin görüş alanını ifade eden bir variable

    public Transform targetPlayer;//first person shooter takip etmek adına

    public List<Transform> walls;//enemynin oyun içerisinde canlıyken lookradius içerisindeki playerı görüşünün engelleyecek bütün duvarlar.
    public List<Transform> currentwalls;//enemynin oyun içerisindeki o andaki player la arasında olan bütün duvarlardır.

    public bool enemyDetected = false;//enemy farkedildiğinin kontrol eden boolen
    public bool playerInvisible = false;//enemynin invisible olduğunu kontrol eden boolen

    void Start()
    {
        targetPlayer = GameObject.Find("FirstPersonPlayer").GetComponent<Transform>();

        SetWalls();
    }

    void Update()
    {
        AIManager();
    }

    public void SetWalls()//enemy oyun başlangıcında veya ölüp dirildiğinde çalışan method. lookradius içerisindeki bütün duvarları kontrol eder ve liste ekler.
    {
        walls.Clear();

        if(Map.instance != null)
        {
            for (int i = 0; i < Map.instance.walls.Length; i++)
            {
                float distanceOfWall = Vector3.Distance(new Vector3(Map.instance.walls[i].x, Map.instance.walls[i].y, Map.instance.walls[i].z), transform.position);

                if (distanceOfWall < lookRadius)
                {
                    Transform wall = GameObject.Find("Wall (" + i + ")").GetComponent<Transform>();//bu rada lookradius içerisinde bulunan duvarın transform değerleriyle uğraşacağım için transform obje olarak atama yaptım.

                    walls.Add(wall);
                }
            }
        }
    }

    void FaceTarget()//enemynin player lookradiusuna girdiği anda yüzünü çevirmesinin sağlayan method.
    {
        Vector3 direction = (targetPlayer.position - transform.position).normalized;

        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
    }

    void AIManager()//düşman yapay zekasının kontrol edildiği methodur.updatette çağırıldığı için her fonksiyona girişte playerinvisible false değer alır.
    {
        playerInvisible = false;

        if(GameManager.instance.isStarted)
        {
            AreaControl(targetPlayer.position.x, targetPlayer.position.z, WhichWallIsBetweenEnemyAndPlayer());//öcelikle WhichWallIsBetweenEnemyAndPlayer methodunu inceleyin sonrada areacontrolu

            float distance = Vector3.Distance(targetPlayer.position, transform.position);//düşman ile enemy arasındaki fark alınır bu fark lookradiustan küçükse ve birde player visible ise ozaman enemy playerı algılar.

            if (distance <= lookRadius && !playerInvisible)
            {
                FaceTarget();

                enemyDetected = true;//bu boolenı kullnamamın sebebi enemynin ateş edebilmesi içindir.
            }
            else
            {
                enemyDetected = false;
            }
        }
    }

    void AreaControl(float x, float z, List<Transform> walls)
    {
        //burada yapmaya çalıştığım şeyi görsellik katarak anlatmak istediğim için attığım dökümanda ulaşabilirsin öncelikle oraya bakmalısın.

        bool underFirstLine; //documentasyondaki E,W1 doğrusunun kontrolünün sağlayan boolen
        bool underSecondLine; //documentasyondaki E,W2 doğrusunun kontrolünün sağlayan boolen
        bool underThirdLine; //documentasyondaki W1,W2 doğrusunun kontrolünü sağlayan boolen

        Vector3 w1;//walla ait doğruyu oluşturmak adına oluşturulmuş küçük(konumsal) noktadır.
        Vector3 w2;//walla ait doğruyu oluşturmak adına oluşturulmuş büyük(konumsal) noktadır.

        float value1;//documentasyonda açıklandığı gibi playerın E,W1 doğrusuna göre konumu veren eşitsizlik değeridir.
        float value2;//documentasyonda açıklandığı gibi playerın E,W2 doğrusuna göre konumu veren eşitsizlik değeridir.
        float value3;//documentasyonda açıklandığı gibi playerın W1,W2 doğrusuna göre konumu veren eşitsizlik değeridir.
        float value4;//documentasyonda açıklandığı gibi enemynin W1,W2 doğrusuna göre konumu veren eşitsizlik değeridir.

        foreach (Transform wall in walls)
        {
            if(wall != null)
            {
                if (wall.GetComponent<Wall>().angle == 0 || wall.GetComponent<Wall>().angle == 180)//documentasyondaki örnektir.
                {
                    w1 = new Vector3(wall.position.x - 3f, 2, wall.position.z);//wall 0 veya 180 derecede yatay olduğu için ancak x ekseninde boyutu vardır.
                    w2 = new Vector3(wall.position.x + 3f, 2, wall.position.z);//bu yüzden sadece x değerleri değiştirilerek iki nokta bulunur.

                    value1 = ((x - transform.position.x) * (transform.position.z - w1.z)) - ((z - transform.position.z) * (transform.position.x - w1.x));
                    value2 = ((x - transform.position.x) * (transform.position.z - w2.z)) - ((z - transform.position.z) * (transform.position.x - w2.x));
                    value3 = ((x - w1.x) * (w1.z - w2.z)) - ((z - w1.z) * (w1.x - w2.x));
                    value4 = ((transform.position.x - w1.x) * (w1.z - w2.z)) - ((transform.position.z - w1.z) * (w1.x - w2.x));

                    if (value4 > 0)//öncelikli olarak value4 heasplanır çünkü enemynin duvarın konumuna göre diğer eşitsizlikler yön değiştirmektedir.Bu demektir ki enemy wall un üzerindedir.
                    {
                        if (value1 > 0)//bu demektir ki player E,W1 doğrusunun üzerindedir.
                            underFirstLine = true;
                        else
                            underFirstLine = false;
                        if (value2 < 0)//bu demektir ki player E,W2 doğrusunun altındadır.
                            underSecondLine = true;
                        else
                            underSecondLine = false;
                        if (value3 < 0)//bu demektir ki player W1,W2 doğrusunun yani dolayısıyla wallun altında kalmaktadır. zaten value3 "-"ise value4 "+" olmak zorundadır yoksa wall enemy ve player arasında değildir.
                            underThirdLine = true;
                        else
                            underThirdLine = false;
                    }
                    else//bu state ise enemy eğer wallun altındaysa gerçekleşir.
                    {
                        if (value1 < 0)//bu demektir ki player wallun altındadır.
                            underFirstLine = true;
                        else
                            underFirstLine = false;
                        if (value2 > 0)//bu demektir ki player wallun üstündedir.
                            underSecondLine = true;
                        else
                            underSecondLine = false;
                        if (value3 > 0)//bu demektir ki player wallun üstündedir.zaten yukarıdada dediğim gibi bu değer pozitifse value 4 negatif olmak zorundadır.
                            underThirdLine = true;
                        else
                            underThirdLine = false;
                    }
                }
                else if (wall.GetComponent<Wall>().angle == 90)//burada değişen tek şey duvarın açısıdır. ve haliyle w1,w2 noktalarının alacağı değer değişmektedir.
                {
                    w1 = new Vector3(wall.position.x, 2, wall.position.z - 3f);//wall 90 derecede dikey olduğu için ancak z ekseninde boyutu vardır.
                    w2 = new Vector3(wall.position.x, 2, wall.position.z + 3f);//bu yüzden sadece z değerleri değiştirilerek iki nokta bulunur.

                    value1 = ((x - transform.position.x) * (transform.position.z - w1.z)) - ((z - transform.position.z) * (transform.position.x - w1.x));
                    value2 = ((x - transform.position.x) * (transform.position.z - w2.z)) - ((z - transform.position.z) * (transform.position.x - w2.x));
                    value3 = ((x - w1.x) * (w1.z - w2.z)) - ((z - w1.z) * (w1.x - w2.x));
                    value4 = ((transform.position.x - w1.x) * (w1.z - w2.z)) - ((transform.position.z - w1.z) * (w1.x - w2.x));

                    if (value4 > 0) // burası bütün açılar için aynıdır zaten yukarıda açıklandığı gibidir.
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
                else//bu state de ise duvarın açılı olduğu durumlar hesaplanır tabi wall bu state de iki eksendede boyutu olduğu için açı yardımıyla w1,w2 noktaları hesaplanır.
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

                if (underFirstLine && underSecondLine && underThirdLine)//son olarak burada ise documentasyonda gösterdiğim gibi 3 stateinde belirli bir bölgeyi göstermesi şartımız olduğu için üçününde true olduğu yani playerin invisible zone içerisinde kaldığı kontrol edilir. 
                {
                    Debug.Log("Player ınvisible from" + transform.name + "because of" + wall.name);

                    Debug.Log(wall.transform.position);

                    playerInvisible = true;//player eğer invisible zoneda ise ozaman bu boolen true döner.

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
        /*
         * burada yapmaya çalıştığım şey kabaca bir liste oluşturmaktır. öncelikle setwall methoduyla birlikte enemynin lookradiusu içerisinde kalan bütün walllar bir list içerisinde toplanır.
         * ardında tıpki map classında yapıldığı gibi burada duvar uç nokataları duvarın açısal propertyleriyle birlikte belirlenir. mesela 0 ve 180 dereceyse mantıken x değerleri artıp azalarak belirlenir.
         * aynı şekilde 90 dereceyse bu sefer z değeri artıp azalrak belirlenir. ancak farklı bir açı varsa bu kez devreye sin con girmektedir.
         * ardından iki noktası bilinen doğru denkleminden bir doğru oluşturdum. 
         * value4 enemy nin o oluşturulan doğruya göre konumunu değerlendirmektedir. yani aşağıdaki value4 değeri karşısında iki noktası bilinin doğru denklemi kullanılıp bir eşitsizlik elde edilmiştir. 
         * value5 playerın oluşturulan doğruya göre konumunu vermektedir. value4 un aynısı ancak sadece value4te enemynin positiıon bilgileri kullanılırken value5te playerın position bilgileri kullanılmaktadır.
         * sonrasında logic oldukça basit aslında value4 un pozitif value5in negatif olduğu değerlerdeki oluşturulmuş doğru parçasını oluşturulan wall enemy ve player arasında kabul edilmektedir.
         * zaten doğrunun üsttarafı ve alt tarafı + veya - olarak farklı değerler vermektedir. yani value4 pozitif ise ve value5 negatif ise bu value4 doğrunun üst tarafında kaladığını ve value5 te doğrunun alt tarafında kaldığının kanıtıdır. 
         * dolayısıyle enemy yukarda player aşağıdadır. buda demek oluyor ki wall enemy ve playerın arasındadır. 
         * yukarıdaki işlemlerden geçen wall ise playerın anlık konumuna göre enemy ile player arasında olan duvarlar listinin içerisine eklenir. 
         */

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
