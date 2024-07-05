using System;
using System.Collections;
using UnityEngine;
public class PlayerMonkey : MonoBehaviour
{
    public static PlayerMonkey Instance { get; private set; }
    public event EventHandler OnPlayerDied;
    public event EventHandler OnCoconutGameModeOn;
    public event EventHandler OnCoconutGameDone;

    private IInteractable interactable = null;
    private float horizontal;
    private float vertical;
    [SerializeField] private float speed = 8f;
    [SerializeField] private float jumpingPower = 16f;
    private bool isFacingRight = true;
    private bool isFlipping = false;
    private Vector2 playerGravity;
    private bool doubleJumpUsed = false;
    private bool isInteractable = false;
    private Rigidbody2D rb2d;
    private BoxCollider2D boxCollider;
    [SerializeField] Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float fallMultiplier;

    [Header("CoconutModeStuff")]
    [SerializeField] private Transform coconutFull;
    [SerializeField] private Transform coconutCut;
    [SerializeField] private int coconutsCutMax = 10;
    private int coconutsCutten = 0;

    [Header("CoconutThrowStuff")]
    [SerializeField] private Transform coconutHolder;
    [SerializeField] private GameObject coconutThrowable;

    [SerializeField] private Rigidbody2D coconutThrowablerb2d;
    [SerializeField] private float throwingAngle = 0f;
    [SerializeField] private float throwingForce = 50f;

    public enum MonkeyState
    {
        Mode2d,
        ModeTopdown,
        ModeCoconutCut,
        ModeCoconutThrow,
    }
    private MonkeyState state;
    private MonkeyState previousState;
    private void Start()
    {
        state = MonkeyState.Mode2d;
        Instance = this;
        rb2d = GetComponent<Rigidbody2D>();
        
        boxCollider = GetComponent<BoxCollider2D>();
        playerGravity = new Vector2(0, -Physics2D.gravity.y);

        coconutThrowable.SetActive(false);
    }
    private void Update()
    {
        switch (state)
        {
            case MonkeyState.Mode2d:
                MovementModeHorizontal();
                break;
            case MonkeyState.ModeTopdown:
                MovementModeTopDown();
                break;
            case MonkeyState.ModeCoconutCut:
                OnCoconutGameModeOn?.Invoke(this, EventArgs.Empty);
                CutCoconut(); 
                break;
            case MonkeyState.ModeCoconutThrow:
                MovementModeCoconutThrow();
                Debug.Log(speed);
                break;
        }
        if (Input.GetKeyDown(KeyCode.E) && isInteractable)
        {
            interactable.Interact();
        }
        Debug.Log(state);
    }
    private bool IsGrounded()
    {
        //creates small circle and if collides with ground = we can jump
        return Physics2D.OverlapCapsule(groundCheck.position, new Vector2(1.0f, 0.2f), CapsuleDirection2D.Horizontal, 0, groundLayer);
    }
    private void FixedUpdate()
    {
        rb2d.velocity = new Vector2(horizontal * speed, rb2d.velocity.y);
    }
    private void FlipPlayerDirection()
    {
        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }
    private void Jumping()
    {
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            doubleJumpUsed = false;
            rb2d.velocity = new Vector2(rb2d.velocity.x, jumpingPower);
        }
        if (Input.GetButtonUp("Jump") && rb2d.velocity.y > 0f)
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, rb2d.velocity.y * 0.5f);
        }
        //DoubleJump
        if (Input.GetButtonDown("Jump") && !IsGrounded() && !doubleJumpUsed && !isFlipping)
        {
            doubleJumpUsed = true;
            rb2d.velocity = new Vector2(rb2d.velocity.x, jumpingPower * 1.2f);
            StartCoroutine(Rotate());
        }
    }
    IEnumerator Rotate()
    {
        float elapsedTime = 0f;
        float startRotation = transform.eulerAngles.z;
        while (elapsedTime < 0.8f)
        {
            elapsedTime += Time.deltaTime;
            if (isFacingRight)
            {
                float zRotation = Mathf.Lerp(startRotation, startRotation - 446, elapsedTime);
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, zRotation);
            }
            if (!isFacingRight)
            {
                float zRotation = Mathf.Lerp(startRotation, startRotation + 446, elapsedTime);
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, zRotation);
            }
            yield return null;
        }
        transform.eulerAngles = Vector3.zero;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        interactable = collision.GetComponent<IInteractable>();
        IPlayerDied playerDied = collision.GetComponent<IPlayerDied>();
        if (playerDied != null)
        {
            playerDied.PlayerDied();
            Destroy(gameObject);
            OnPlayerDied?.Invoke(this, EventArgs.Empty);
        }
        if (interactable != null)
        {
            isInteractable = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        interactable = null;
        isInteractable = false;
    }
    public void ChangeGravityMode()
    {
        rb2d.gravityScale = 0;
        fallMultiplier = 0;
    }
    private void MovementModeHorizontal()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        Jumping();
        FlipPlayerDirection();
        rb2d.interpolation = RigidbodyInterpolation2D.None;
        boxCollider.isTrigger = false;
        rb2d.gravityScale = 4;
        fallMultiplier = 3;

        //FALL GRAVITY SPEED
        if (rb2d.velocity.y < 0)
        {
            rb2d.velocity -= playerGravity * fallMultiplier * Time.deltaTime;
        }
    }
    private void MovementModeTopDown()
    {
        FlipPlayerDirection();
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
        rb2d.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb2d.interpolation = RigidbodyInterpolation2D.Interpolate;
        boxCollider.isTrigger = true;
        Vector3 movement = new Vector3(horizontal, vertical, 0).normalized;
        transform.position += movement * speed * Time.deltaTime;
    }

    private void MovementModeCoconutThrow()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            speed += 0.1f;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            speed += 0.1f;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            coconutThrowablerb2d.isKinematic = false;
            ThrowCoconut();
        }
    }



    private void ThrowCoconut()
    {
        throwingAngle = coconutThrowable.GetComponent<Transform>().rotation.eulerAngles.z;
        float radAngle = throwingAngle * Mathf.Deg2Rad;
        float x1 = Mathf.Cos(radAngle);
        float y1 = Mathf.Sin(radAngle);
        coconutThrowable.GetComponent<Rigidbody2D>().AddForce(new Vector2(x1, y1) * throwingForce);
    }


    public void CoconutGameModeOn()
    {
        state = MonkeyState.ModeCoconutCut;
    }
    public void CoconutThrowGameModeOn()
    {
        state = MonkeyState.ModeCoconutThrow;
        speed = 0;
    }
    public void ChangeMovementMode()
    {
        gameObject.SetActive(false);
        gameObject.SetActive(true);
        state = MonkeyState.ModeTopdown;
    }


    private void CutCoconut()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (coconutsCutten < coconutsCutMax)
            {
                Transform coconut = Instantiate(coconutCut, coconutFull.GetComponent<Transform>().position, coconutFull.GetComponent<Transform>().rotation);
                Destroy(coconut.gameObject, 15f);
                coconutsCutten++;
                Debug.Log("COCOnut");
            }
            else
            {
                OnCoconutGameDone?.Invoke(this, EventArgs.Empty);
                Debug.Log("Enough");
                state = MonkeyState.Mode2d;
                return;
            }
        }
    }

    public void ActivateCoconut()
    {
        coconutThrowable.SetActive(true);
    }

}
