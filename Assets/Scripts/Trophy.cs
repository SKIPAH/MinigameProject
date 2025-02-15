using UnityEngine;
using UnityEngine.SceneManagement;

public class Trophy : MonoBehaviour
{
    [SerializeField] private GameObject trophyGameObject;

    void Start()
    {
        trophyGameObject.SetActive(false);
        PlayerMonkey.Instance.OnCoconutHitGeorge += Instance_OnCoconutHitGeorge;
    }

    private void Instance_OnCoconutHitGeorge(object sender, System.EventArgs e)
    {
        trophyGameObject.SetActive(true);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerMonkey>())
        {
            SceneManager.LoadScene("EndingScene");
        }
    }
}
