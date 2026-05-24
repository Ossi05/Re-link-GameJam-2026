using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineCamera))]
public class DynamicCameraZoom : MonoBehaviour
{
    [Header("Zoom Settings")]
    [SerializeField] private float minOrthoSize = 20f;
    [SerializeField] private float maxOrthoSize = 30f;
    [SerializeField] private float zoomSpeed = 3f;

    [Header("Bounds Settings")]
    [Tooltip("How close the player can get to the screen edge before the camera starts zooming out.")]
    [SerializeField] private float edgeMargin = 2f;

    [Header("References")]
    [SerializeField] private Transform target;

    private CinemachineCamera cmCamera;
    private Camera mainCamera;

    private void Awake()
    {
        cmCamera = GetComponent<CinemachineCamera>();
        mainCamera = Camera.main;
    }

    private void LateUpdate()
    {
        if (target == null || mainCamera == null) return;

        // 1. Calculate the absolute world distance from the camera center to the player
        float distanceX = Mathf.Abs(target.position.x - mainCamera.transform.position.x);
        float distanceY = Mathf.Abs(target.position.y - mainCamera.transform.position.y);

        // 2. Calculate the required Orthographic Size to keep the player visible.
        float requiredZoomY = distanceY + edgeMargin;
        float requiredZoomX = (distanceX + edgeMargin) / mainCamera.aspect;

        // 3. Find the largest required zoom
        float targetZoom = Mathf.Max(minOrthoSize, Mathf.Max(requiredZoomY, requiredZoomX));
        targetZoom = Mathf.Min(targetZoom, maxOrthoSize);

        // 4. Apply the smooth zoom
        LensSettings lens = cmCamera.Lens;

        lens.OrthographicSize = Mathf.Lerp(
            lens.OrthographicSize,
            targetZoom,
            Time.deltaTime * zoomSpeed
        );

        cmCamera.Lens = lens;
    }
}