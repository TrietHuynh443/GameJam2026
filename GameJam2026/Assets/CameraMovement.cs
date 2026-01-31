using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollowDampedBoundsFOV : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Follow Settings")]
    public float dampTime = 0.25f;
    public Vector3 offset = new Vector3(0, 0, -10);

    [Header("Camera Bounds (World Space)")]
    public Vector2 minBounds;
    public Vector2 maxBounds;

    [Tooltip("Z position of the gameplay plane (usually 0)")]
    public float worldPlaneZ = 0f;

    private Vector3 _velocity;
    private Camera _camera;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        if (!target) return;

        Vector3 desired = target.position + offset;

        Vector3 smooth = Vector3.SmoothDamp(
            transform.position,
            desired,
            ref _velocity,
            dampTime
        );

        transform.position = ClampToBounds(smooth);
    }

    private Vector3 ClampToBounds(Vector3 position)
    {
        float halfHeight = _camera.orthographicSize;
        float halfWidth = halfHeight * _camera.aspect;

        position.x = Mathf.Clamp(
            position.x,
            minBounds.x + halfWidth,
            maxBounds.x - halfWidth
        );

        position.y = Mathf.Clamp(
            position.y,
            minBounds.y + halfHeight,
            maxBounds.y - halfHeight
        );

        position.z = offset.z;
        return position;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 center = (minBounds + maxBounds) * 0.5f;
        Vector3 size = maxBounds - minBounds;
        Gizmos.DrawWireCube(center, size);
    }
#endif
}
