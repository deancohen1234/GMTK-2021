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
    public DaylightController daylightController;
    public AutoOrbitingCamera autoRotatingCamera;
    public RaftQueue raftQueue;
    public RaftCommander raftCommander;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI pointText;
    public TextMeshProUGUI endScreenInstructions;

    [Header("Time")]
    public float gameDuration = 120f; //in seconds

    [Header("Reloading")]
    public string mainSceneName = "Main";

    private float gameEndTime = 0;

    private int currentPoints;

    private TimeSpan timeSpan;

    private bool gameIsOver = false;

    void Awake()
    {
        instance = this;
        autoRotatingCamera.enabled = false;
    }

    void Start()
    {
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

            float percentageOfDayLeft = (gameDuration - timeLeft) / gameDuration;
            daylightController.UpdateDaylight(percentageOfDayLeft);
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
        autoRotatingCamera.enabled = true;

        PlayAllFireworks();

        endScreenInstructions.gameObject.SetActive(true);
    }

    public void AddPoints(int pointDelta)
    {
        currentPoints += pointDelta;

        pointText.text = currentPoints.ToString();
    }

    private void PlayAllFireworks()
    {
        Raft[] allRafts = FindObjectsOfType<Raft>();

        for (int i = 0; i < allRafts.Length; i++)
        {
            if (allRafts[i].raftType == RaftType.Lodging)
            {
                //set looping
                ParticleSystem.MainModule main = allRafts[i].fireworks.main;
                main.loop = true;

                if (allRafts[i].fullySatisfied)
                {
                    allRafts[i].fireworks.Play();
                }
            }

        }
    }
}
