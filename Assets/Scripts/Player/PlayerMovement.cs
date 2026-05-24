using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float forwardThrustForce = 3500f;
    [SerializeField] float backwardsThrustForce = 3500f;
    [SerializeField] float turnSpeed = 3500f;


    [Header("References")]
    [SerializeField] Transform bodyTransform;
    [SerializeField] Rigidbody2D rb;

    void FixedUpdate()
    {
        if (PlayerControls.Instance.IsThrusting())
        {
            rb.AddRelativeForce(Vector3.up * forwardThrustForce);
        }

        if (PlayerControls.Instance.IsBackwardsThrusting())
        {
            rb.AddRelativeForce(Vector3.down * backwardsThrustForce);
        }

        float rotationInput = PlayerControls.Instance.GetRotationInput();

        if (rotationInput != 0)
        {
            rb.AddTorque(turnSpeed * -rotationInput);
        }

    }
}
