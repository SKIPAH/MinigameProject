
using System;
using UnityEngine;

public class Coconut : MonoBehaviour
{

    [SerializeField] private GameObject coconut;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerMonkey3>())
        {
            PlayerMonkey3.Instance.CoconutThrowState();
        }

    }
}
