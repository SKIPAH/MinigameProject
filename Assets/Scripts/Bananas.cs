using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bananas : MonoBehaviour
{
    public event EventHandler OnBananaHit;

    [SerializeField] private GameObject groundAndTeleport;
    [SerializeField] private GameObject player;

    void Update()
    {
        transform.Rotate(0, 0.5f, 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerMonkey>())
        {
            groundAndTeleport.gameObject.SetActive(true);
            OnBananaHit?.Invoke(this, EventArgs.Empty);

        }
         
    }
}
