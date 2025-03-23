using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameCoconutThrowUI : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI coconutThrowInstructions;
    [SerializeField] private Text powerText;
    [SerializeField] private float gameTime = 10f;
    [SerializeField] private float powerToThrow = 12.0f;
    [SerializeField] private Text gameTimeText;
    private bool isGameOn = false;
    private bool isGameCompleted = false;   


    private float power = 0f;

    void Start()
    {
        PlayerMonkey.Instance.OnCoconutThrowModeOn += Instance_OnCoconutThrowModeOn;
        PlayerMonkey.Instance.OnCoconutThrown += Instance_OnCoconutThrown;
        PlayerMonkey.Instance.OnPowerIncreased += Instance_OnPowerIncreased;
        PlayerMonkey.Instance.OnCoconutHitGeorge += Instance_OnCoconutHitGeorge;
        PlayerMonkey.Instance.OnCoconutDidNotHitGeorge += Instance_OnCoconutDidNotHitGeorge;
       
        Hide();
    }

    private void Update()
    {
        if (!isGameCompleted) { 
        GameTimer();
        }
    }

    private void GameTimer()
    {
        if (isGameOn)
        {
            if (gameTime > 0f)
            {
                gameTimeText.text = gameTime.ToString("F1");
                gameTime -= Time.deltaTime;
            }
            if (gameTime < 0f)
            {
                gameTimeText.text = "Too late. Game restarts soon";
                FunctionTimer.Create(() => SceneManager.LoadScene("MiniGame1"), 5f);
            }
        }

    }

    private void Instance_OnCoconutDidNotHitGeorge(object sender, System.EventArgs e)
    {
        Show();
        coconutThrowInstructions.text = "Not Far Enough. Try Again!";
    }

    private void Instance_OnCoconutHitGeorge(object sender, System.EventArgs e)
    {
        Show();
        coconutThrowInstructions.text = "Coconut hit george ebin. Get the trophy you have earned it!";
       
    }

    private void Instance_OnPowerIncreased(object sender, System.EventArgs e)
    {
        isGameOn = true;
        power += 0.1f;
        UpdatePowerText();
    }

    private void Instance_OnCoconutThrown(object sender, System.EventArgs e)
    {
        Hide();
        powerText.gameObject.SetActive(false);
        isGameCompleted = true;
    }

    private void Instance_OnCoconutThrowModeOn(object sender, System.EventArgs e)
    {
        Show();
        powerText.gameObject.SetActive(true); 
    }
    private void UpdatePowerText()
    {
        powerText.text = power.ToString("F2");
    }
    public void Show()
    {
        coconutThrowInstructions.gameObject.SetActive(true);
        coconutThrowInstructions.text = "Throw the Coconut as far as you can!\r\nTap \"A\" and \"D\" to generate power \r\nand \"S\" to throw! Timer starts when you press A or D";
    }

    public void Hide()
    {
        coconutThrowInstructions.gameObject.SetActive(false);
    }


}
