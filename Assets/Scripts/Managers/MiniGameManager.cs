using System;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MiniGameManager : MonoBehaviour
{
    public static MiniGameManager Instance { get; private set; }

    [SerializeField] private GameAsteroidUI gameUI;
    private bool isGameOver = false;
    
    public event EventHandler OnGameStarted;
    public event EventHandler OnCountdownStarted;
    private enum State
    {
        GameStart,
        GameOver,
        Countdown,
        WaitingForTeleport,
    }
    private State gameState;
    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    private void Start()
    {
        PlayerMonkey.Instance.OnPlayerDied += Player_OnPlayerDied;
        gameState = State.Countdown;     
    }
    private void Player_OnPlayerDied(object sender, EventArgs e)
    {
        gameState = State.GameOver;
    }
    private void Update()
    {
        ModeStateMachine();

        if (isGameOver)
        {
            if(Input.GetKeyUp(KeyCode.Space)) 
            {
                TryAgain();
            }
        } 
    }

    private void ModeStateMachine()
    {
        switch (gameState)
        {
            case State.Countdown:
                isGameOver = false;
                Time.timeScale = 1.0f;
                OnCountdownStarted?.Invoke(this, EventArgs.Empty);
                if (gameUI.countdownTime <= 0f)
                {
                    gameState = State.GameStart;
                    OnGameStarted?.Invoke(this, EventArgs.Empty);
                }
                break;
            case State.GameStart:
                isGameOver = false;
                break;
            case State.GameOver:
                isGameOver = true;
                Time.timeScale = 0f;
                break;
            case State.WaitingForTeleport:
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
