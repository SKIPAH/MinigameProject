using System;
using UnityEngine;
using UnityEngine.UI;
public class GameAsteroidUI : MonoBehaviour
{
    public event EventHandler OnAsteroidGameFinished;
    [SerializeField] private Text roundTimeText;
    [SerializeField] private Text countdownTimerText;
    [SerializeField] private float gameTime;
    private float gameTimeUI;
    public float countdownTime;
    private bool IsAsteroidGameOn = false;
    private void Start()
    {
        MiniGameManager.Instance.OnCountdownStarted += MiniGameManager_OnCountdownStarted;
        MiniGameManager.Instance.OnGameStarted += MiniGameManager_OnAsteroidGameStarted;
        roundTimeText.text = string.Empty;
    }
    private void MiniGameManager_OnAsteroidGameStarted(object sender, System.EventArgs e)
    {
        IsAsteroidGameOn = true;
    }
    private void MiniGameManager_OnCountdownStarted(object sender, System.EventArgs e)
    {
        CountdownTimer();
    }
    private void Update()
    {
        GameTimer();
    }
    private void CountdownTimer()
    {
        countdownTime -= Time.deltaTime;
        if (countdownTimerText != null)
        {
            countdownTimerText.text = countdownTime.ToString("F1");
        }
        if (countdownTime <= 0f)
        {
            countdownTimerText.text = string.Empty;
            return;
        }
    }
    private void GameTimer()
    {
        if (IsAsteroidGameOn)
        {
            if (gameTime > 0f)
            {
                roundTimeText.text = gameTime.ToString("F1");
                gameTime -= Time.deltaTime;
            }
            if (gameTime <= 0f)
            {
                roundTimeText.text = "Done!";
                OnAsteroidGameFinished?.Invoke(this, EventArgs.Empty);
            }
        }
    }


    public void SkipGame()
    {
        gameTime = 0f;
    }





    public void Show()
    {
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
