using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAction : MonoBehaviour
{
    public float interactionDistance = 2f; 
    public LayerMask interactableLayer; 
    private PlayerInput playerInput;

    private void Awake()
    {
        playerInput = new PlayerInput();
        playerInput.Player.Action.performed += ctx => Interact();
    }

    private void OnEnable()
    {
        playerInput.Enable();
    }

    private void OnDisable()
    {
        playerInput.Disable();
    }

    private void Interact()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsDialogueActive)
        {
            Debug.Log("Interaction disabled during dialogue.");
            return;
        }

        Collider2D[] interactableObjects = Physics2D.OverlapCircleAll(transform.position, interactionDistance, interactableLayer);

        foreach (Collider2D collider in interactableObjects)
        {
            InteractionIndicator interactionIndicator = collider.GetComponent<InteractionIndicator>();

            if (interactionIndicator != null && interactionIndicator.IsIndicatorActive())
            {
                Debug.Log("Interacting with: " + collider.name);
                interactionIndicator.ActivatePanel();
                return;
            }
        }

        Debug.Log("No valid interactable objects found.");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}
