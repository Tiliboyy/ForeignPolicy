using UnityEngine;

public class CameraPanZoom : MonoBehaviour
{
    public float zoomSpeed = 10f;
    public float panSpeed = 0.5f;
    public float zoomLerpSpeed = 10f;
    public float minZoom = 5f;
    public float maxZoom = 100f;

    public float edgeScrollSpeed = 1f;   // Speed of edge scrolling
    public float edgeThickness = 10f;     // How close the cursor needs to be to the edge to scroll
    public Vector2 cameraBoundsMin = new Vector2(-50f, -50f);  // Bottom left bound
    public Vector2 cameraBoundsMax = new Vector2(50f, 50f);   // Top right bound

    private Camera cam;
    private Vector3 targetPosition;
    private Vector3 currentVelocity;

    void Start()
    {
        cam = Camera.main;
        targetPosition = cam.transform.position;
    }

    void Update()
    {
        HandleZoom();
        HandlePan();
        HandleEdgeScrolling();
        SmoothMove();
    }

    void HandlePan()
    {
        // Only pan if the right or middle mouse button is held down
        if (Input.GetMouseButton(2) || Input.GetMouseButton(1))
        {
            Vector3 drag = new Vector3(-Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y"), 0);
            float zoomFactor = cam.orthographic ? cam.orthographicSize : cam.transform.position.y;
            drag *= panSpeed * zoomFactor;
            targetPosition += cam.transform.TransformDirection(drag);
        }
    }

    void HandleZoom()
    {
        // Zoom with mouse scroll
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (Mathf.Abs(scroll) > 0.01f)
        {
            Vector3 mouseScreenPos = Input.mousePosition;

            if (scroll > 0f) // Zooming in
            {
                ZoomIn(mouseScreenPos);
            }
            else if (scroll < 0f) // Zooming out
            {
                ZoomOut();
            }
        }
    }

    void ZoomIn(Vector3 mouseScreenPos)
    {
        // Calculate mouse world position before zooming
        Vector3 mouseWorldBeforeZoom = cam.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, Mathf.Abs(cam.transform.position.z)));

        // Zoom in by reducing orthographic size or adjusting position in perspective mode
        if (cam.orthographic)
        {
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize - zoomSpeed, minZoom, maxZoom);
        }
        else
        {
            targetPosition.y = Mathf.Clamp(cam.transform.position.y - zoomSpeed, minZoom, maxZoom);
        }

        // Calculate mouse world position after zooming to find the offset
        Vector3 mouseWorldAfterZoom = cam.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, Mathf.Abs(cam.transform.position.z)));

        // Apply the offset to the camera position to keep the focus on the mouse pointer
        Vector3 zoomOffset = mouseWorldBeforeZoom - mouseWorldAfterZoom;
        targetPosition += zoomOffset;
    }

    void ZoomOut()
    {
        // Zoom out towards the center of the camera (no pointer-based zoom)
        if (cam.orthographic)
        {
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize + zoomSpeed, minZoom, maxZoom);
        }
        else
        {
            targetPosition.y = Mathf.Clamp(cam.transform.position.y + zoomSpeed, minZoom, maxZoom);
        }
    }

    void HandleEdgeScrolling()
    {
        // Only handle edge scrolling if there's no zooming happening
        if (Input.GetAxis("Mouse ScrollWheel") == 0)
        {
            Vector3 mousePos = Input.mousePosition;

            float screenWidth = Screen.width;
            float screenHeight = Screen.height;

            // Move camera towards the edges (no zooming at the edges)
            if (mousePos.x < edgeThickness)
            {
                targetPosition.x -= edgeScrollSpeed;
            }
            else if (mousePos.x > screenWidth - edgeThickness)
            {
                targetPosition.x += edgeScrollSpeed;
            }

            // Only move the X axis (not Y) for vertical edge scrolling
            if (mousePos.y < edgeThickness)
            {
                targetPosition.z -= edgeScrollSpeed;  // Assuming you're working with 2D, use Z for camera movement
            }
            else if (mousePos.y > screenHeight - edgeThickness)
            {
                targetPosition.z += edgeScrollSpeed;  // Same here, Z is used for 2D movement
            }
        }
    }

    void SmoothMove()
    {
        // Apply camera bounds (clamping the position)
        targetPosition = new Vector3(
            Mathf.Clamp(targetPosition.x, cameraBoundsMin.x, cameraBoundsMax.x),
            Mathf.Clamp(targetPosition.y, cameraBoundsMin.y, cameraBoundsMax.y),
            targetPosition.z
        );

        // Smoothly transition the camera position with Lerp
        cam.transform.position = Vector3.SmoothDamp(cam.transform.position, targetPosition, ref currentVelocity, zoomLerpSpeed * Time.deltaTime);
    }
}
