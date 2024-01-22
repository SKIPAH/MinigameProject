using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MiniGameManager : MonoBehaviour
{
    [SerializeField] private PlayerMonkey player;
    [SerializeField] private GameAsteroidUI gameUI;
    public static MiniGameManager Instance { get; private set; }


    public event EventHandler OnGameStarted;
    public event EventHandler OnCountdownStarted;


    private enum State
    {
        GameStart,
        GameOver,
        Countdown,
        WaitingForTeleport,
        TestState
    }

    private State gameState;


    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        player.OnPlayerDied += Player_OnPlayerDied;
        //   gameState = State.Countdown;
        gameState = State.TestState;
    }

    private void Player_OnPlayerDied(object sender, EventArgs e)
    {
        gameState = State.GameOver;
    }

    private void Update()
    {
       // Debug.Log(gameState);
        switch (gameState)
        {
            case State.Countdown:
                Time.timeScale = 1.0f;
                OnCountdownStarted?.Invoke(this, EventArgs.Empty);
                if(gameUI.countdownTime <= 0f)
                {
                    gameState = State.GameStart;
                    OnGameStarted?.Invoke(this, EventArgs.Empty);
                }
                break;

            case State.GameStart:
                
                break;

            case State.GameOver:
                Time.timeScale = 0f;
                break;
            case State.WaitingForTeleport:
                break;
            case State.TestState:
                break;

        }
    }

    public bool IsCountdown()
    {
        return gameState == State.Countdown;
    }
    public bool IsAsteroidGame()
    {
        return gameState == State.GameStart;
    }

    public bool IsGameOver()
    {
        return gameState == State.GameOver;
    }


    public void TryAgain()
    {
        SceneManager.LoadScene("MiniGame1");
        gameState = State.Countdown;
    }



}
