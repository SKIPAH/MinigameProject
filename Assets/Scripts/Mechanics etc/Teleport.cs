using UnityEngine;
using UnityEngine.Events;
public class Teleport : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform teleportDestination;
    public UnityEvent monkeyTeleported;
    public void Interact()
    {
        if (PlayerMonkey.Instance && teleportDestination)
        {
            PlayerMonkey.Instance.transform.position = teleportDestination.position;
            monkeyTeleported.Invoke();
        }
    }
    public void TeleportToNext(Transform location)
    {
        monkeyTeleported.Invoke();
        PlayerMonkey.Instance.transform.position = location.position;        
    }
}
