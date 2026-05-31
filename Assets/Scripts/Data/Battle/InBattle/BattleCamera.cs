using System.Collections;
using UnityEngine;

public class BattleCamera : MonoBehaviour
{
    public static BattleCamera Instance;

    [Header("Move")]
    public float moveSpeed = 10f;

    [Header("Drag")]
    public float dragSpeed = 0.02f;

    [Header("Zoom")]
    public float zoomSpeed = 5f;

    [Header("Bounds")]
    public Vector2 minBounds;

    public Vector2 maxBounds;

    public float minZoom = 3f;

    public float maxZoom = 10f;

    [Header("Follow")]
    public float followSpeed = 5f;

    [Header("Combat Zoom")]
    public float combatZoomSize = 4f;

    private Camera cam;

    private bool isLocked;

    private Transform followTarget;

    private Vector3 lastMousePos;

    private float defaultZoom;

    private void Awake()
    {
        Instance = this;

        cam = GetComponent<Camera>();

        defaultZoom =
            cam.orthographicSize;
    }

    private void Update()
    {
        // =====================
        // FOLLOW TARGET
        // =====================

        if (followTarget != null)
        {
            Vector3 targetPos =
                new Vector3(
                    followTarget.position.x,
                    followTarget.position.y,
                    transform.position.z);

            transform.position =
                Vector3.Lerp(
                    transform.position,
                    targetPos,
                    followSpeed *
                    Time.deltaTime);
        }

        // =====================
        // LOCK
        // =====================

        if (isLocked)
            return;

        HandleKeyboardMove();

        HandleMouseDrag();

        HandleZoom();

        ClampPosition();
    }

    // =========================
    // FOLLOW
    // =========================

    public void StartFollow(
        Transform target)
    {
        followTarget = target;
    }

    public void StopFollow()
    {
        followTarget = null;
    }

    // =========================
    // LOCK
    // =========================

    public void SetLocked(
        bool value)
    {
        isLocked = value;
    }

    // =========================
    // KEYBOARD MOVE
    // =========================

    private void HandleKeyboardMove()
    {
        Vector3 move =
            Vector3.zero;

        if (Input.GetKey(KeyCode.W))
            move.y += 1;

        if (Input.GetKey(KeyCode.S))
            move.y -= 1;

        if (Input.GetKey(KeyCode.A))
            move.x -= 1;

        if (Input.GetKey(KeyCode.D))
            move.x += 1;

        transform.position +=
            move *
            moveSpeed *
            Time.deltaTime;
    }

    // =========================
    // MOUSE DRAG
    // =========================

    private void HandleMouseDrag()
    {
        if (Input.GetMouseButtonDown(1))
        {
            lastMousePos =
                Input.mousePosition;
        }

        if (Input.GetMouseButton(1))
        {
            Vector3 delta =
                Input.mousePosition
                - lastMousePos;

            Vector3 move =
                new Vector3(
                    -delta.x * dragSpeed,
                    -delta.y * dragSpeed,
                    0f);

            transform.position += move;

            lastMousePos =
                Input.mousePosition;
        }
    }

    // =========================
    // ZOOM
    // =========================

    private void HandleZoom()
    {
        float scroll =
            Input.mouseScrollDelta.y;

        cam.orthographicSize -=
            scroll * zoomSpeed *
            Time.deltaTime *
            100f;

        cam.orthographicSize =
            Mathf.Clamp(
                cam.orthographicSize,
                minZoom,
                maxZoom);
    }

    // =========================
    // FOCUS TARGET
    // =========================

    public IEnumerator FocusTarget(
        Transform target)
    {
        float timer = 0f;

        float duration = 0.3f;

        Vector3 startPos =
            transform.position;

        Vector3 targetPos =
            new Vector3(
                target.position.x,
                target.position.y,
                transform.position.z);

        while (timer < duration)
        {
            timer += Time.deltaTime;

            transform.position =
                Vector3.Lerp(
                    startPos,
                    targetPos,
                    timer / duration);

            yield return null;
        }
    }

    // =========================
    // ZOOM IN
    // =========================

    public IEnumerator ZoomIn()
    {
        float timer = 0f;

        float duration = 0.25f;

        float startZoom =
            cam.orthographicSize;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            cam.orthographicSize =
                Mathf.Lerp(
                    startZoom,
                    combatZoomSize,
                    timer / duration);

            yield return null;
        }
    }

    // =========================
    // RESET CAMERA
    // =========================

    public IEnumerator ResetCamera()
    {
        float timer = 0f;

        float duration = 0.25f;

        float startZoom =
            cam.orthographicSize;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            cam.orthographicSize =
                Mathf.Lerp(
                    startZoom,
                    defaultZoom,
                    timer / duration);

            yield return null;
        }
    }

    private void ClampPosition()
    {
        Vector3 pos =
            transform.position;

        pos.x =
            Mathf.Clamp(
                pos.x,
                minBounds.x,
                maxBounds.x);

        pos.y =
            Mathf.Clamp(
                pos.y,
                minBounds.y,
                maxBounds.y);

        transform.position =
            pos;
    }

    // =========================
    // SHAKE
    // =========================

    public IEnumerator Shake()
    {
        Vector3 originalPos =
            transform.position;

        float duration = 0.15f;

        float strength = 0.15f;

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            transform.position =
                originalPos +
                (Vector3)
                Random.insideUnitCircle *
                strength;

            yield return null;
        }

        transform.position =
            originalPos;
    }
}