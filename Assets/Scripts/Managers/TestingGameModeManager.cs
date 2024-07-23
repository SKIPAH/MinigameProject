using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingGameModeManager : MonoBehaviour
{
    public static TestingGameModeManager Instance { get; private set; }

    [SerializeField] Teleport teleport;
    [SerializeField] GameAsteroidUI gameAsteroid;
    [SerializeField] GameCoconutUI gameCoconutUI;
    [SerializeField] GameLionUI gameLionUI;
    [SerializeField] GameUIManager gameUIManager;

    private void Update()
    {

        
    }


    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

}
