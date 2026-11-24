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

    [Header("Opponent Tracking")]
    [SerializeField] private Transform opponent;

    private void Awake()
    {
        if (playerInput == null)
            playerInput = GetComponent<PlayerInput>();
    }

    private void Start()
    {
        groundHeight = transform.position.y;

        if (opponent == null)
            FindOpponent();

        FaceOpponent();
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

        FaceOpponent();
    }

    private void FindOpponent()
    {
        PlayerMovement[] players = FindObjectsByType<PlayerMovement>(FindObjectsSortMode.None);
        foreach (PlayerMovement player in players)
        {
            if (player != this)
            {
                opponent = player.transform;
                break;
            }
        }
    }

    public void SetOpponent(Transform rivalTransform)
    {
        opponent = rivalTransform;
    }

    private void FaceOpponent()
    {
        if (opponent == null) return;

        float differenceX = opponent.position.x - transform.position.x;

        // Evita cambios bruscos en el instante exacto en que ambos comparten la misma coordenada X al saltar
        if (Mathf.Abs(differenceX) < 0.01f) return;

        Vector3 scale = transform.localScale;
        scale.x = (differenceX > 0f) ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        transform.localScale = scale;
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