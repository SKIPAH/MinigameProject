using UnityEngine;
using UnityEngine.UI;

public class GameCoconutThrowUI : MonoBehaviour
{

    [SerializeField] private Text coconutThrowInstructions;
    [SerializeField] private Text powerText;

    private float power = 0f;




    void Start()
    {
        PlayerMonkey.Instance.OnCoconutThrowModeOn += Instance_OnCoconutThrowModeOn;
        PlayerMonkey.Instance.OnCoconutThrown += Instance_OnCoconutThrown;
        PlayerMonkey.Instance.OnPowerIncreased += Instance_OnPowerIncreased;
        Hide();
    }

    private void Instance_OnPowerIncreased(object sender, System.EventArgs e)
    {
        power += 0.1f;
        UpdatePowerText();
    }

    private void Instance_OnCoconutThrown(object sender, System.EventArgs e)
    {
        Hide();
    }

    private void Instance_OnCoconutThrowModeOn(object sender, System.EventArgs e)
    {
        Show();
    }



    private void UpdatePowerText()
    {
        powerText.text = power.ToString("F2");
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
