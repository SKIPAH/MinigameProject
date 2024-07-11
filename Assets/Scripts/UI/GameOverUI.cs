using UnityEngine;
using UnityEngine.UI;
public class GameOverUI : MonoBehaviour
{
    [SerializeField] private Button tryAgainButton;
    private void Awake()
    {
        tryAgainButton.onClick.AddListener(() =>
        {
            MiniGameManager.Instance.TryAgain();
        });
    }
    private void Start()
    {
        PlayerMonkey.Instance.OnPlayerDied += Player_OnPlayerDied;
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
