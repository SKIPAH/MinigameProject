using UnityEngine;

public class MoveCubeTest : MonoBehaviour
{

    private float movementSpeed = 0.1f;

    private Rigidbody2D rb2D;

    private void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }
    private void FixedUpdate()
    {
        rb2D.AddForce(Vector2.right * movementSpeed, ForceMode2D.Impulse);


          //  collision.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
      //  collision.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.up * bouncePower, ForceMode2D.Impulse);
    }
}
