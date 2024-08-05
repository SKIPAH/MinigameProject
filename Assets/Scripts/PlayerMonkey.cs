using System;
using System.Collections;
using UnityEngine;
public class PlayerMonkey : MonoBehaviour
{
    public static PlayerMonkey Instance { get; private set; }
    public event EventHandler OnPlayerDied;
    public event EventHandler OnCoconutGameModeOn;
    public event EventHandler OnCoconutGameDone;
    public event EventHandler OnCoconutThrown;

    [SerializeField] private float movementSpeed = 8f;
    [SerializeField] private float jumpingPower = 16f;
    [SerializeField] Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float fallMultiplier;

    private IInteractable interactable;
    private float horizontal;
    private float vertical;
    private bool isFacingRight = true;
    private bool isFlipping = false;
    private Vector2 playerGravity;
    private bool isDoubleJumpUsed = false;
    private bool isInteractable = false;
    private Rigidbody2D monkeyRB2D;
    private BoxCollider2D monkeyBoxCollider2D;
    private Quaternion currentRotation;
    

    [Header("CoconutModeStuff")]
    [SerializeField] private Transform coconutFull;
    [SerializeField] private Transform coconutCut;
    [SerializeField] private int coconutsCutMax = 30;
    [SerializeField] private GameObject sword;
    private bool canCutCoconut = false;

    private int coconutsCutten = 0;

    [Header("CoconutThrowStuff")]
    [SerializeField] private Transform coconutHolder;
    [SerializeField] private GameObject coconutThrowable;
    [SerializeField] private Rigidbody2D coconutThrowableRB2D;
    [SerializeField] private float throwingAngle = 0f;
    [SerializeField] private float throwingForce = 50f;

    public enum MonkeyState
    {
        Mode2d,
        ModeTopdown,
        ModeCoconutCut,
        ModeCoconutThrow,
    }
    private MonkeyState currentState;
    private MonkeyState previousState;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    private void Start()
    {
        currentRotation = gameObject.transform.rotation;
        currentState = MonkeyState.Mode2d;   
        monkeyRB2D = GetComponent<Rigidbody2D>();    
        monkeyBoxCollider2D = GetComponent<BoxCollider2D>();
        playerGravity = new Vector2(0, -Physics2D.gravity.y);
        if (coconutThrowable){
            coconutThrowable.SetActive(false);
        }
        if (sword)
        {
            sword.SetActive(false);
        }
        
    }
    private void Update()
    {
        StateMachine();
        MonkeyInteract();

        Debug.Log(currentState);

        if (Input.GetKeyDown(KeyCode.O))
        {
            ResetMonkeyRotation();
        }
    }
    private void FixedUpdate()
    {
        monkeyRB2D.velocity = new Vector2(horizontal * movementSpeed, monkeyRB2D.velocity.y);
    }

    private void MonkeyInteract()
    {
        if (Input.GetKeyDown(KeyCode.E) && isInteractable)
        {
            interactable.Interact();
        }
    }

    private void StateMachine()
    {
        switch (currentState)
        {
            case MonkeyState.Mode2d:
                MovementModeHorizontal();
                break;
            case MonkeyState.ModeTopdown:
                MovementModeTopDown();
                break;
            case MonkeyState.ModeCoconutCut:
                OnCoconutGameModeOn?.Invoke(this, EventArgs.Empty);
                sword.SetActive(true);
                CutCoconut();
                break;
            case MonkeyState.ModeCoconutThrow:
                MovementModeCoconutThrow();
                sword.SetActive(false);
                Debug.Log(movementSpeed);
                break;
        }
    }

    private bool IsGrounded()
    {
        //creates small circle and if collides with ground = we can jump
        return Physics2D.OverlapCapsule(groundCheck.position, new Vector2(1.0f, 0.2f), CapsuleDirection2D.Horizontal, 0, groundLayer);
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

    public void FlipToRightSide()
    {
        if (!isFacingRight)
        {
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }
    private void Jumping()
    {
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            isDoubleJumpUsed = false;
            monkeyRB2D.velocity = new Vector2(monkeyRB2D.velocity.x, jumpingPower);
        }
        if (Input.GetButtonUp("Jump") && monkeyRB2D.velocity.y > 0f)
        {
            monkeyRB2D.velocity = new Vector2(monkeyRB2D.velocity.x, monkeyRB2D.velocity.y * 0.5f);
        }
        if (Input.GetButtonDown("Jump") && !IsGrounded() && !isDoubleJumpUsed && !isFlipping)
        {
            isDoubleJumpUsed = true;
            monkeyRB2D.velocity = new Vector2(monkeyRB2D.velocity.x, jumpingPower * 1.2f);
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
        monkeyRB2D.gravityScale = 0;
        fallMultiplier = 0;
    }
    private void MovementModeHorizontal()
    {
        monkeyRB2D.interpolation = RigidbodyInterpolation2D.None;
        monkeyBoxCollider2D.isTrigger = false;
        monkeyRB2D.gravityScale = 4;
        fallMultiplier = 3;
        
        horizontal = Input.GetAxisRaw("Horizontal");
        Jumping();
        FlipPlayerDirection();
        

        //FALL GRAVITY SPEED
        if (monkeyRB2D.velocity.y < 0)
        {
            monkeyRB2D.velocity -= playerGravity * fallMultiplier * Time.deltaTime;
        }
    }
    private void MovementModeTopDown()
    {
        monkeyRB2D.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        monkeyRB2D.interpolation = RigidbodyInterpolation2D.Interpolate;
        monkeyBoxCollider2D.isTrigger = true;
        
        FlipPlayerDirection();
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
        
        Vector3 movement = new Vector3(horizontal, vertical, 0).normalized;
        transform.position += movement * movementSpeed * Time.deltaTime;
    }

    private void MovementModeCoconutThrow()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            movementSpeed += 0.1f;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            movementSpeed += 0.1f;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            ThrowCoconut();
            OnCoconutThrown?.Invoke(this, EventArgs.Empty);
        }
    }

    private void ThrowCoconut()
    {
        coconutThrowableRB2D.isKinematic = false;
        throwingAngle = coconutThrowable.GetComponent<Transform>().rotation.eulerAngles.z;
        float radAngle = throwingAngle * Mathf.Deg2Rad;
        float x1 = Mathf.Cos(radAngle);
        float y1 = Mathf.Sin(radAngle);
        throwingForce = movementSpeed * 160;
        coconutThrowable.GetComponent<Rigidbody2D>().AddForce(new Vector2(x1, y1) * throwingForce);
    }


    public void CoconutGameModeOn()
    {
        currentState = MonkeyState.ModeCoconutCut;
    }
    public void CoconutThrowGameModeOn()
    {
        currentState = MonkeyState.ModeCoconutThrow;
        movementSpeed = 0;
        CameraManager.Instance.ChangeCameraToFollowCoconut();
    }
    public void ChangeMovementMode()
    {
        gameObject.SetActive(false);
        gameObject.SetActive(true);
        currentState = MonkeyState.ModeTopdown;
    }


    private void CutCoconut()
    {
        if (canCutCoconut && Input.GetKeyDown(KeyCode.Space))
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
                currentState = MonkeyState.Mode2d;
                return;
            }
        }
    }
    public void ActivateCoconut()
    {
        coconutThrowable.SetActive(true);
    }
    public void DeactivateSword()
    {
        sword.SetActive(false);
    }
    public void CanCutCoconut()
    {
        canCutCoconut = true;
    }

    public void ResetMonkeyRotation()
    {
        gameObject.transform.rotation = currentRotation;
    }
}
