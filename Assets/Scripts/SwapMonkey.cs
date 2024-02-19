using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapMonkey : MonoBehaviour
{

    public static SwapMonkey Instance { get; private set; }
    [SerializeField] private PlayerMonkey2 monkey2;

    [SerializeField] private List<GameObject> playerMonkeys = new List<GameObject>();
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private Bananas bananas;
    private GameObject currentMonkey;

    private void Start()
    {
        Instance = this;

        if (playerMonkeys.Count > 0)
        {
            currentMonkey = playerMonkeys[0];
            foreach (GameObject monkey in playerMonkeys)
            {
                monkey.SetActive(false);
            }
            currentMonkey.SetActive(true);
            bananas.OnBananaHit += Bananas_OnBananaHit;
        }
        else
        {
            Debug.LogError("No player monkeys assigned to the list!");
        }
    }

    private void Monkey2_OnPlayer2Teleported(object sender, System.EventArgs e)
    {
        SwapMonkeyMode();
    }

    private void Bananas_OnBananaHit(object sender, System.EventArgs e)
    {
        SwapMonkeyMode();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            SwapMonkeyMode();
        }
    }

    public void SwapMonkeyMode()
    {
        int currentIndex = playerMonkeys.IndexOf(currentMonkey);
        int nextIndex = (currentIndex + 1) % playerMonkeys.Count;

        currentMonkey.SetActive(false);
        playerMonkeys[nextIndex].SetActive(true);
        currentMonkey = playerMonkeys[nextIndex];
        virtualCamera.Follow = currentMonkey.transform;

        Debug.Log("Switched to monkey " + (nextIndex + 1));
    }
}
