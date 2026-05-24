using System;
using UnityEngine;

public class Capsule : MonoBehaviour
{

    [Header("Settings")]
    [SerializeField] float oxygenDepletionRatePerSecond = 5f;
    [SerializeField] float oxygenRefillRatePerSecond = 5f;
    [SerializeField] float outwardDriftForce = 10f;
    [SerializeField] float playerCableConnectedDriftForce = 4f;

    [Header("References")]
    [SerializeField] CableAttachPoint capsuleCableAttachPoint;

    Oxygen oxygen;
    Rigidbody2D rb;
    float originMass;

    bool isAlive = true;

    public event EventHandler OnDeath;

    public enum ConnectionState
    {
        Disconnected,
        ConnectedToHub,
        ConnectedToPlayer,
        ConnectedToOther
    }
    ConnectionState currentConnectionState = ConnectionState.Disconnected;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        originMass = rb.mass;
        oxygen = GetComponent<Oxygen>();
    }

    void OnEnable()
    {
        oxygen.OnOutOfOxygen += Oxygen_OnOutOfOxygen;
        capsuleCableAttachPoint.OnConnectedCableChanged += HandleConnectionChanged;
        capsuleCableAttachPoint.OnCableEndPointChanged += HandleConnectionChanged;
    }

    void Oxygen_OnOutOfOxygen(object sender, System.EventArgs e)
    {
        if (!isAlive) return;
        isAlive = false;
        capsuleCableAttachPoint.Disable();
        OnDeath?.Invoke(this, EventArgs.Empty);
    }

    void HandleConnectionChanged(object sender, CableAttachPoint.OnConnectedCableChangedEventArgs e)
    {

        Cable attachedCable = e.attachedCable;
        if (attachedCable == null)
        {
            UpdateConnectionState(ConnectionState.Disconnected);
            return;
        }

        CableAttachPoint anchor = attachedCable.GetAnchorPoint();
        switch (anchor.GetOwnerType())
        {
            case CableAttachPoint.OwnerType.LifeSupportHub:
                UpdateConnectionState(ConnectionState.ConnectedToHub);
                break;
            case CableAttachPoint.OwnerType.Player:
                UpdateConnectionState(ConnectionState.ConnectedToPlayer);
                break;
            default:
                UpdateConnectionState(ConnectionState.ConnectedToOther);
                break;
        }
    }

    void Update()
    {
        if (!isAlive) return;
        if (currentConnectionState != ConnectionState.ConnectedToHub)
        {
            ConsumeOxygen();
        }
        else
        {
            if (LifeSupportHub.Instance.HasOxygen())
            {
                if (oxygen.GetCurrentOxygenLevel() < oxygen.GetMaxOxygen())
                {
                    float oxygenAmt = LifeSupportHub.Instance.GetOxygen(oxygenRefillRatePerSecond * Time.deltaTime);
                    oxygen.Refill(oxygenAmt);
                }
            }
            else
            {
                ConsumeOxygen();
            }
        }
    }

    void ConsumeOxygen()
    {
        oxygen.Consume(oxygenDepletionRatePerSecond * Time.deltaTime);
    }

    void FixedUpdate()
    {
        // Calculate direction pointing directly away from the hub
        Vector2 directionAwayFromHub = (transform.position - LifeSupportHub.Instance.transform.position).normalized;

        // Calculate the force to apply
        float appliedForce = (currentConnectionState == ConnectionState.ConnectedToPlayer)
            ? playerCableConnectedDriftForce
            : outwardDriftForce;

        // Apply the constant force
        rb.AddForce(directionAwayFromHub * appliedForce, ForceMode2D.Force);
    }

    void UpdateConnectionState(ConnectionState newState)
    {
        if (!isAlive) return;
        currentConnectionState = newState;

        if (currentConnectionState == ConnectionState.ConnectedToPlayer)
        {
            float towedMass = 1f;
            rb.angularVelocity = 0f;
            rb.mass = towedMass;
        }
        else
        {
            rb.mass = originMass;
        }
    }


    void OnDestroy()
    {
        capsuleCableAttachPoint.OnConnectedCableChanged -= HandleConnectionChanged;
        capsuleCableAttachPoint.OnCableEndPointChanged -= HandleConnectionChanged;
        oxygen.OnOutOfOxygen -= Oxygen_OnOutOfOxygen;
    }

}
