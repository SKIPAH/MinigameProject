using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoconutThrowable : MonoBehaviour
{
    [SerializeField] private GameObject coconutThrowableGameObject;
    [SerializeField] private George george;
    private Transform coconutStartingPosition;

    private void Start()
    {
        coconutStartingPosition = coconutThrowableGameObject.transform;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
       
        if (collision.gameObject.GetComponent<George>())
        {
            Debug.Log("Coconut hit George");
            PlayerMonkey.Instance.CoconutHitGeorge();
        }
    }

    public void ResetPosition()
    {
        coconutThrowableGameObject.transform.position = coconutStartingPosition.position;
    }



}
