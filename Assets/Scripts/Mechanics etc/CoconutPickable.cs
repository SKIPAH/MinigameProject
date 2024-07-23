using UnityEngine;

public class CoconutPickable : MonoBehaviour
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
