using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    public float speed = 50f;
    public float jumpForce = 10f;

    [SerializeField] PlayerInput playerInput;
    private Vector2 entry;

    //[SerializeField] private Animator anim;

    private void Update()
    {
        entry = playerInput.actions["Move"].ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        rb.AddForce(new Vector3(entry.x, 0f, entry.y) * speed);
    }

    public void Jump(InputAction.CallbackContext callbackContext)
    {
        if(callbackContext.performed)
        {
            rb.AddForce(Vector3.up * jumpForce);
        }
    }
}
