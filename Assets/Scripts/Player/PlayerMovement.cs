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

    public bool IsThrustingUp => GameManager.Instance.IsGamePlaying() && PlayerControls.Instance.IsThrusting();
    public bool IsThrustingDown => GameManager.Instance.IsGamePlaying() && PlayerControls.Instance.IsBackwardsThrusting();
    public bool IsTurningLeft => GameManager.Instance.IsGamePlaying() && PlayerControls.Instance.GetRotationInput() < 0f;
    public bool IsTurningRight => GameManager.Instance.IsGamePlaying() && PlayerControls.Instance.GetRotationInput() > 0f;

    void FixedUpdate()
    {
        if (!GameManager.Instance.IsGamePlaying()) return;

        // Up
        if (PlayerControls.Instance.IsThrusting())
        {
            rb.AddRelativeForce(Vector3.up * forwardThrustForce);
        }

        // Down
        if (PlayerControls.Instance.IsBackwardsThrusting())
        {
            rb.AddRelativeForce(Vector3.down * backwardsThrustForce);
        }

        // Left/Right
        float rotationInput = PlayerControls.Instance.GetRotationInput();
        if (rotationInput != 0)
        {
            rb.AddTorque(turnSpeed * -rotationInput);
        }
    }
}
