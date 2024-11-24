using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    private Vector2 movementDirection;
    private Rigidbody2D rb;

    [Header("Animations")]
    [SerializeField] private Animator anim;
    private string lastDirection = "Down";
    private PlayerInput controls;

    private Vector2 lastActiveInput = Vector2.zero;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        controls = new PlayerInput();
    }

    private void OnEnable()
    {
        controls.Player.Move.performed += ctx =>
        {
            if (IsMovementLocked())
            {
                movementDirection = Vector2.zero;
                return;
            }

            Vector2 input = ctx.ReadValue<Vector2>();

            if (input != Vector2.zero)
            {
                lastActiveInput = input;
                SetDirection(input);
            }
        };

        controls.Player.Move.canceled += ctx =>
        {
            if (IsMovementLocked())
            {
                movementDirection = Vector2.zero;
                return;
            }

            Vector2 input = ctx.ReadValue<Vector2>();

            if (input == Vector2.zero)
            {
                lastActiveInput = Vector2.zero;
                movementDirection = Vector2.zero;
            }
            else
            {
                lastActiveInput = input;
                SetDirection(lastActiveInput);
            }
        };

        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void Update()
    {
        if (IsMovementLocked())
        {
            rb.velocity = Vector2.zero;
            return;
        }

        HandleAnimation();
    }

    private void FixedUpdate()
    {
        if (IsMovementLocked())
        {
            rb.velocity = Vector2.zero;
            return;
        }

        rb.velocity = movementDirection * moveSpeed;
    }

    private void HandleAnimation()
    {
        if (GameManager.Instance.IsDialogueActive || anim == null)
        {
            return; 
        }

        anim.SetTrigger(lastDirection);
    }

    private void SetDirection(Vector2 input)
    {
        if (IsMovementLocked()) return;

        if (input.y > 0.01f)
        {
            lastDirection = "Up";
            movementDirection = Vector2.up;
        }
        else if (input.y < -0.01f)
        {
            lastDirection = "Down";
            movementDirection = Vector2.down;
        }
        else if (input.x > 0.01f)
        {
            lastDirection = "Right";
            movementDirection = Vector2.right;
        }
        else if (input.x < -0.01f)
        {
            lastDirection = "Left";
            movementDirection = Vector2.left;
        }
    }

    private bool IsMovementLocked()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsDialogueActive)
        {
            return true;
        }

        if (GameManager.Instance != null && GameManager.Instance.ArePanelsActive())
        {
            return true;
        }

        return false;
    }
}
