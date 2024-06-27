using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoconutThrow : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI coconutThrowText;
    [SerializeField] private GameObject coconutGameObject;

    [SerializeField] private Transform coconutHoldingPoint;




    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerMonkey>()){

            PlayerMonkey.Instance.ActivateCoconut();
            coconutGameObject.SetActive(false);

        }
    }

}
