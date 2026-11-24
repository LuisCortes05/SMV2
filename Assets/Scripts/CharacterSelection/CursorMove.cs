using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class CursorMove : MonoBehaviour
{
    public float speed = 600f;
    [SerializeField] private RectTransform canvasRect;

    private PlayerInput playerInput;
    private InputAction moveAction;
    private Vector3[] canvasCorners = new Vector3[4];
    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
    }

    void Start()
    {
        if (canvasRect == null)
        {
            Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas != null)
                canvasRect = canvas.GetComponent<RectTransform>();
        }
    }

    void Update()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();

        transform.position += new Vector3(input.x, input.y, 0f) * Time.deltaTime * speed;

        if (canvasRect != null)
        {
            // [0] = esquina inferior izquierda, [2] = esquina superior derecha del Canvas[cite: 14]
            canvasRect.GetWorldCorners(canvasCorners);

            transform.position = new Vector3(
                Mathf.Clamp(transform.position.x, canvasCorners[0].x, canvasCorners[2].x),
                Mathf.Clamp(transform.position.y, canvasCorners[0].y, canvasCorners[2].y),
                transform.position.z
            );
        }
    }
}
