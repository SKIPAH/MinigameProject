using UnityEngine;

public class CoconutThrow : MonoBehaviour
{
    [SerializeField] private GameObject coconutGameObject;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerMonkey>()){

            PlayerMonkey.Instance.ActivateCoconut();
            coconutGameObject.SetActive(false);
        }
    }

}
