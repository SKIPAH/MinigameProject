using System;
using UnityEngine;
using UnityEngine.UI;
public class GameLionUI : MonoBehaviour
{
    [SerializeField] private LionEnemy lionGame;
    [SerializeField] private float gameTime;
    [SerializeField] private Text gameTimeText;
    [SerializeField] private GameObject teleportToCoconut;
    public event EventHandler OnLionGameEnded;
    private bool isLionGameOn = false;
    private void Start()
    {
        Hide();
        lionGame.OnLionGameStarted += LionGame_OnLionGameStarted;
        teleportToCoconut.SetActive(false);
    }
    private void LionGame_OnLionGameStarted(object sender, System.EventArgs e)
    {
        Show();
        isLionGameOn = true;
    }
    private void Update()
    {
        GameTimer();
    }
    private void GameTimer()
    {
        if (isLionGameOn)
        {
            gameTime -= Time.deltaTime;
            if (gameTime >= 0f)
            {
                gameTimeText.text = gameTime.ToString("F1");
            }
            else
            {
                gameTimeText.text = "DONE";
                teleportToCoconut.SetActive(true);
                OnLionGameEnded?.Invoke(this, EventArgs.Empty);
            }
        }
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
