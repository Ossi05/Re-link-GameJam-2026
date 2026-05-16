using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float movementSpeed = 12f;

    [Header("References")]
    [SerializeField] Transform bodyTransform;
    [SerializeField] Rigidbody2D rb;

    void FixedUpdate()
    {
        MovePlayer();
    }

    void MovePlayer()
    {
        rb.linearVelocity = PlayerControls.Instance.MoveInput * movementSpeed;
    }
}
