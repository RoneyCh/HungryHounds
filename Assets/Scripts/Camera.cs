using UnityEngine;

public class Camera : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    public float smoothSpeed = 0.125f;
    public float maxYValue;

    private void LateUpdate()
    {
        if (target != null)
        {
            Vector3 desiredPosition = target.position + offset;

            // Clamp the Y position to not go below maxYValue
            float clampedY = Mathf.Max(desiredPosition.y, maxYValue);

            // Smooth movement
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, new Vector3(desiredPosition.x, clampedY, transform.position.z), smoothSpeed);

            // Apply the new position
            transform.position = smoothedPosition;
        }
    }
}
