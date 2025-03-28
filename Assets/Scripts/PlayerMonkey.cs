using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
public class PlayerMonkey : MonoBehaviour
{
    public static PlayerMonkey Instance { get; private set; }
    public event EventHandler OnPlayerDied;
    public event EventHandler OnCoconutGameModeOn;
    public event EventHandler OnCoconutGameDone;
    public event EventHandler OnCoconutThrown;
    public event EventHandler OnCoconutThrowModeOn;
    public event EventHandler OnCoconutCutten;
    public event EventHandler OnPowerIncreased;
    public event EventHandler OnCoconutHitGeorge;
    public event EventHandler OnCoconutDidNotHitGeorge;

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
    [SerializeField] private BoxCollider2D monkeyTriggerBoxCollider2D;
    private Quaternion defaultRotation;
    

    [Header("CoconutModeStuff")]
    [SerializeField] private Transform coconutFull;
    [SerializeField] private Transform coconutCut;
    [SerializeField] private GameObject sword;
    private bool canCutCoconut = false;


    [Header("CoconutThrowStuff")]
    [SerializeField] private Transform coconutHolder;
    [SerializeField] private GameObject coconutThrowable;
    [SerializeField] private Rigidbody2D coconutThrowableRB2D;
    [SerializeField] private CoconutPickable coconutPickable;
    [SerializeField] private float throwingAngle = 0f;
    [SerializeField] private float throwingForce = 50f;
    private float throwingPower = 0f;
    private bool isCoconutThrown = false;
    private bool isPlatformActive = true;
    private bool isCoconutFarEnough = false;

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
        isCoconutThrown = false;
        defaultRotation = gameObject.transform.rotation;
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

        if(Input.GetKeyDown(KeyCode.J))
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
            Debug.Log("Pressed J");
        }
    }
    private void FixedUpdate()
    {
        monkeyRB2D.linearVelocity = new Vector2(horizontal * movementSpeed, monkeyRB2D.linearVelocity.y);
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

        if(currentState != previousState)
        {
            OnStateExit(previousState);
            OnStateEnter(currentState);
            previousState = currentState;
        }
        switch (currentState)
        {
            case MonkeyState.Mode2d:
                MovementModeHorizontal();
                break;
            case MonkeyState.ModeTopdown:
                MovementModeTopDown();
                break;
            case MonkeyState.ModeCoconutCut:
                CutCoconut();
                break;
            case MonkeyState.ModeCoconutThrow:
                MovementModeCoconutThrow();
                break;
        }
    }

    private void OnStateEnter(MonkeyState state)
    {
        switch (state)
        {
            case MonkeyState.ModeCoconutCut:
                OnCoconutGameModeOn?.Invoke(this, EventArgs.Empty);
                sword.SetActive(true);
                break;
            case MonkeyState.ModeCoconutThrow:
                OnCoconutThrowModeOn?.Invoke(this, EventArgs.Empty);
                sword.SetActive(false);
                break;
        }
    }

    private void OnStateExit(MonkeyState state)
    {
        switch (state)
        {
            case MonkeyState.ModeCoconutCut:
                break;
            case MonkeyState.ModeCoconutThrow:
                break;
        }
    }
    private bool IsGrounded()
    {
        //creates small circle and if collides with ground = we can jump
        return Physics2D.OverlapCapsule(groundCheck.position, new Vector2(1.0f, 0.2f), CapsuleDirection2D.Horizontal, 0, groundLayer);
        
    }
   
    public void FlipPlayerDirection()
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
            FlipPlayerDirection();
        }
    }


    private void Jumping()
    {
        if (IsGrounded())
        {
            isDoubleJumpUsed = false;
        }
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            isDoubleJumpUsed = false;
            monkeyRB2D.linearVelocity = new Vector2(monkeyRB2D.linearVelocity.x, jumpingPower);
         
        }
        if (Input.GetButtonUp("Jump") && monkeyRB2D.linearVelocity.y > 0f)
        {
            monkeyRB2D.linearVelocity = new Vector2(monkeyRB2D.linearVelocity.x, monkeyRB2D.linearVelocity.y * 0.5f);
        }
        if (Input.GetButtonDown("Jump") && !IsGrounded() && !isDoubleJumpUsed && !isFlipping)
        {
            isDoubleJumpUsed = true;
            monkeyRB2D.linearVelocity = new Vector2(monkeyRB2D.linearVelocity.x, jumpingPower * 1.2f);
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
        if (monkeyRB2D.linearVelocity.y < 0)
        {
            monkeyRB2D.linearVelocity -= playerGravity * fallMultiplier * Time.deltaTime;
        }
    }
    private void MovementModeTopDown()
    {
        monkeyRB2D.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        monkeyRB2D.interpolation = RigidbodyInterpolation2D.Interpolate;
        monkeyBoxCollider2D.isTrigger = false;
        monkeyTriggerBoxCollider2D.isTrigger = true;


        FlipPlayerDirection();
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
        
        Vector3 movement = new Vector3(horizontal, vertical, 0).normalized;
        transform.position += movement * movementSpeed * Time.deltaTime;
    }

    private void MovementModeCoconutThrow()
    {
        monkeyRB2D.isKinematic = true;
        if (Input.GetKeyDown(KeyCode.A) || (Input.GetKeyDown(KeyCode.D)))
        {
            throwingPower += 0.1f;
            OnPowerIncreased?.Invoke(this, EventArgs.Empty);
        }
        if (!isCoconutThrown && throwingPower >= 12.0f)
        {
            Debug.Log("THROWING FORCE IS 12");
            ThrowCoconut();
            OnCoconutThrown?.Invoke(this, EventArgs.Empty);
            isCoconutThrown = true;

            FunctionTimer.Create(() => ChangeTo2DModeFromThrowMode(), 10f);
        }
    }

   
    public void CoconutHitGeorge()
    {
        isCoconutFarEnough = true;
        OnCoconutHitGeorge?.Invoke(this, EventArgs.Empty);
    }


    private void ThrowCoconut()
    {
        coconutThrowableRB2D.isKinematic = false;
        throwingAngle = coconutThrowable.GetComponent<Transform>().rotation.eulerAngles.z;
        float radAngle = throwingAngle * Mathf.Deg2Rad;
        float x1 = Mathf.Cos(radAngle);
        float y1 = Mathf.Sin(radAngle);
        throwingForce = throwingPower * 160;
        coconutThrowable.GetComponent<Rigidbody2D>().AddForce(new Vector2(x1, y1) * throwingForce);
    }


    public void CoconutGameModeOn()
    {
        isFlipping = false;
        ResetMonkeyRotation();
        currentState = MonkeyState.ModeCoconutCut;
    }
    public void CoconutThrowGameModeOn()
    {
        if (isPlatformActive)
        {
            if (!isFacingRight)
            {
                isFacingRight = !isFacingRight;
                Vector3 localScale = transform.localScale;
                localScale.x *= -1f;
                transform.localScale = localScale;
            }
            currentState = MonkeyState.ModeCoconutThrow;
            movementSpeed = 0;
            CameraManager.Instance.ChangeCameraToFollowCoconut();
            isPlatformActive = false;
          
        }
        
    }
    public void ChangeMovementModeToTopDown()
    {
        gameObject.SetActive(false);
        gameObject.SetActive(true);
        currentState = MonkeyState.ModeTopdown;
    }

    public void ChangeTo2DModeFromThrowMode()
    {
        CameraManager.Instance.ChangeCameraToFollowMonkey();
        monkeyRB2D.isKinematic = false;
        movementSpeed = 8f;
        currentState = MonkeyState.Mode2d;
        if (isCoconutFarEnough)
        {
            Debug.Log("Game Done");
            OnCoconutHitGeorge?.Invoke(this, EventArgs.Empty);
        }
        if(!isCoconutFarEnough)
        {
            Debug.Log("Try Again");
            OnCoconutDidNotHitGeorge?.Invoke(this, EventArgs.Empty);
        }

    }


    private void CutCoconut()
    {
        if (canCutCoconut && Input.GetKeyDown(KeyCode.Space))
        {
            Transform coconut = Instantiate(coconutCut, coconutFull.GetComponent<Transform>().position, coconutFull.GetComponent<Transform>().rotation);
            Destroy(coconut.gameObject, 15f);
            OnCoconutCutten?.Invoke(this, EventArgs.Empty);
        }

    }


    public void CoconutCutGameDone()
    {
        OnCoconutGameDone?.Invoke(this, EventArgs.Empty);
        currentState = MonkeyState.Mode2d;
        FunctionTimer.Create(() => isFlipping = false, 2f);
        FunctionTimer.Create(() => ResetMonkeyRotation(), 2f);

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
        gameObject.transform.rotation = defaultRotation;


    }
}
