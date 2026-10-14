using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Follow Settings")]
    public float smoothSpeed = 5f;
    public Vector3 offset = new Vector3(0f, 0f, -10f);

    [Header("Camera Bounds")]
    public Vector2 minBounds;
    public Vector2 maxBounds;

    private float camHalfHeight;
    private float camHalfWidth;

    private void Start()
    {
        Camera cam = GetComponent<Camera>();

        camHalfHeight = cam.orthographicSize;
        camHalfWidth = camHalfHeight * cam.aspect;
    }

    private void LateUpdate()
    {
        if (!target) return;

        Vector3 desiredPosition = target.position + offset;

        Vector3 smoothedPosition = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed * Time.deltaTime
        );

        // Clamp camera position to bounds
        float clampedX = Mathf.Clamp(
            smoothedPosition.x,
            minBounds.x + camHalfWidth,
            maxBounds.x - camHalfWidth
        );

        float clampedY = Mathf.Clamp(
            smoothedPosition.y,
            minBounds.y + camHalfHeight,
            maxBounds.y - camHalfHeight
        );

        transform.position = new Vector3(clampedX, clampedY, smoothedPosition.z);
    }
}