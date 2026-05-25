using System.Collections;
using UnityEngine;

public class OxygenContainer : MonoBehaviour
{
    [SerializeField] float oxygenAmount = 25f;
    [SerializeField] float minFloatSpeed = 2.0f;
    [SerializeField] float maxFloatSpeed = 5.0f;
    [SerializeField] float randomSpinTorque = 10f;

    [Header("References")]
    [SerializeField] CableAttachPoint cableAttachPoint;

    Rigidbody2D rb;
    float originalMass;

    Coroutine boundsCheckCoroutine;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        originalMass = rb.mass;
    }

    void OnEnable()
    {
        if (!cableAttachPoint)
        {
            Debug.LogError("CableAttachPoint reference is missing on " + gameObject.name);
            return;
        }
        cableAttachPoint.OnConnectionChanged += CableAttachPoint_OnConnectionChanged;
        if (boundsCheckCoroutine != null) StopCoroutine(boundsCheckCoroutine);
        boundsCheckCoroutine = StartCoroutine(MonitorBounds());
    }

    void OnDisable()
    {
        cableAttachPoint.OnConnectionChanged -= CableAttachPoint_OnConnectionChanged;
        if (boundsCheckCoroutine != null) StopCoroutine(boundsCheckCoroutine);
    }
    void CableAttachPoint_OnConnectionChanged(object sender, CableAttachPoint.OnConnectionChangedEventArgs e)
    {
        if (e.connectedPoint == null)
        {
            ResetMass();
            return;
        }

        CableConnectionType connectedType = e.connectedPoint.GetPointType();

        if (connectedType == CableConnectionType.Player)
        {
            float towedMass = 1f;
            rb.angularVelocity = 0f;
            rb.mass = towedMass;
        }
        else
        {
            ResetMass();
        }
    }
    private void ResetMass()
    {
        rb.mass = originalMass;
    }

    public void Push(Vector2 direction)
    {
        rb.AddTorque(Random.Range(-randomSpinTorque, randomSpinTorque));
        rb.linearVelocity = direction * Random.Range(minFloatSpeed, maxFloatSpeed);
    }

    public float GetOxygenAmt()
    {
        return oxygenAmount;
    }

    IEnumerator MonitorBounds()
    {
        WaitForSeconds waitDelay = new WaitForSeconds(0.2f);
        float checkInterval = 0.2f;

        float timeWaiting = 0f;
        float maxWaitTime = 5f;
        bool successfullyEntered = false;

        // 1. Wait for the oxygenContainer to enter the play area
        while (timeWaiting < maxWaitTime)
        {
            if (!PlayArea.Instance.IsOutOfBounds(transform.position))
            {
                successfullyEntered = true;
                break;
            }

            yield return waitDelay;
            timeWaiting += checkInterval;
        }

        if (!successfullyEntered)
        {
            if (cableAttachPoint.IsConnected()) cableAttachPoint.Disconnect();

            ObjectPoolManager.ReturnObjectToPool(gameObject, ObjectPoolManager.PoolType.OxygenContainer);
            yield break;
        }

        // 2. Wait for the oxygenContainer to exit the playarea
        while (!PlayArea.Instance.IsOutOfBounds(transform.position))
        {
            yield return waitDelay;
        }

        if (cableAttachPoint.IsConnected()) cableAttachPoint.Disconnect();
        ObjectPoolManager.ReturnObjectToPool(gameObject, ObjectPoolManager.PoolType.OxygenContainer);
    }

}
