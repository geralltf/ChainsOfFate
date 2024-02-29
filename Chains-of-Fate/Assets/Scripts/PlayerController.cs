using System;
using System.Collections;
using System.Collections.Generic;
using ChainsOfFate.Gerallt;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    //public PlayerInput playerInput;
    public CoFPlayerControls controls;
    public float speed = 10f;

    public Vector3 defaultSpawnLocation;
    
    public event Action OnReady;

    [SerializeField] private SpriteRenderer characterSpriteRenderer;
    
    private CombatUI CUI;
    public Vector2 move;
    private Rigidbody2D rb;
    private Champion player;
    private InteractTriggerBox _interactBox;

    public Animator animator;
    public bool playerMoving = false;

    public static PlayerController instance;
    public string areaTransitionName;

    private void UpdateSprite(Vector2 pos)
    {
        /*if (pos.x < 0)
        {
            flipState = true;
        }
        else
        {
            flipState = false;
        }

        if (flipState != characterSpriteRenderer.flipX)
        {
            characterSpriteRenderer.flipX = flipState;
        }

        characterSpriteRenderer.transform.rotation = Quaternion.identity;*/
    }

    private void Awake()
    {
        controls = new CoFPlayerControls();
        controls.Player.Movement.performed += ctx => move = ctx.ReadValue<Vector2>(); //on button press gets the movement value and starts the movement
        controls.Player.Movement.canceled += ctx => move = Vector2.zero; //on button release stops movement
        //DontDestroyOnLoad(this);
        controls.Player.Interact.performed += Interact;

        rb = GetComponent<Rigidbody2D>();

        if(instance == null)
        {
            instance = this;
        } else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        controls.Player.Enable();
    }

    private void OnDisable()
    {
        controls.Player.Disable();
    }

    private void Start()
    {
        defaultSpawnLocation = transform.position;
        
        OnReady?.Invoke();

        player = ChainsOfFate.Gerallt.GameManager.Instance.GetPlayer();
        _interactBox = player.GetComponentInChildren<InteractTriggerBox>();
    }

    private bool flipState = false;
    
    public void FixedUpdate()
    {
        Vector2 movement = new Vector2(move.x, move.y) * speed * player.MovementSpeed * Time.fixedDeltaTime; //gets a value based on actual seconds not pc specs that is used to calculate movement

        if (movement != Vector2.zero)
        {
            playerMoving = true;
            //transform.Translate(movement,Space.World);
        
            // Switched to a rigidbody version instead of directly affecting transform
            // because there's a 2D collision system using 2D Colliders
            rb.MovePosition(rb.position + movement);
            
            //rb.AddRelativeForce(movement, ForceMode2D.Force);
            //rb.AddRelativeForce(movement, ForceMode2D.Impulse);
            //rb.velocity += movement;

            UpdateSprite(movement);

            animator.SetFloat("Horizontal", move.x); //code that checks the movement direction for the animator to use for displaying the walking animations
            animator.SetFloat("Vertical", move.y);

            //mariaAnim.SetFloat("Horizontal", move.x); 
            //mariaAnim.SetFloat("Vertical", move.y);

            animator.SetBool("isMoving", true);
            //mariaAnim.SetBool("isMoving", true);
        }
        else
        {
            playerMoving = false;
            animator.SetBool("isMoving", false);
            //mariaAnim.SetBool("isMoving", false);//if there is no movement isMoving is set to false which sets the animator state to idle.
        }
    }

    private void Interact(InputAction.CallbackContext context)
    {
	    _interactBox.InteractEvent?.Invoke();
    }
}
