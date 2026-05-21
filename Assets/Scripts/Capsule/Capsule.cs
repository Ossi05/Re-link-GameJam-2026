using UnityEngine;

public class Capsule : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float outwardDriftForce = 10f;

    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }


    void FixedUpdate()
    {
        // Calculate direction pointing directly away from the hub
        Vector2 directionAwayFromHub = (transform.position - LifeSupportHub.Instance.transform.position).normalized;

        // Apply the constant force
        rb.AddForce(directionAwayFromHub * outwardDriftForce);
    }

    void Update()
    {

    }
}
