using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Teleport : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform teleportDestination;

    [SerializeField] private GameObject monkey1;

    public UnityEvent monkeyTeleported;


    public void Interact()
    {
        if (monkey1.activeInHierarchy)
        {
            PlayerMonkey.Instance.transform.position = teleportDestination.position;
            monkeyTeleported.Invoke();
        }
    }
}
