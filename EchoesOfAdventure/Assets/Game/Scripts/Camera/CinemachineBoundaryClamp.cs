using UnityEngine;
using Unity.Cinemachine;

[ExecuteAlways]
[SaveDuringPlay]
[AddComponentMenu("Cinemachine/Extensions/Boundary Clamp")]
public class CinemachineBoundaryClamp : CinemachineExtension
{
    [Tooltip("Define the boundaries for the camera (world space).")]
    public Bounds levelBounds;

    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage,
        ref CameraState state,
        float deltaTime)
    {
        if (stage == CinemachineCore.Stage.Body)
        {
            // Get the raw position of the camera
            Vector3 position = state.RawPosition;

            // Clamp the position within the bounds
            position.x = Mathf.Clamp(position.x, levelBounds.min.x, levelBounds.max.x);
            position.y = Mathf.Clamp(position.y, levelBounds.min.y, levelBounds.max.y);

            // Update the camera state with the clamped position
            state.RawPosition = position;
        }
    }

    private void OnDrawGizmos()
    {
        // Draw the bounds in the Scene view
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(levelBounds.center, levelBounds.size);
    }
}
