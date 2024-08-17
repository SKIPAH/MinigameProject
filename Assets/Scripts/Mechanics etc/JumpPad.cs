using UnityEngine;

public class JumpPad : MonoBehaviour
{
    [SerializeField] private float bouncePower = 50f;

    [SerializeField] private bool isMovingJumpPad;
    private float changeDirectionTimer = 2.0f;
    private float currentXposition;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerMonkey>())
        {
            collision.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            collision.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.up * bouncePower, ForceMode2D.Impulse);
        }
    }

    private void Start()
    {
        if (isMovingJumpPad)
        {
            MoveJumpPad();
            currentXposition = gameObject.transform.position.x;
            
        }
    }


    private void MoveJumpPad()
    {
        LeanTween.moveX(gameObject, currentXposition, changeDirectionTimer).setEaseLinear().setLoopPingPong();
    }

}
