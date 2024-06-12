
using UnityEngine;
using UnityEngine.UI;

public class GameCoconutUI : MonoBehaviour
{
    
    [SerializeField] private Text roundTimeText;
    [SerializeField] private Text countdownTimerText;


    [SerializeField] private float gameTime = 10f;
    private float gameTimeUI = 15f;
    public float countdownTime = 3f;

    private bool isCoconutGameOn = false;


    private void Start()
    {
        roundTimeText.text = string.Empty;
        Hide();
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


    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
