using System;
using UnityEngine;
public class Bananas : MonoBehaviour
{
    public event EventHandler OnBananaHit;
    [SerializeField] private GameObject groundAndTeleport;
    [SerializeField] private Transform liongamePos;
    void Update()
    {
        transform.Rotate(0, 0.5f, 0);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerMonkey>())
        {
            LionGameMode();
            PlayerMonkey.Instance.ResetMonkeyRotation();
        }
    }



    private void LionGameMode()
    {
        groundAndTeleport.gameObject.SetActive(true);
        OnBananaHit?.Invoke(this, EventArgs.Empty);
        PlayerMonkey.Instance.transform.position = liongamePos.transform.position;
        PlayerMonkey.Instance.ChangeGravityMode();
        PlayerMonkey.Instance.ChangeMovementMode();
        CameraManager.Instance.ChangeCameraProjectionSizeLionGame();
    }
}
