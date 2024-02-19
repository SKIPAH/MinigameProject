using System.Collections;
using System.Collections.Generic;
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
        PlayerMonkey2.Instance.OnPlayer2Teleported += Instance_OnPlayer2Teleported;
        roundTimeText.text = string.Empty;
        Hide();
    }

    private void Instance_OnPlayer2Teleported(object sender, System.EventArgs e)
    {
        Show();
        isCoconutGameOn = true;
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


    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
