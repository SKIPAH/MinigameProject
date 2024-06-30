using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoconutThrowPlatform : MonoBehaviour
{


    private void OnCollisionEnter2D(Collision2D collision)
    {
       if(collision.gameObject.GetComponent<PlayerMonkey>() != null)
        {
            PlayerMonkey.Instance.CoconutThrowGameModeOn();
        }
    }
}
