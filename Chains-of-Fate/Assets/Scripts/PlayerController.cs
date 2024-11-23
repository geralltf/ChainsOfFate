using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
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

    [SerializeField] private Camera _mainCamera;
    [SerializeField] private float cameraZ;
    [SerializeField] private float cameraMovementSpeed;

    private CombatUI CUI;
    public Vector2 move;
    private Rigidbody2D rb;
    private Champion player;
    private InteractTriggerBox _interactBox;

    public Animator animator;
    public bool playerMoving = false;

    public static PlayerController instance;
    public string areaTransitionName;

    public RuntimeAnimatorController newControllerWalk;
    public RuntimeAnimatorController newControllerAttack;
    public RuntimeAnimatorController newControllerUseItem;

    public GameObject spotlight2DTorch;

    private AnimControllerState animControllerState = AnimControllerState.Walking;

    public enum AnimControllerState
    {
        Walking,
        Attack,
        UseItem
    }

    private void UpdateSprite(Vector2 pos)
    {
        if (animControllerState == AnimControllerState.Attack || animControllerState == AnimControllerState.UseItem)
        {
            if (pos.x < 0)
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

            characterSpriteRenderer.transform.rotation = Quaternion.identity;
        }
        else
        {
            flipState = false;

            if (flipState != characterSpriteRenderer.flipX)
            {
                characterSpriteRenderer.flipX = flipState;
            }

            characterSpriteRenderer.transform.rotation = Quaternion.identity;
        }

        if(pos.x < 0)
        {
            spotlight2DTorch.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 90.0f);

            if (pos.y < 0)
            {
                spotlight2DTorch.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 90.0f);
            }
            else
            {
                spotlight2DTorch.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0);
            }
        }
        else
        {
            spotlight2DTorch.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);

            if (pos.y < 0)
            {
                spotlight2DTorch.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 180.0f);
            }
            else
            {
                spotlight2DTorch.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 270.0f);
            }
        }
    }

    void MovementFromControls(InputAction.CallbackContext ctx)
    {
        move = ctx.ReadValue<Vector2>(); //on button press gets the movement value and starts the movement
    }
    void CancelMovementFromControls(InputAction.CallbackContext ctx)
    {
        move = Vector2.zero; //on button release stops movement
    }

    private void Awake()
    {
        controls = new CoFPlayerControls();
        controls.Player.Movement.performed += ctx => MovementFromControls(ctx);
        controls.Player.Movement.canceled += ctx => CancelMovementFromControls(ctx);
        //DontDestroyOnLoad(this);
        controls.Player.Interact.performed += Interact;

        controls.Player.Attack.performed += Attack_performed;
        controls.Player.UseItem.performed += UseItem_performed;

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

            //mariaAnim.SetFloat("Horizontal", move.x); 
            //mariaAnim.SetFloat("Vertical", move.y);
            //animator.runtimeAnimatorController

            if(animControllerState == AnimControllerState.Walking)
            {
                animator.SetFloat("Horizontal", move.x); //code that checks the movement direction for the animator to use for displaying the walking animations
                animator.SetFloat("Vertical", move.y);

                animator.SetBool("isMoving", true);
                //mariaAnim.SetBool("isMoving", true);
            }
        }
        else
        {
            playerMoving = false;

            if (animControllerState == AnimControllerState.Walking)
            {
                animator.SetBool("isMoving", false);
            }
            //mariaAnim.SetBool("isMoving", false);//if there is no movement isMoving is set to false which sets the animator state to idle.
        }

        //if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)// && !animator.IsInTransition(0))
        //if(animator.GetCurrentAnimatorStateInfo(0).length > animator.GetCurrentAnimatorStateInfo(0).normalizedTime)
        //if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !animator.IsInTransition(0))
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
        {
            if(animControllerState == AnimControllerState.Attack || animControllerState == AnimControllerState.UseItem)
            {
                animator.runtimeAnimatorController = newControllerWalk;

                animControllerState = AnimControllerState.Walking;
            }
        }

        if (_mainCamera != null)
        {
            _mainCamera.transform.position = Vector3.Lerp(_mainCamera.transform.position, rb.position, Time.fixedDeltaTime * cameraMovementSpeed);
            _mainCamera.transform.position = new Vector3(_mainCamera.transform.position.x, _mainCamera.transform.position.y, cameraZ);
        }


    }

    private void Interact(InputAction.CallbackContext context)
    {
	    _interactBox.InteractEvent?.Invoke();
    }

    private void UseItem_performed(InputAction.CallbackContext context)
    {
        animator.runtimeAnimatorController = newControllerUseItem;
        //animator.Play();
        animControllerState = AnimControllerState.UseItem;
    }

    private void Attack_performed(InputAction.CallbackContext context)
    {
        animator.runtimeAnimatorController = newControllerAttack;

        animControllerState = AnimControllerState.Attack;
    }
}
