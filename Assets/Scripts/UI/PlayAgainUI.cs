using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayAgainUI : MonoBehaviour
{
    [SerializeField] private Button playGameButton;
    [SerializeField] private Button quitGameButton;
    private void Awake()
    {
        playGameButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("MiniGame1");
        });

        quitGameButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });
    }
    public void PlayAgain()
    {
        SceneManager.LoadScene("MiniGame1");
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
