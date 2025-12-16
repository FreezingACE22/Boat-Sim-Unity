using UnityEngine;

public class BoatOrbitCamera : MonoBehaviour
{
    public Transform boat;      // assign your Boat here
    public float rotationSpeed = 5f;
    public float zoomSpeed = 6f;

    public float minZoom = 5f;
    public float maxZoom = 25f;

    public float minPitch = -20f;
    public float maxPitch = 70f;

    private float currentZoom = 12f;
    private float yaw = 0f;
    private float pitch = 20f;

    void LateUpdate()
    {
        if (boat == null)
            return;

        // --- Rotation with mouse ---
        if (Input.GetMouseButton(0))     // left mouse button drag
        {
            yaw += Input.GetAxis("Mouse X") * rotationSpeed;
            pitch -= Input.GetAxis("Mouse Y") * rotationSpeed;

            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        }

        // --- Zoom ---
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        currentZoom -= scroll * zoomSpeed;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

        // --- Calculate camera position ---
        Quaternion rot = Quaternion.Euler(pitch, yaw, 0);
        Vector3 offset = rot * (Vector3.back * currentZoom);

        Vector3 desiredPos = boat.position + offset;

        transform.position = desiredPos;

        // --- Always look at the boat ---
        transform.LookAt(boat.position, Vector3.up);
    }
}
