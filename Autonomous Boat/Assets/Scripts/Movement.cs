using UnityEngine;

public class BoatDifferentialArrows : MonoBehaviour
{
    public Rigidbody Boat;

    [Header("Motors")]
    public Transform leftMotor;   // left side of boat
    public Transform rightMotor;  // right side of boat

    [Header("Thrust Settings")]
    public float maxThrustPerMotor = 50f;    // adjust to taste
    public float inPlaceTurnTorque = 30f;    // for turning with only left/right

    void FixedUpdate()
    {
        // --- 1) Read arrow keys ---
        bool forwardKey = Input.GetKey(KeyCode.UpArrow);
        bool backwardKey = Input.GetKey(KeyCode.DownArrow);
        bool leftKey = Input.GetKey(KeyCode.LeftArrow);
        bool rightKey = Input.GetKey(KeyCode.RightArrow);

        int forwardDir = 0; // -1 = backward, 0 = none, 1 = forward
        if (forwardKey) forwardDir += 1;
        if (backwardKey) forwardDir -= 1;

        int turnDir = 0; // -1 = left, 0 = none, 1 = right
        if (rightKey) turnDir += 1;
        if (leftKey) turnDir -= 1;

        float leftPower = 0f;   // -1..1 (multiplier)
        float rightPower = 0f;

        // --- 2) Movement logic ---

        if (forwardDir != 0)
        {
            // We are moving forward or backward
            float basePower = forwardDir; // +1 or -1

            if (turnDir == 0)
            {
                // Straight: both motors same power
                leftPower = basePower;
                rightPower = basePower;
            }
            else if (turnDir > 0)
            {
                // Turning RIGHT with forward/back:
                //   outer (left) motor ON, inner (right) motor OFF
                leftPower = basePower; // outer
                rightPower = 0f;        // inner
            }
            else // turnDir < 0
            {
                // Turning LEFT with forward/back:
                //   outer (right) motor ON, inner (left) motor OFF
                rightPower = basePower; // outer
                leftPower = 0f;        // inner
            }
        }
        else
        {
            // No Up/Down pressed
            // You said you want arrows: left/right also do something,
            // so we’ll allow **in-place turning** using torque.
            if (turnDir != 0)
            {
                Boat.AddTorque(Vector3.up * (turnDir * inPlaceTurnTorque), ForceMode.Force);
            }
        }

        // --- 3) Apply forces at motor positions ---

        Vector3 forwardDirWorld = Boat.transform.forward;

        if (leftMotor != null && Mathf.Abs(leftPower) > 0.001f)
        {
            Vector3 leftForce = forwardDirWorld * (leftPower * maxThrustPerMotor);
            Boat.AddForceAtPosition(leftForce, leftMotor.position, ForceMode.Force);
        }

        if (rightMotor != null && Mathf.Abs(rightPower) > 0.001f)
        {
            Vector3 rightForce = forwardDirWorld * (rightPower * maxThrustPerMotor);
            Boat.AddForceAtPosition(rightForce, rightMotor.position, ForceMode.Force);
        }
    }
}
