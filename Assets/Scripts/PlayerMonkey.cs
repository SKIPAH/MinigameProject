using System;
using System.Collections;
using Unity.VisualScripting;
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
    private float speed = 8f;
    private float jumpingPower = 16f;
    private bool isFacingRight = true;
    private bool isFlipping = false;
    private Vector2 playerGravity;
    private bool doubleJumpUsed = false;
    private bool isInteractable = false;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float fallMultiplier;

    [Header("CoconutModeStuff")]
    [SerializeField] private Transform coconutFull;
    [SerializeField] private Transform coconutCut;
    [SerializeField] private int coconutsCutMax = 10;
    private int coconutsCutten = 0;
    public enum MonkeyState
    {
        Mode2d,
        Modetopdown,
        Modecoconut
    }
    private MonkeyState state;
    private MonkeyState previousState;
    private void Start()
    {
        state = MonkeyState.Mode2d;
        Instance = this;
        rb = GetComponent<Rigidbody2D>();
        playerGravity = new Vector2(0, -Physics2D.gravity.y);
    }
    private void Update()
    {
        switch (state)
        {
            case MonkeyState.Mode2d:
                MovementModeHorizontal();
                break;
            case MonkeyState.Modetopdown:
                MovementModeTopDown();
                break;
            case MonkeyState.Modecoconut:
                OnCoconutGameModeOn?.Invoke(this, EventArgs.Empty);
                CutCoconut(); 
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
        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
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
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
        }
        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }
        //DoubleJump
        if (Input.GetButtonDown("Jump") && !IsGrounded() && !doubleJumpUsed && !isFlipping)
        {
            doubleJumpUsed = true;
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower * 1.2f);
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
        rb.gravityScale = 0;
        fallMultiplier = 0;
    }
    private void MovementModeHorizontal()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        Jumping();
        FlipPlayerDirection();
        rb.interpolation = RigidbodyInterpolation2D.None;
        boxCollider.isTrigger = false;
        rb.gravityScale = 4;
        fallMultiplier = 3;

        //FALL GRAVITY SPEED
        if (rb.velocity.y < 0)
        {
            rb.velocity -= playerGravity * fallMultiplier * Time.deltaTime;
        }
    }
    private void MovementModeTopDown()
    {
        FlipPlayerDirection();
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        boxCollider.isTrigger = true;
        Vector3 movement = new Vector3(horizontal, vertical, 0).normalized;
        transform.position += movement * speed * Time.deltaTime;
    }
    public void CoconutGameModeOn()
    {
        state = MonkeyState.Modecoconut;
    }
    public void ChangeMovementMode()
    {
        gameObject.SetActive(false);
        gameObject.SetActive(true);
        state = MonkeyState.Modetopdown;
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

}
