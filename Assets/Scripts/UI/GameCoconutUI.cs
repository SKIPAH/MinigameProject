using UnityEngine;
using UnityEngine.UI;
public class GameCoconutUI : MonoBehaviour
{
    [SerializeField] private Text roundTimeText;
    [SerializeField] private Text countdownTimerText;
    [SerializeField] private Text mashText;
    [SerializeField] private float gameTime = 10f;
    public float countdownTime = 3f;
    private bool isCoconutGameOn = false;
    private void Start()
    {
        PlayerMonkey.Instance.OnCoconutGameModeOn += Instance_OnCoconutGameModeOn;
        PlayerMonkey.Instance.OnCoconutGameDone += Instance_OnCoconutGameDone;
        roundTimeText.text = string.Empty;

        Hide();
    }

    private void Instance_OnCoconutGameDone(object sender, System.EventArgs e)
    {
        roundTimeText.text = "Done";
        mashText.text = string.Empty;
        gameTime = 0f;
        countdownTime = 0f;
        isCoconutGameOn = false;
        FunctionTimer.Create(() => roundTimeText.text = string.Empty, 3f);
    }

    private void Instance_OnCoconutGameModeOn(object sender, System.EventArgs e)
    {
        Show();
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
            mashText.text = "MASH!";
            isCoconutGameOn = true;
            PlayerMonkey.Instance.CanCutCoconut();
        }
    }
    private void GameTimer()
    {
        if (isCoconutGameOn)
        {
            if (gameTime > 0f)
            {
                roundTimeText.text = gameTime.ToString("F1");
                gameTime -= Time.deltaTime;
            }
            if (gameTime < 0f)
            {
                roundTimeText.text = "Done";

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
