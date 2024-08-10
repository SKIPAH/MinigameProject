using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoconutThrowable : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "coconutWall")
        {
            Debug.Log("LOL");
        }
    }
}
