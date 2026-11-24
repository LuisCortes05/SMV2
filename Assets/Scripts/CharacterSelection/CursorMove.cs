using UnityEngine;

public class CursorMove : MonoBehaviour
{
    public float speed = 600f;
    [SerializeField] private RectTransform canvasRect;

    private Vector3[] canvasCorners = new Vector3[4];

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
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        transform.position += new Vector3(x, y, 0) * Time.deltaTime * speed;

        if (canvasRect != null)
        {
            // [0] = esquina inferior izquierda, [2] = esquina superior derecha del Canvas
            canvasRect.GetWorldCorners(canvasCorners);

            transform.position = new Vector3(
                Mathf.Clamp(transform.position.x, canvasCorners[0].x, canvasCorners[2].x),
                Mathf.Clamp(transform.position.y, canvasCorners[0].y, canvasCorners[2].y),
                transform.position.z
            );
        }
    }
}
