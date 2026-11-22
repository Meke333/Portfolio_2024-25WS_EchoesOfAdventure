using System;
using System.Collections;
using Events;
using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using Zenject;

public class CameraController : MonoBehaviour
{
    [Inject] private ProjectEvents _projectEvents;
    private CinemachineCamera _cinemachineCamera;
    private CinemachineFollow _cinemachineFollow;

    [Header("Camera Properties")] 
    [SerializeField] private float normalFOV = 60f; // Default FOV
    [SerializeField] private float normalZOffset = -10f; // Default Z-offset
    [SerializeField] private Vector3 cameraOffset; // Base offset (X, Y)

    [Header("Zoom Settings")]
    [SerializeField] private float zoomSpeed = 5f; // Speed of zoom transition
    [SerializeField] private float zoomedFOV = 40f; // FOV when zoomed in
    [SerializeField] private float zoomedZOffset = -5f; // Z-offset when zoomed in
    
    private bool _isZoomedOut = false;
    private Coroutine zoomCoroutine; // Stores the ongoing zoom coroutine

    [Header("Boundary Settings")]
    private Transform _characterTransform; // Reference to the player character's transform

    private void OnEnable()
    {
        _projectEvents.SetCameraTarget += SetCameraTarget;
        InputSystem.actions["Zoom"].performed += ToggleZoom;
    }

    private void OnDisable()
    {
        _projectEvents.SetCameraTarget -= SetCameraTarget;
        InputSystem.actions["Zoom"].performed -= ToggleZoom;
    }

    private void Awake()
    {
        _cinemachineFollow = GetComponent<CinemachineFollow>();
        _cinemachineCamera = GetComponent<CinemachineCamera>();
        
        if (_cinemachineFollow == null || _cinemachineCamera == null)
        {
            Debug.LogError("Missing Cinemachine components! Ensure CinemachineFollow and CinemachineCamera are attached.");
            return;
        }
        
        // Apply Initial Camera Values
        _cinemachineFollow.FollowOffset = new Vector3(cameraOffset.x, cameraOffset.y, normalZOffset);
        _cinemachineCamera.Lens.FieldOfView = normalFOV; // Default FOV
    }
    
    private void SetCameraTarget(GameObject target)
    {
        if (_cinemachineCamera != null)
        {
            _cinemachineCamera.Target.TrackingTarget = target.transform;
            _characterTransform = target.transform;
        }
    }
    
    // Method to toggle zoom with smooth transition
    private void ToggleZoom(InputAction.CallbackContext obj)
    {
        // Stop any ongoing zoom coroutine
        if (zoomCoroutine != null)
        {
            StopCoroutine(zoomCoroutine);
        }

        // Start a new zoom coroutine with appropriate target values
        float targetFOV = _isZoomedOut ? normalFOV : zoomedFOV;
        float targetZOffset = _isZoomedOut ? normalZOffset : zoomedZOffset;
        zoomCoroutine = StartCoroutine(SmoothZoom(targetFOV, targetZOffset));

        // Toggle zoom state
        _isZoomedOut = !_isZoomedOut;
    }

    // Coroutine to smoothly transition the FOV and Z-offset
    private IEnumerator SmoothZoom(float targetFOV, float targetZOffset)
    {
        float startFOV = _cinemachineCamera.Lens.FieldOfView;
        float startZOffset = _cinemachineFollow.FollowOffset.z;

        float elapsedTime = 0f;

        while (Mathf.Abs(_cinemachineCamera.Lens.FieldOfView - targetFOV) > 0.01f ||
               Mathf.Abs(_cinemachineFollow.FollowOffset.z - targetZOffset) > 0.01f)
        {
            elapsedTime += Time.deltaTime * zoomSpeed;

            // Smoothly interpolate FOV and Z-offset
            _cinemachineCamera.Lens.FieldOfView = Mathf.Lerp(startFOV, targetFOV, elapsedTime);
            _cinemachineFollow.FollowOffset = new Vector3(cameraOffset.x, cameraOffset.y, Mathf.Lerp(startZOffset, targetZOffset, elapsedTime));

            yield return null; // Wait for the next frame
        }

        // Ensure the final values are exactly the target
        _cinemachineCamera.Lens.FieldOfView = targetFOV;
        _cinemachineFollow.FollowOffset = new Vector3(cameraOffset.x, cameraOffset.y, targetZOffset);
    }
}
