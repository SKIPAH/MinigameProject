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
        Hide();
    }

    private void Instance_OnCoconutThrown(object sender, System.EventArgs e)
    {
        Hide();
    }

    private void Instance_OnCoconutThrowModeOn(object sender, System.EventArgs e)
    {
        Show();
    }



    private void IncreasePower()
    {

        powerText.text = power.ToString();
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
