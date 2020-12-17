using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Game Parameters")]
    public int killedEnemy = 0;
    public float timer = 0;

    [Header("Texts")]
    public Text scoreText;
    public Text timerText;
    Text gameOverText;
    Text endGameScoreText;

    [Header("Panels")]
    public GameObject entryPanel;
    public GameObject pausePanel;
    public GameObject gameOverPanel;

    [Header("Controls")]
    public bool isStarted = false;
    public bool isEnded = false;
    public bool isDied = false;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        UpdateScoreText();

        entryPanel.SetActive(true);
    }

    void Update()
    {
        InputControls();

        if(isStarted)
        {
            timer += Time.deltaTime;

            int minutes = Mathf.FloorToInt(timer / 60F);

            int seconds = Mathf.FloorToInt(timer - minutes * 60);

            string niceTime = string.Format("{0:0}:{1:00}", minutes, seconds);

            timerText.text = niceTime;
        }

        FinishGame();
    }

    void InputControls()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (entryPanel.activeInHierarchy)
            {
                entryPanel.SetActive(false);

                isStarted = true;
            }
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            if (pausePanel.activeInHierarchy)
            {
                isStarted = true;

                pausePanel.SetActive(false);

                Time.timeScale = 1;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Q))
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
        else if (Input.GetKeyDown(KeyCode.P))
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
        else if (isStarted)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                isStarted = false;

                pausePanel.SetActive(true);

                Time.timeScale = 0;
            }
        }
    }

    public void UpdateScoreText()
    {
        scoreText.text = killedEnemy.ToString();
    }

    void FinishGame()
    {
        Vector3 finalArea = ObjectPoolingManager.instance.smokeEffect.transform.position;

        float finalDistance = Vector3.Distance(finalArea, PlayerManager.instance.player.transform.position);

        if(finalDistance < 2)
        {
            Debug.Log("EndGame");

            isStarted = false;

            EndGameControl(timer, killedEnemy);
        }
    }

    public void EndGameControl(float time, int killedEnemyCount)
    {
        if(isDied)
        {
            gameOverPanel.SetActive(true);

            endGameScoreText = GameObject.Find("EndGameScoreText").GetComponent<Text>();

            gameOverText = GameObject.Find("GameOverText").GetComponent<Text>();

            gameOverText.text = "You are killed by enemies.";

            if (killedEnemyCount > 1)
            {
                endGameScoreText.text = "You killed " + killedEnemyCount + " enemies in " + Mathf.Floor(time) + " seconds";
            }
            else
            {
                endGameScoreText.text = "You killed " + killedEnemyCount + " enemy in " + Mathf.Floor(time) + " seconds";
            }
        }
        else
        {
            if (time <= 100)
            {
                gameOverPanel.SetActive(true);

                endGameScoreText = GameObject.Find("EndGameScoreText").GetComponent<Text>();

                gameOverText = GameObject.Find("GameOverText").GetComponent<Text>();

                gameOverText.text = "You killed enough enemy in a certain time.";

                if (killedEnemyCount > 1)
                {
                    endGameScoreText.text = "You killed " + killedEnemyCount + " enemies in " + Mathf.Floor(time) + " seconds";
                }
                else
                {
                    endGameScoreText.text = "You killed " + killedEnemyCount + " enemy in " + Mathf.Floor(time) + " seconds";
                }
            }
            else
            {
                gameOverPanel.SetActive(true);

                endGameScoreText = GameObject.Find("EndGameScoreText").GetComponent<Text>();

                gameOverText = GameObject.Find("GameOverText").GetComponent<Text>();

                gameOverText.text = "You killed enough enemy but did not in a certain time.";

                if (killedEnemyCount > 1)
                {
                    endGameScoreText.text = "You killed " + killedEnemyCount + " enemies in " + Mathf.Floor(time) + " seconds";
                }
                else
                {
                    endGameScoreText.text = "You killed " + killedEnemyCount + " enemy in " + Mathf.Floor(time) + " seconds";
                }
            }
        }
    }
}
