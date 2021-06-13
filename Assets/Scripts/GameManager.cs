using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("References")]
    public RaftQueue raftQueue;
    public RaftCommander raftCommander;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI pointText;
    public TextMeshProUGUI endScreenInstructions;

    [Header("Time")]
    public float gameDuration = 120f; //in seconds
    public int numDaysInGame = 3;

    [Header("Reloading")]
    public string mainSceneName = "Main";

    private float gameEndTime = 0;
    private float timePerDay;

    private int currentPoints;

    private TimeSpan timeSpan;

    private bool gameIsOver = false;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        timePerDay = gameDuration / numDaysInGame;

        StartGame();
    }

    void Update()
    {
        if (!gameIsOver)
        {
            float timeLeft = gameEndTime - Time.time;

            if (timerText != null)
            {
                int seconds = Mathf.RoundToInt(timeLeft) % 60;
                int minutes = Mathf.RoundToInt(timeLeft) / 60;
                timerText.text = minutes + ":" + seconds.ToString("00");
            }

            if (timeLeft <= 0)
            {
                EndGame();
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(mainSceneName);
            }
        }
        
    }

    public void StartGame()
    {
        gameEndTime = Time.time + gameDuration;
    }

    public void EndGame()
    {
        gameIsOver = true;

        raftQueue.enabled = false;
        raftCommander.enabled = false;

        endScreenInstructions.gameObject.SetActive(true);
    }

    public void AddPoints(int pointDelta)
    {
        currentPoints += pointDelta;

        pointText.text = currentPoints.ToString();
    }
}
