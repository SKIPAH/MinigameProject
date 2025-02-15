using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button playGameButton;
    [SerializeField] private Button instructionsButton;
    [SerializeField] private Button quitGameButton;
    [SerializeField] private Button closeInstructionsButton;

    [SerializeField] private GameObject instructionsUI;


    private void Awake()
    {
        playGameButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("MiniGame1");
        });


        instructionsButton.onClick.AddListener(() =>
        {
            ShowInstructionsUI();
        });

        quitGameButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });

        closeInstructionsButton.onClick.AddListener(() =>
        {
            HideInstructionsUI();
        });

        HideInstructionsUI();
    }


    private void ShowInstructionsUI()
    {
        instructionsUI.SetActive(true);
    }

    private void HideInstructionsUI()
    {
        instructionsUI.SetActive(false);
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
