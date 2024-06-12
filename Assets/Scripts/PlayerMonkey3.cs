
using UnityEngine;


public class PlayerMonkey3 : MonoBehaviour
{

    public static PlayerMonkey3 Instance { get; private set; }

    [SerializeField] private Transform coconutFull;
    [SerializeField] private Transform coconutCut;

    [SerializeField] private float speed = 5f;

    private IInteractable interactable = null;

    private bool isInteractable = false;

    private int coconutsCutMax = 50;
    private int coconutsCutten = 0;

    private float horizontal;
    private float vertical;

    private bool canMove = false;

    public enum GameMonkeyState
    {
        coconutGame,
        coconutThrow,
    }
    private GameMonkeyState state;

    private void Start()
    {
        Instance = this;
        state = GameMonkeyState.coconutGame;
    }
    private void Update()
    {

        switch (state)
        {
            case GameMonkeyState.coconutGame:
                CutCoconutState();
                break;
            case GameMonkeyState.coconutThrow:
                CoconutThrowState();
                break;
        }

    }

    private void CutCoconut()
    {
        if(Input.GetKeyDown(KeyCode.Space)) 
        {
            if(coconutsCutten < coconutsCutMax )
            {
                Transform coconut = Instantiate(coconutCut, coconutFull.GetComponent<Transform>().position, coconutFull.GetComponent<Transform>().rotation);
                Destroy(coconut.gameObject, 15f);
                coconutsCutten++;
            }
            else
            {
                Debug.Log("Enough");
                canMove = true;
            }
        }

    }

    private void Movement()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
        Vector3 movement = new Vector3(horizontal, vertical, 0).normalized;

        transform.position += movement * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        interactable = collision.GetComponent<IInteractable>();
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



    public void CutCoconutState()
    {
        CutCoconut();
        if (canMove)
        {
            Movement();
        }
        if (Input.GetKeyDown(KeyCode.E) && isInteractable)
        {
            interactable.Interact();
        }
    }

    public void CoconutThrowState()
    {
        state = GameMonkeyState.coconutGame;
        Debug.Log("coconutthrowstate");
    }

   
}
