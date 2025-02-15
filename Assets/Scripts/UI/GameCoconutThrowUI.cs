using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameCoconutThrowUI : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI coconutThrowInstructions;
    [SerializeField] private Text powerText;
    [SerializeField] private float gameTime;
    [SerializeField] private Text gameTimeText;
    

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
        power += 0.1f;
        UpdatePowerText();
    }

    private void Instance_OnCoconutThrown(object sender, System.EventArgs e)
    {
        Hide();
        powerText.gameObject.SetActive(false);
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
        coconutThrowInstructions.text = "Throw the Coconut as far as you can!\r\nTap \"A\" and \"D\" to generate power \r\nand \"S\" to throw!";
    }

    public void Hide()
    {
        coconutThrowInstructions.gameObject.SetActive(false);
    }


}
