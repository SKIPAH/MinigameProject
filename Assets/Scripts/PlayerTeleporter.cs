using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTeleporter : MonoBehaviour
{
    private GameObject currentTeleporter;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
          //  transform.position = currentTeleporter.GetComponent<Teleport>().GetDestination().position;
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Teleport>())
        {
            currentTeleporter = collision.gameObject;
            
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Teleport>())
        {
            if(collision.gameObject == currentTeleporter)
            {
                currentTeleporter = null;
            }
        }
    }
}
