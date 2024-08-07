using System;
using UnityEngine;
public class LionEnemy : MonoBehaviour, IPlayerDied
{
    [SerializeField] private Bananas bananas;
    [SerializeField] private GameLionUI gameLionUI;
    [SerializeField] private Transform playerMonkey;
    public event EventHandler OnLionGameStarted;

    private float changeDirectionTimer = 0f;
    private float changeDirectionTimerMax = 0.5f;
    private float lionSpeed = 6.0f;
    private Transform lionTransform;
    private int randomDirectionNumber;
    private bool isGameOn;
    private void Start()
    {
        isGameOn = false;
        lionTransform = gameObject.transform;
        bananas.OnBananaHit += Bananas_OnBananaHit;
        gameLionUI.OnLionGameEnded += GameLionUI_OnLionGameEnded;
    }
    private void GameLionUI_OnLionGameEnded(object sender, EventArgs e)
    {
        isGameOn = false;
        gameObject.SetActive(false);
    }
    private void Bananas_OnBananaHit(object sender, System.EventArgs e)
    {
        isGameOn = true;
        OnLionGameStarted?.Invoke(this, EventArgs.Empty);
    }
    private void Update()
    {
        if (isGameOn)
        {
            MovementEnemy();
        }
        else return;
    }
    private void MovementEnemy()
    {
        changeDirectionTimer += Time.deltaTime;
        if (changeDirectionTimer >= changeDirectionTimerMax)
        {
            randomDirectionNumber = UnityEngine.Random.Range(0, 7);
            changeDirectionTimer = 0f;
        }
        switch (randomDirectionNumber)
        {
            case 0:
                lionTransform.Translate(lionSpeed * Time.deltaTime * Vector3.right);
                break;
            case 1:
                lionTransform.Translate(lionSpeed * Time.deltaTime * Vector3.left);
                break;
            case 2:
                lionTransform.Translate(lionSpeed * Time.deltaTime * Vector3.up);
                break;
            case 3:
                lionTransform.Translate(lionSpeed * Time.deltaTime * Vector3.down);
                break;
            default:
                if (playerMonkey != null)
                {
                    lionTransform.position = Vector2.MoveTowards(lionTransform.position, playerMonkey.position, lionSpeed * Time.deltaTime);
                }
                break;
        }
    }

}
