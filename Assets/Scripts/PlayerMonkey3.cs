using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMonkey3 : MonoBehaviour
{

    [SerializeField] private Transform coconutFull;
    [SerializeField] private Transform coconutCut;

    private int coconutsCutMax = 50;
    private int coconutsCutten = 0;


    private void Update()
    {
        CutCoconut();
    }

    private void CutCoconut()
    {
        if(Input.GetKeyDown(KeyCode.Space)) 
        {
            if(coconutsCutten < coconutsCutMax )
            {
                Transform coconut = Instantiate(coconutCut, coconutFull.GetComponent<Transform>().position, coconutFull.GetComponent<Transform>().rotation);
                coconutsCutten++;
            }
            else
            {
                Debug.Log("Enough");
            }
            
        }
    }


}
