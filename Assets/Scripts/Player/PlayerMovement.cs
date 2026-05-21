using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float thrustForce = 1000f;
    [SerializeField] float turnSpeed = 1000f;


    [Header("References")]
    [SerializeField] Transform bodyTransform;
    [SerializeField] Rigidbody2D rb;

    void FixedUpdate()
    {
        if (PlayerControls.Instance.IsThrusting())
        {
            rb.AddRelativeForce(Vector3.up * thrustForce * Time.fixedDeltaTime);
        }

        float rotationInput = PlayerControls.Instance.GetRotationInput();

        if (rotationInput != 0)
        {
            rb.AddTorque(turnSpeed * -rotationInput * Time.fixedDeltaTime);
        }

    }
}
