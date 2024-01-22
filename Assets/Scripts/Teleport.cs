using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform teleportDestination;
    

    public void Interact()
    {
        PlayerMonkey.Instance.transform.position = teleportDestination.position;
        PlayerMonkey2.Instance.transform.position = teleportDestination.position;
    }
}
