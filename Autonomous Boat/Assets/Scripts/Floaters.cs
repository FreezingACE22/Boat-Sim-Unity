using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class Floater : MonoBehaviour
{
    [Header("References")]
    public Rigidbody boat;                 // Boat Rigidbody
    public WaterSurface water;             // HDRP WaterSurface

    [Header("Buoyancy Settings")]
    [Tooltip("Depth where full buoyant force is reached.")]
    public float depthBefSub = 0.6f;

    [Tooltip("Buoyancy strength multiplier (per floater).")]
    public float displacementAmt = 1f;

    [Tooltip("Total number of floaters on the boat.")]
    public int floaters = 4;

    [Header("Water Drag")]
    [Tooltip("Linear drag applied to horizontal motion while submerged.")]
    public float waterDrag = 0.8f;

    [Tooltip("Extra drag applied to vertical motion while submerged (reduces bouncing).")]
    public float verticalWaterDrag = 3f;

    [Tooltip("Angular drag while submerged.")]
    public float waterAngularDrag = 0.8f;

    [Header("Debug")]
    public bool logDepth = false;
    public bool drawForces = false;

    private WaterSearchParameters search;
    private WaterSearchResult searchResult;

    void FixedUpdate()
    {
        if (!boat || !water) return;

        // Distributed gravity (so N floaters = total gravity)
        // Assumes boat.useGravity = false
        boat.AddForceAtPosition(Physics.gravity / Mathf.Max(1, floaters),
                                transform.position,
                                ForceMode.Acceleration);

        // Get water height at floater position
        search.startPositionWS = transform.position;
        water.ProjectPointOnWaterSurface(search, out searchResult);
        float waterY = searchResult.projectedPositionWS.y;

        float depth = waterY - transform.position.y;

        if (logDepth)
            Debug.Log($"{name}: depth={depth:F3}, waterY={waterY:F3}");

        if (depth > 0f)
        {
            // Amount of displacement based on submersion
            float disp = Mathf.Clamp01(depth / Mathf.Max(0.001f, depthBefSub)) * displacementAmt;

            // --- Buoyancy (acts like a spring) ---
            Vector3 buoyancy = Vector3.up * Mathf.Abs(Physics.gravity.y) * disp;
            boat.AddForceAtPosition(buoyancy, transform.position, ForceMode.Acceleration);

            if (drawForces)
                Debug.DrawRay(transform.position, buoyancy * 0.02f, Color.green, 0.1f, false);

            // --- Linear drag ---

            // Velocity at the floater point (more accurate than center of mass)
            Vector3 pointVel = boat.GetPointVelocity(transform.position);

            // Horizontal damping (X/Z)
            Vector3 horizV = Vector3.ProjectOnPlane(pointVel, Vector3.up);
            Vector3 horizDragForce = -horizV * waterDrag * disp;
            boat.AddForce(horizDragForce, ForceMode.Acceleration);

            // Vertical damping (Y) – this is what kills the bounciness
            Vector3 verticalV = Vector3.Project(pointVel, Vector3.up);
            Vector3 verticalDragForce = -verticalV * verticalWaterDrag * disp;
            boat.AddForce(verticalDragForce, ForceMode.Acceleration);

            // --- Angular drag ---
            Vector3 w = boat.angularVelocity;
            Vector3 yaw = Vector3.Project(w, boat.transform.up);
            Vector3 rollPitch = w - yaw;

            Vector3 angularDamping = -(yaw + rollPitch) * waterAngularDrag * disp;
            boat.AddTorque(angularDamping, ForceMode.Acceleration);
        }
    }
}
