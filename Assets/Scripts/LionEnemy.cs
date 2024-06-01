using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class LionEnemy : MonoBehaviour, IPlayerDied
{
    [SerializeField] private Bananas bananas;

    public event EventHandler OnLionGameStarted;

    private float changeDirectionTimer = 0f;
    private float changeDirectionTimerMax = 0.8f;
    private float lionSpeed = 4.0f;

   
    private Transform lionTransform;
    [SerializeField] private Transform playerMonkey;

    private int randomDirectionNumber;
   

    private bool isGameOn = false;


    private void Start()
    {
        isGameOn = false;
        lionTransform = gameObject.transform;
        bananas.OnBananaHit += Bananas_OnBananaHit;
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
    }

    private void MovementEnemy()
    {
        changeDirectionTimer += Time.deltaTime;

        if(changeDirectionTimer >= changeDirectionTimerMax)
        {
            randomDirectionNumber = UnityEngine.Random.Range(0, 5);
            changeDirectionTimer = 0f;
        } 

        if (randomDirectionNumber == 0)
        {
            lionTransform.Translate(lionSpeed * Time.deltaTime * Vector3.right);      
        }
        if (randomDirectionNumber == 1)
        {
            lionTransform.Translate(lionSpeed * Time.deltaTime * Vector3.left);
        }
        if (randomDirectionNumber == 2)
        {
            lionTransform.Translate(lionSpeed * Time.deltaTime * Vector3.up);
        }
        if (randomDirectionNumber == 3)
        {
            lionTransform.Translate(lionSpeed * Time.deltaTime * Vector3.down);
        }
        if(randomDirectionNumber == 4)
        {
            if(playerMonkey != null)
            {
                lionTransform.position = (Vector2.MoveTowards(lionTransform.position, playerMonkey.position, lionSpeed * Time.deltaTime));
            }
            
        }
    }


    public void PlayerDied()
    {
        Debug.Log("LOL");
    }

}
