using UnityEngine;

public class JumpPad : MonoBehaviour
{
    [SerializeField] private float bouncePower = 50f;

    [SerializeField] private bool isMovingJumpPad;
    private float changeDirectionTimer = 5.0f;
    private float currentXposition;
    private float moveToPosition;
    private float randomTimer;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerMonkey>())
        {
            collision.gameObject.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
            collision.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.up * bouncePower, ForceMode2D.Impulse);
        }
    }

    private void Start()
    {
        randomTimer = Random.Range(1.0f, 5.0f);
        currentXposition = gameObject.transform.position.x;
        moveToPosition = currentXposition - 24f;
        if (isMovingJumpPad)
        {
            MoveJumpPad();
        }
    }

    private void MoveJumpPad()
    {
        LeanTween.moveX(gameObject, moveToPosition, randomTimer).setEaseLinear().setLoopPingPong();
    }

}
