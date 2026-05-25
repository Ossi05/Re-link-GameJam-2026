using System;
using UnityEngine;

public class Capsule : MonoBehaviour
{
    public static event EventHandler OnAnyDeath;
    public event EventHandler OnDeath;
    public event EventHandler OnCableDisconnected;

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
    CableConnectionType connectedToType = CableConnectionType.None;


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        oxygen = GetComponent<Oxygen>();
        originMass = rb.mass;
    }

    void OnEnable()
    {
        if (oxygen == null)
        {
            Debug.LogError("Oxygen component not found on Capsule.");
            return;
        }
        oxygen.OnOutOfOxygen += Oxygen_OnOutOfOxygen;
        capsuleCableAttachPoint.OnConnectionChanged += CapsuleCableAttachPoint_OnConnectionChanged;
    }

    void OnDisable()
    {
        oxygen.OnOutOfOxygen -= Oxygen_OnOutOfOxygen;
        capsuleCableAttachPoint.OnConnectionChanged -= CapsuleCableAttachPoint_OnConnectionChanged;
    }

    void Oxygen_OnOutOfOxygen(object sender, System.EventArgs e)
    {
        if (!isAlive) return;
        isAlive = false;
        capsuleCableAttachPoint.Disable();
        OnDeath?.Invoke(this, EventArgs.Empty);
        OnAnyDeath?.Invoke(this, EventArgs.Empty);
    }

    void CapsuleCableAttachPoint_OnConnectionChanged(object sender, CableAttachPoint.OnConnectionChangedEventArgs e)
    {
        CableAttachPoint connectedPoint = e.connectedPoint;
        if (connectedPoint == null)
        {
            UpdateConnectionState(CableConnectionType.None);
            OnCableDisconnected?.Invoke(this, EventArgs.Empty);
            return;
        }

        UpdateConnectionState(connectedPoint.GetPointType());
    }

    void UpdateConnectionState(CableConnectionType newType)
    {
        if (!isAlive) return;
        connectedToType = newType;

        if (connectedToType == CableConnectionType.Player)
        {
            rb.angularVelocity = 0f;
            rb.mass = 1f; // Towed mass
        }
        else
        {
            rb.mass = originMass;
        }
    }

    void Update()
    {
        if (!isAlive || !GameManager.Instance.IsGamePlaying()) return;

        if (connectedToType != CableConnectionType.LifeSupportHub)
        {
            ConsumeOxygen();
            return;
        }


        if (LifeSupportHub.Instance.HasOxygen() && oxygen.GetCurrentOxygenLevel() < oxygen.GetMaxOxygen())
        {
            float oxygenAmt = LifeSupportHub.Instance.GetOxygen(oxygenRefillRatePerSecond * Time.deltaTime);
            oxygen.Refill(oxygenAmt);
        }
        else
        {
            ConsumeOxygen();
        }

    }

    void ConsumeOxygen()
    {
        oxygen.Consume(oxygenDepletionRatePerSecond * Time.deltaTime);
    }

    void FixedUpdate()
    {
        // 1. Calculate direction pointing directly away from the hub
        Vector2 directionAwayFromHub = (transform.position - LifeSupportHub.Instance.transform.position).normalized;

        // 2. Calculate the force to apply
        float appliedForce = (connectedToType == CableConnectionType.Player)
            ? playerCableConnectedDriftForce
            : outwardDriftForce;

        // 3. Apply the constant force
        rb.AddForce(directionAwayFromHub * appliedForce, ForceMode2D.Force);
    }

}
