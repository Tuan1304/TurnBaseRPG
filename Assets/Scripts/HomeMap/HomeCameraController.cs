using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Camera))]
public class HomeCameraController : MonoBehaviour
{
    [Header("Move")]
    public float keyboardSpeed = 8f;

    public float dragSpeed = 1f;

    [Header("Zoom")]
    public float zoomSpeed = 4f;

    public float minZoom = 4f;

    public float maxZoom = 9f;

    [Header("Bounds")]
    public Vector2 minPosition;

    public Vector2 maxPosition;

    public bool autoDetectBounds = true;

    private Camera cam;

    private Vector3 lastMouseWorldPosition;

    private bool hasBounds;

    private bool isDragging;

    [RuntimeInitializeOnLoadMethod(
        RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void AutoAttach()
    {
        SceneManager.sceneLoaded +=
            OnSceneLoaded;

        TryAttachToHomeCamera();
    }

    private static void OnSceneLoaded(
        Scene scene,
        LoadSceneMode mode)
    {
        if (scene.name == "HomeScene")
        {
            TryAttachToHomeCamera();
        }
    }

    private static void TryAttachToHomeCamera()
    {
        if (SceneManager.GetActiveScene().name !=
            "HomeScene")
        {
            return;
        }

        Camera mainCamera =
            Camera.main;

        if (mainCamera == null)
            return;

        if (mainCamera.GetComponent<HomeCameraController>() ==
            null)
        {
            mainCamera.gameObject
                .AddComponent<HomeCameraController>();
        }
    }

    private void Awake()
    {
        cam =
            GetComponent<Camera>();
    }

    private void Start()
    {
        if (autoDetectBounds)
        {
            DetectBounds();
        }

        ClampCamera();
    }

    private void Update()
    {
        HandleKeyboardMove();

        HandleMouseDrag();

        HandleZoom();

        ClampCamera();
    }

    private void HandleKeyboardMove()
    {
        Vector3 input =
            new Vector3(
                Input.GetAxisRaw("Horizontal"),
                Input.GetAxisRaw("Vertical"),
                0f);

        if (input.sqrMagnitude <= 0f)
            return;

        transform.position +=
            input.normalized *
            keyboardSpeed *
            Time.deltaTime;
    }

    private void HandleMouseDrag()
    {
        if (Input.GetMouseButtonDown(1) ||
            Input.GetMouseButtonDown(2))
        {
            if (IsPointerOverUI())
                return;

            isDragging = true;

            lastMouseWorldPosition =
                GetMouseWorldPosition();
        }

        if (!Input.GetMouseButton(1) &&
            !Input.GetMouseButton(2))
        {
            isDragging = false;
        }

        if (!isDragging)
            return;

        Vector3 currentMouseWorldPosition =
            GetMouseWorldPosition();

        Vector3 delta =
            lastMouseWorldPosition -
            currentMouseWorldPosition;

        transform.position +=
            delta * dragSpeed;

        lastMouseWorldPosition =
            GetMouseWorldPosition();
    }

    private void HandleZoom()
    {
        float scroll =
            Input.mouseScrollDelta.y;

        if (Mathf.Approximately(scroll, 0f))
            return;

        cam.orthographicSize =
            Mathf.Clamp(
                cam.orthographicSize -
                (scroll * zoomSpeed * Time.deltaTime),
                minZoom,
                maxZoom);
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePosition =
            Input.mousePosition;

        mousePosition.z =
            -transform.position.z;

        return cam.ScreenToWorldPoint(
            mousePosition);
    }

    private bool IsPointerOverUI()
    {
        return EventSystem.current != null &&
            EventSystem.current.IsPointerOverGameObject();
    }

    private void DetectBounds()
    {
        Renderer[] renderers =
            Object.FindObjectsByType<Renderer>(
                FindObjectsInactive.Exclude,
                FindObjectsSortMode.None);

        bool foundBounds = false;

        Bounds bounds =
            new Bounds();

        foreach (Renderer renderer
            in renderers)
        {
            if (renderer == null ||
                renderer.gameObject.layer ==
                LayerMask.NameToLayer("UI"))
            {
                continue;
            }

            if (!renderer.GetComponent<UnityEngine.Tilemaps.TilemapRenderer>())
            {
                continue;
            }

            if (!foundBounds)
            {
                bounds = renderer.bounds;

                foundBounds = true;
            }
            else
            {
                bounds.Encapsulate(
                    renderer.bounds);
            }
        }

        if (!foundBounds)
            return;

        minPosition =
            bounds.min;

        maxPosition =
            bounds.max;

        hasBounds = true;
    }

    private void ClampCamera()
    {
        if (!hasBounds &&
            !autoDetectBounds)
        {
            hasBounds = true;
        }

        if (!hasBounds)
            return;

        float verticalExtent =
            cam.orthographicSize;

        float horizontalExtent =
            verticalExtent * cam.aspect;

        float minX =
            minPosition.x + horizontalExtent;

        float maxX =
            maxPosition.x - horizontalExtent;

        float minY =
            minPosition.y + verticalExtent;

        float maxY =
            maxPosition.y - verticalExtent;

        if (minX > maxX)
        {
            float centerX =
                (minPosition.x + maxPosition.x) * 0.5f;

            minX = centerX;
            maxX = centerX;
        }

        if (minY > maxY)
        {
            float centerY =
                (minPosition.y + maxPosition.y) * 0.5f;

            minY = centerY;
            maxY = centerY;
        }

        Vector3 position =
            transform.position;

        position.x =
            Mathf.Clamp(
                position.x,
                minX,
                maxX);

        position.y =
            Mathf.Clamp(
                position.y,
                minY,
                maxY);

        transform.position =
            position;
    }
}
