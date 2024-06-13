using System;
using UnityEngine;
public class PlayerMonkey2 : MonoBehaviour
{
    public static PlayerMonkey2 Instance { get; private set; }
    public event EventHandler OnPlayer2Died;
    public event EventHandler OnPlayer2Teleported;
    private IInteractable interactable = null;
    private float horizontal;
    private float vertical;
    [SerializeField] private float speed = 5f;
    [SerializeField] private LayerMask wallLayer;
    private bool isInteractable = false;
    private void Start()
    {
        Instance = this;
    }
    private void Update()
    {
        Movement();
        if (Input.GetKeyDown(KeyCode.E) && isInteractable)
        {
            OnPlayer2Teleported?.Invoke(this, EventArgs.Empty);
            SwapMonkey.Instance.SwapMonkeyMode();
            interactable.Interact();  
        }
    }
    private void Movement()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
        Vector3 movement = new Vector3 (horizontal, vertical, 0).normalized;
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
    private void OnCollisionEnter2D(Collision2D collision)
    {
        IPlayerDied playerDied = collision.gameObject.GetComponent<IPlayerDied>();
        if(playerDied != null) 
        {
            Destroy(gameObject);
            OnPlayer2Died?.Invoke(this, EventArgs.Empty);
        }
    }
}
