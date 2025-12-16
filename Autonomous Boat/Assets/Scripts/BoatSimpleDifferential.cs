using UnityEngine;

public class BoatSimpleDifferential : MonoBehaviour
{
    public Rigidbody rb;

    [Header("Thrust & Turning")]
    public float maxForwardThrust = 150f;      // straight full speed
    public float movingTurnTorque = 60f;       // turning while moving
    public float inPlaceTurnTorque = 40f;      // turning when only left/right pressed

    void FixedUpdate()
    {
        // --- Read arrow keys ---
        int throttle = 0;  // -1 = back, 0 = none, 1 = forward
        int steer = 0;  // -1 = left, 0 = none, 1 = right

        if (Input.GetKey(KeyCode.UpArrow)) throttle += 1;
        if (Input.GetKey(KeyCode.DownArrow)) throttle -= 1;

        if (Input.GetKey(KeyCode.RightArrow)) steer += 1;
        if (Input.GetKey(KeyCode.LeftArrow)) steer -= 1;

        // Nothing pressed
        if (throttle == 0 && steer == 0)
            return;

        // --- Movement ---
        // Forward/backwards force
        if (throttle != 0)
        {
            // If we are turning, only "one motor" is considered active,
            // so we give it ~half the straight-line thrust.
            float activeMotorFactor = (steer == 0) ? 1f : 0.5f;

            float thrust = throttle * maxForwardThrust * activeMotorFactor;

            // Use local +X as "forward"
            Vector3 boatForward = transform.right;

            rb.AddForce(boatForward * thrust, ForceMode.Force);

            // Add turning torque if steering at the same time
            if (steer != 0)
            {
                float torque = throttle * steer * movingTurnTorque;
                rb.AddTorque(Vector3.up * torque, ForceMode.Force);
            }
        }
        else
        {
            // No Up/Down, only Left/Right -> rotate in place
            if (steer != 0)
            {
                float torque = steer * inPlaceTurnTorque;
                rb.AddTorque(Vector3.up * torque, ForceMode.Force);
            }
        }
    }
}
