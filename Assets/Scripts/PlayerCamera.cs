using UnityEngine;

[RequireComponent(typeof(Camera))]
public class PlayerCamera : MonoBehaviour
{
    private static PlayerCamera _instance;

    [SerializeField] private float _velocity = 5.0f;
    [SerializeField] private float _cameraSize = 8.0f;
    [SerializeField] private float _distanceOffset = 30.0f;
    [SerializeField] private LayerMask _raycastMask;
    [SerializeField] private Vector3 _rotationAngle;

    private Camera _camera;
    private Transform _target;

    private Vector3 TargetPosition => _target ? _target.position - transform.forward * _distanceOffset : Vector3.zero;

    private const float MinViewportPointRange = -0.25F;
    private const float MaxViewportPointRange = 1.25F;
    private const float MaxRaycastDistance = 1000.0F;

    private void Awake()
    {
        _instance = this;
        _camera = GetComponent<Camera>();
    }

    private void Start()
    {
        _camera.orthographic = true;
        _camera.orthographicSize = _cameraSize;
        _target = PlayerCharacter.Transform;
        transform.rotation = Quaternion.Euler(_rotationAngle);
        transform.position = TargetPosition;
    }

    private void LateUpdate()
    {
        if (!_target) return;
        transform.position = Vector3.Lerp(transform.position, TargetPosition, _velocity * Time.deltaTime);
    }

    public static Vector3 GetCursorWorldPosition()
    {
        if (!_instance) return Vector3.zero;
        Vector3 worldPosition = Vector3.zero;
        Ray ray = _instance._camera.ScreenPointToRay(InputManager.CursorPosition);
        if (Physics.Raycast(ray, out RaycastHit hit, MaxRaycastDistance, _instance._raycastMask))
            worldPosition = hit.collider.CompareTag(GameSettings.EnemyTag) ? hit.transform.position : hit.point;
        return worldPosition;
    }

    public static bool IsCameraSeeTransform(Transform transform)
    {
        if (!_instance) return false;
        Vector3 viewportPoint = _instance._camera.WorldToViewportPoint(transform.position);
        return viewportPoint.x >= MinViewportPointRange && viewportPoint.x <= MaxViewportPointRange
            && viewportPoint.y >= MinViewportPointRange && viewportPoint.y <= MaxViewportPointRange
            && viewportPoint.z > 0.0f;
    }
}
