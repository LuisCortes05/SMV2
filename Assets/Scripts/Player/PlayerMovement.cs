using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 10f;
    public float gravity = 25f;

    [SerializeField] private PlayerInput playerInput;

    private float verticalVelocity;
    private float groundHeight;
    private bool isGrounded = true;
    private void Awake()
    {
        if (playerInput == null)
            playerInput = GetComponent<PlayerInput>();
    }

    private void Start()
    {
        groundHeight = transform.position.y;
    }

    private void Update()
    {
        Vector2 entry = playerInput.actions["Move"].ReadValue<Vector2>();

        if (!isGrounded)
        {
            verticalVelocity -= gravity * Time.deltaTime;
        }

        Vector3 position = transform.position;

        position.x += entry.x * speed * Time.deltaTime;
        position.y += verticalVelocity * Time.deltaTime;

        if (position.y <= groundHeight && verticalVelocity <= 0f)
        {
            position.y = groundHeight;
            verticalVelocity = 0f;
            isGrounded = true;
        }

        transform.position = position;
    }

    public void Jump(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.performed && isGrounded)
        {
            verticalVelocity = jumpForce;
            isGrounded = false;
        }
    }

    private void OnDestroy()
    {
        if (playerInput != null && playerInput.actions["Jump"] != null)
            playerInput.actions["Jump"].performed -= Jump;
    }
}