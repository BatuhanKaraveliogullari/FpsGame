using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Singleton 
    public static GameManager instance;//hiyerarşide static obje olduğu için bir static örnek tanımı

    private void Awake()
    {
        instance = this;
    }
    #endregion

    [Header("Game Parameters")]
    public int killedEnemy = 0;//öldürülen enemy sayısı
    public float timer = 0;//oyun içerisinde geçen süre

    [Header("Texts")]
    public Text scoreText;//oyun ekranındaki score text
    public Text timerText;//oyun ekranındaki time text
    Text gameOverText;//oyun bitiminde description text
    Text endGameScoreText;//oyun bitimindeki score text
    string niceTime;//oyun içindeki time dakika ve saniye olarak yazdıkmak için null bir variable

    [Header("Panels")]
    public GameObject entryPanel;//oyun giriş panel
    public GameObject pausePanel;//oyun içi pause panel
    public GameObject gameOverPanel;//oyun bitim ekranı

    [Header("Controls")]
    public bool isStarted = false;//oyunun başlangıcını kontrol eden bool
    public bool isEnded = false;//oyunun normal bitimini kontrol eden bool
    public bool isDied = false;//oyunun bitimini ölümle kontrol eden bool

    [Header("Player")]
    public GameObject player;//first person shooter

    void Start()
    {
        UpdateScoreText();

        if(entryPanel != null)
            entryPanel.SetActive(true);//oyun başlangıcında açıklamayla birlikte oyunu açmak
    }

    void Update()
    {
        InputControls();

        UpdateTimer();

        if(killedEnemy > ObjectPoolingManager.instance.enemyCount + 10)//oyun kontrolü enemy sayısından 10 fazla adam öldürmeyle birlikte oyun bitirilebiliyor
        {
            FinishGame();
        }
    }

    void InputControls()//oyun içi panellerin kontrolünü sağlayan method
    {
        if (Input.GetKeyDown(KeyCode.Space))//oyun başlangıcını kontrol eden tuş ataması
        {
            if(entryPanel != null)
            {
                if (entryPanel.activeInHierarchy)
                {
                    entryPanel.SetActive(false);

                    isStarted = true; //burada oyun başladı demek
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.C))//c tuşu ancak ve ancak pause panel açıkken çalışır yanı oyun içindede basıldığında eğer pause panel açık değilse bu if blockunun içine giremeyecek ve  görevi oyunu devam ettirmek
        {
            if(pausePanel != null)
            {
                if (pausePanel.activeInHierarchy)
                {
                    isStarted = true;

                    pausePanel.SetActive(false);

                    Time.timeScale = 1;
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.Q))//q oyundan çıkmak için kullanılan bir tuştur, pause panel ve gameover panel açıksa çalışır ancak
        {
            if(pausePanel != null && gameOverPanel != null)
            {
                if (pausePanel.activeInHierarchy)
                {
                    Application.Quit();
                }
                else if (gameOverPanel.activeInHierarchy)
                {
                    Application.Quit();
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.P))//p play again tuşudur. oyun bitiminde ve durdurulduğunda çalışır. ve bu paneller açıldığında oyun durduğu içi aynı c tuşunda olduğu gibi oyunu devam ettirmek gerekir. ve tabi play again olduğu için scene tekrar yüklenir.
        {
            if (pausePanel != null && gameOverPanel != null)
            {
                if (gameOverPanel.activeInHierarchy)
                {
                    gameOverPanel.SetActive(false);
                }
                else if (pausePanel.activeInHierarchy)
                {
                    pausePanel.SetActive(false);
                }

                Time.timeScale = 1;

                SceneManager.LoadScene(0);
            }
        }
        else if (isStarted)
        {
            if (Input.GetKeyDown(KeyCode.Escape))//pause paneli açan tuştur. ve ancak oyun oynanıyorken çalışır ve oyunu durdurur.
            {
                if(pausePanel != null)
                {
                    isStarted = false;

                    pausePanel.SetActive(true);

                    Time.timeScale = 0;
                }
            }
        }
    }

    void FinishGame()
    {
        ObjectPoolingManager.instance.InstantiateSmokeEffect();//burada static bir obje örneği olan pooling sisteminden smoke komutunu çağırıyor ve final point oyuncuya hissettiriliyor.

        Vector3 finalArea = ObjectPoolingManager.instance.smokeEffect.transform.position;//burada oyun bitim bölgesinin positionı belirleniyor. o positionda smokenın positionı oluyor.

        float finalDistance = Vector3.Distance(finalArea, player.transform.position);//burada player ile finish positionunun farkı alınıyor.

        if(finalDistance < 2)//bu fark 2 den küçükse oyun bitmiş sayılıyor.
        {
            Debug.Log("EndGame");

            isStarted = false;

            EndGameControl(timer, killedEnemy);
        }
    }

    void UpdateTimer()
    {
        /*
         * burada süreyi kontrol edip dakika ve saniye cinsinden screene yazıyoruz. ve update te çağırıyoruz.
         */

        if (isStarted)
        {
            timer += Time.deltaTime;

            int minutes = Mathf.FloorToInt(timer / 60F);

            int seconds = Mathf.FloorToInt(timer - minutes * 60);

            niceTime = string.Format("{0:0}:{1:00}", minutes, seconds);

            if (timerText != null)
                    timerText.text = niceTime;
        }
    }

    public void EndGameControl(float time, int killedEnemyCount)
    {
        /*
         * burada ise ölüm ve normal bitiş olarak iki ayrı bitiş senaryosu vardır.
         * bu senaryolarda farklı olan ise sadece yazılan textlerdir.
         * enemy count un sayısına göre enemy mi yoksa enemies mi yazılacağın agöre if blocklarıyla ayrılmıştır. 
         * ve timeı kontrol eden yani belirli bir zaman içerisinde kalıbı tamamen örnektir o oyunun yapısına ve mapteki duvarlara göre değişiklik göstermektedir.
         */

        if(isDied)
        {
            if(gameOverPanel != null)   
                gameOverPanel.SetActive(true);

            endGameScoreText = GameObject.Find("EndGameScoreText").GetComponent<Text>();

            gameOverText = GameObject.Find("GameOverText").GetComponent<Text>();

            if(gameOverText != null)
                gameOverText.text = "You are killed by enemies.";

            if (killedEnemyCount > 1)
            {
                if(timer < 60f)
                {
                    if(endGameScoreText != null)
                        endGameScoreText.text = "You killed " + killedEnemyCount + " enemies in " + Mathf.Floor(timer) + " seconds";
                }
                else
                {
                    if(endGameScoreText != null)
                        endGameScoreText.text = "You killed " + killedEnemyCount + " enemies in " + niceTime;
                }
            }
            else
            {
                if (timer < 60f)
                {
                    if(endGameScoreText != null)
                        endGameScoreText.text = "You killed " + killedEnemyCount + " enemy in " + Mathf.Floor(timer) + " seconds";
                }
                else
                {
                    if(endGameScoreText != null)
                        endGameScoreText.text = "You killed " + killedEnemyCount + " enemy in " + niceTime;
                }
            }
        }
        else
        {
            if (time <= 100)
            {
                if (gameOverPanel != null)
                    gameOverPanel.SetActive(true);

                endGameScoreText = GameObject.Find("EndGameScoreText").GetComponent<Text>();

                gameOverText = GameObject.Find("GameOverText").GetComponent<Text>();

                if (gameOverText != null)
                    gameOverText.text = "You killed enough enemy in a certain time.";

                if (killedEnemyCount > 1)
                {
                    if (timer < 60f)
                    {
                        if (endGameScoreText != null)
                            endGameScoreText.text = "You killed " + killedEnemyCount + " enemies in " + Mathf.Floor(timer) + " seconds";
                    }
                    else
                    {
                        if (endGameScoreText != null)
                            endGameScoreText.text = "You killed " + killedEnemyCount + " enemies in " + niceTime;
                    }
                }
                else
                {
                    if (timer < 60f)
                    {
                        if (endGameScoreText != null)
                            endGameScoreText.text = "You killed " + killedEnemyCount + " enemy in " + Mathf.Floor(timer) + " seconds";
                    }
                    else
                    {
                        if (endGameScoreText != null)
                            endGameScoreText.text = "You killed " + killedEnemyCount + " enemy in " + niceTime;
                    }
                }
            }
            else
            {
                if (gameOverPanel != null)
                    gameOverPanel.SetActive(true);

                endGameScoreText = GameObject.Find("EndGameScoreText").GetComponent<Text>();

                gameOverText = GameObject.Find("GameOverText").GetComponent<Text>();

                if (gameOverText != null)
                    gameOverText.text = "You killed enough enemy but did not in a certain time.";

                if (killedEnemyCount > 1)
                {
                    if (timer < 60f)
                    {
                        if (endGameScoreText != null)
                            endGameScoreText.text = "You killed " + killedEnemyCount + " enemies in " + Mathf.Floor(timer) + " seconds";
                    }
                    else
                    {
                        if (endGameScoreText != null)
                            endGameScoreText.text = "You killed " + killedEnemyCount + " enemies in " + niceTime;
                    }
                }
                else
                {
                    if (timer < 60f)
                    {
                        if (endGameScoreText != null)
                            endGameScoreText.text = "You killed " + killedEnemyCount + " enemy in " + Mathf.Floor(timer) + " seconds";
                    }
                    else
                    {
                        if (endGameScoreText != null)
                            endGameScoreText.text = "You killed " + killedEnemyCount + " enemy in " + niceTime;
                    }
                }
            }
        }
    }

    public void UpdateScoreText()//burada score screendaki score texti yazdırmamıza yardımcı olan bir method var ve public yaptım diğer yerlerden çağırılsın diye.
    {
        if(scoreText != null)
            scoreText.text = killedEnemy.ToString();
    }
}
