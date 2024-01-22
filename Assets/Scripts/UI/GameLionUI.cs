using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameLionUI : MonoBehaviour
{
    [SerializeField] private LionEnemy lionGame;
    private float gameTime = 10f;
    [SerializeField] private Text gameTimeText;
    [SerializeField] private GameObject teleportToCoconut;

    private bool isLionGameOn = false;



    private void Start()
    {
        Hide();
        lionGame.OnLionGameStarted += LionGame_OnLionGameStarted;
      //  teleportToCoconut.SetActive(false);
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
        if(isLionGameOn)
        {
            gameTime -= Time.deltaTime;
            if(gameTime >= 0f)
            {
                gameTimeText.text = gameTime.ToString("F1");
            }
            else
            {
                gameTimeText.text = "DONE";
                teleportToCoconut.SetActive(true);
            }
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
