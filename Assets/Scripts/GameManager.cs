using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Singleton 
    public static GameManager instance;

    private void Awake()
    {
        instance = this;
    }
    #endregion

    [Header("Game Parameters")]
    public int killedEnemy = 0;
    public float timer = 0;

    [Header("Texts")]
    public Text scoreText;
    public Text timerText;
    Text gameOverText;
    Text endGameScoreText;
    string niceTime;

    [Header("Panels")]
    public GameObject entryPanel;
    public GameObject pausePanel;
    public GameObject gameOverPanel;

    [Header("Controls")]
    public bool isStarted = false;
    public bool isEnded = false;
    public bool isDied = false;

    void Start()
    {
        UpdateScoreText();

        if(entryPanel != null)
            entryPanel.SetActive(true);
    }

    void Update()
    {
        InputControls();

        UpdateTimer();

        if(killedEnemy > ObjectPoolingManager.instance.enemyCount + 10)
        {
            FinishGame();
        }
    }

    void InputControls()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(entryPanel != null)
            {
                if (entryPanel.activeInHierarchy)
                {
                    entryPanel.SetActive(false);

                    isStarted = true;
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.C))
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
        else if (Input.GetKeyDown(KeyCode.Q))
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
        else if (Input.GetKeyDown(KeyCode.P))
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
            if (Input.GetKeyDown(KeyCode.Escape))
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
        ObjectPoolingManager.instance.InstantiateSmokeEffect();

        Vector3 finalArea = ObjectPoolingManager.instance.smokeEffect.transform.position;

        float finalDistance = Vector3.Distance(finalArea, PlayerManager.instance.player.transform.position);

        if(finalDistance < 2)
        {
            Debug.Log("EndGame");

            isStarted = false;

            EndGameControl(timer, killedEnemy);
        }
    }

    void UpdateTimer()
    {
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

    public void UpdateScoreText()
    {
        if(scoreText != null)
            scoreText.text = killedEnemy.ToString();
    }
}
