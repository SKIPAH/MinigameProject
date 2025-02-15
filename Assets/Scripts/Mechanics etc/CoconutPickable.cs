using UnityEngine;

public class CoconutPickable : MonoBehaviour
{
    [SerializeField] private GameObject coconutGameObject;
    private Transform coconutStartingPosition;



    private void Start()
    {
        coconutStartingPosition = coconutGameObject.transform;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerMonkey>()){

            PlayerMonkey.Instance.ActivateCoconut();
            coconutGameObject.SetActive(false);
        } 
    }

    public void OnEnable()
    {
        coconutGameObject.SetActive(true);
    }

    public void OnDisable()
    {
        coconutGameObject.SetActive(false);
    }

    public void ResetPosition()
    {
        coconutGameObject.transform.position = coconutStartingPosition.position;
    }

}
