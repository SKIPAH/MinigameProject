using UnityEngine;

public class Asteroid : MonoBehaviour, IPlayerDied
{
    BoxCollider2D boxCollider2D;

    private void Start()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
        boxCollider2D.isTrigger = true;
    }
    private void Update()
    {
        transform.Rotate(0, 0, 2f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerMonkey>()){
            boxCollider2D.isTrigger = false;
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject, 3f);
        }
    }

    public void PlayerDied()
    {
        Debug.Log("Player died");
    }
}
