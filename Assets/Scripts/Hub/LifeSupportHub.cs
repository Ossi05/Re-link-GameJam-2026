using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Oxygen))]
public class LifeSupportHub : Singleton<LifeSupportHub>
{
    [SerializeField] List<CableAttachPoint> attachPoints = new List<CableAttachPoint>();
    [SerializeField] float minCapsuleEjectTime = 0.5f;
    [SerializeField] float maxCapsuleEjectTime = 1.5f;
    [Header("Oxygen")]
    [SerializeField] float oxygenDepletionRatePerSecond = 15f;

    Oxygen oxygen;

    public event EventHandler OnOutOfOxygen;

    protected override void Awake()
    {
        base.Awake();
        oxygen = GetComponent<Oxygen>();
    }

    private void Start()
    {
        StartCoroutine(RandomEjectionRoutine());
        oxygen.OnOutOfOxygen += Oxygen_OnOutOfOxygen;
    }

    void Oxygen_OnOutOfOxygen(object sender, EventArgs e)
    {
        OnOutOfOxygen?.Invoke(this, EventArgs.Empty);
    }

    void Update()
    {
        if (!HasOxygen()) return;
        oxygen.Consume(oxygenDepletionRatePerSecond * Time.deltaTime);
    }

    public float GetOxygen(float requestedAmt)
    {
        float availableOxygen = oxygen.GetCurrentOxygenLevel();

        if (availableOxygen <= 0) return 0f;

        float amountToGive = Mathf.Min(requestedAmt, availableOxygen);

        oxygen.Consume(amountToGive);

        return amountToGive;
    }

    public bool HasOxygen()
    {
        return oxygen.GetCurrentOxygenLevel() > 0f;
    }

    public void AddOxygen(float amt)
    {
        oxygen.Refill(amt);
    }

    IEnumerator RandomEjectionRoutine()
    {
        while (true)
        {
            // 1. Wait for a random amount of time
            float ejectWaitTime = UnityEngine.Random.Range(minCapsuleEjectTime, maxCapsuleEjectTime);
            yield return new WaitForSeconds(ejectWaitTime);

            // 2. Find all currently connected points
            List<CableAttachPoint> connectedPoints = new List<CableAttachPoint>();
            foreach (CableAttachPoint point in attachPoints)
            {
                if (point.IsConnected())
                {
                    connectedPoints.Add(point);
                }
            }

            // 3. Pick a random connected point and eject it
            if (connectedPoints.Count > 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, connectedPoints.Count);
                connectedPoints[randomIndex].Disconnect();
            }
        }
    }
}
