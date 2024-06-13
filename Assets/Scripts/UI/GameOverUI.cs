using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameOverUI : MonoBehaviour
{
    [SerializeField] private Button tryAgainButton;
    [SerializeField] private PlayerMonkey player;
    private void Awake()
    {
        tryAgainButton.onClick.AddListener(() =>
        {
            MiniGameManager.Instance.TryAgain();
        });
    }
    private void Start()
    {
        player.OnPlayerDied += Player_OnPlayerDied;
        Hide();
    }
    private void Player_OnPlayerDied(object sender, System.EventArgs e)
    {
        Show();
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
