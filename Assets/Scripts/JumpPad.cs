using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    [SerializeField] private float bouncePower = 50f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerMonkey>())
        {
            collision.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            collision.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.up * bouncePower, ForceMode2D.Impulse);
        }
    }
}
