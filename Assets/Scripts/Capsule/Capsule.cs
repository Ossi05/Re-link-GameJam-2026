using UnityEngine;

public class Capsule : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float outwardDriftForce = 10f;
    [SerializeField] float playerCableConnectedDriftForce = 4f;

    [Header("References")]
    [SerializeField] CableAttachPoint capsuleCableAttachPoint;

    Rigidbody2D rb;
    float originMass;
    bool isPlayerCableConnected;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        originMass = rb.mass;
    }

    void Start()
    {
        PlayerCableController.Instance.OnAttachedCableChanged += PlayerCableController_OnAttachedCableChanged;
    }

    void PlayerCableController_OnAttachedCableChanged(object sender, CableAttachPoint.OnConnectedCableChangedEventArgs e)
    {
        Cable attachedCable = e.attachedCable;
        if (attachedCable?.GetTowablePoint() == capsuleCableAttachPoint)
        {
            float towedMass = 1f;
            rb.mass = towedMass;
            isPlayerCableConnected = true;
        }
        else
        {
            rb.mass = originMass;
            isPlayerCableConnected = false;
        }
    }

    void FixedUpdate()
    {
        // Calculate direction pointing directly away from the hub
        Vector2 directionAwayFromHub = (transform.position - LifeSupportHub.Instance.transform.position).normalized;

        // Apply the constant force
        rb.AddForce(directionAwayFromHub * (isPlayerCableConnected ? playerCableConnectedDriftForce : outwardDriftForce), ForceMode2D.Force);
    }

}
