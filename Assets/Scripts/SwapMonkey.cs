using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapMonkey : MonoBehaviour
{
    [SerializeField] private List<GameObject> playerMonkeys = new List<GameObject>();
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private Bananas bananas;
    private GameObject currentMonkey;

    private void Start()
    {
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

    private void SwapMonkeyMode()
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

    /* OLD CODE MADE CHATGPT DO THIS BETTR ONE
   

public class SwapMonkey : MonoBehaviour
{
    [SerializeField] private GameObject playerMonkey1;
    [SerializeField] private GameObject playerMonkey2;
    [SerializeField] private GameObject playerMonkey3;

    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private Bananas bananas;
    private GameObject currentMonkey;



    private void Start()
    {
        currentMonkey = playerMonkey1;
        playerMonkey1.SetActive(true);
        playerMonkey2.SetActive(false);
        playerMonkey3.SetActive(false);
        bananas.OnBananaHit += Bananas_OnBananaHit;


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


    private void SwapMonkeyMode()
    {


        if (currentMonkey == playerMonkey1)
        {
            playerMonkey1.SetActive(false);
            playerMonkey2.SetActive(true);
            currentMonkey = playerMonkey2;
            virtualCamera.Follow = playerMonkey2.transform;
            Debug.Log("lol");
        }
        else if (currentMonkey == playerMonkey2)
        {
            playerMonkey2.SetActive(false);
            playerMonkey3.SetActive(true);
            currentMonkey = playerMonkey3;
            virtualCamera.Follow = playerMonkey3.transform;
            Debug.Log("xd");
        }
        else
        {
            playerMonkey3.SetActive(false);
            playerMonkey1.SetActive(true);
            currentMonkey = playerMonkey1;
            virtualCamera.Follow = playerMonkey1.transform;
            Debug.Log("loler");
        }
    }

}

}
    */
