using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Oxygen))]
public class LifeSupportHub : Singleton<LifeSupportHub>
{
    [SerializeField] List<CableAttachPoint> attachPoints = new List<CableAttachPoint>();
    [SerializeField] float initialMinEjectTime = 3f;
    [SerializeField] float initialMaxEjectTime = 5f;
    [Space]
    [SerializeField] float minCapsuleEjectTime = 0.5f;
    [SerializeField] float maxCapsuleEjectTime = 1.5f;
    [Header("Oxygen")]
    [SerializeField] float oxygenDepletionRatePerSecond = 15f;

    Oxygen oxygen;

    public event EventHandler OnOutOfOxygen;
    public event EventHandler OnCapsuleEjected;
    public event EventHandler OnAllCapsulesDied;

    int totalCapsuleAmt = 0;
    int numCapsulesAlive = 0;

    protected override void Awake()
    {
        base.Awake();
        oxygen = GetComponent<Oxygen>();
    }

    private void Start()
    {
        GameManager.Instance.OnGameStarted += GameManager_OnGameStarted;
        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;
        oxygen.OnOutOfOxygen += Oxygen_OnOutOfOxygen;
        Capsule[] capsulesInScene = FindObjectsByType<Capsule>(FindObjectsSortMode.None);
        totalCapsuleAmt = capsulesInScene.Length;
        numCapsulesAlive = totalCapsuleAmt;
        Capsule.OnAnyDeath += Capsule_OnAnyDeath;
    }

    void GameManager_OnStateChanged(object sender, EventArgs e)
    {
        if (GameManager.Instance.IsGameOver())
        {
            StopAllCoroutines();
        }
    }

    void GameManager_OnGameStarted(object sender, EventArgs e)
    {
        StartCoroutine(RandomEjectionRoutine());
    }

    void Oxygen_OnOutOfOxygen(object sender, EventArgs e)
    {
        OnOutOfOxygen?.Invoke(this, EventArgs.Empty);
    }

    void Capsule_OnAnyDeath(object sender, EventArgs e)
    {
        numCapsulesAlive--;
        if (numCapsulesAlive <= 0)
        {
            OnAllCapsulesDied?.Invoke(this, EventArgs.Empty);
        }
    }

    void Update()
    {
        if (!HasOxygen() || !GameManager.Instance.IsGamePlaying()) return;
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
        // Initial ejection
        float firstWaitTime = UnityEngine.Random.Range(initialMinEjectTime, initialMaxEjectTime);
        yield return new WaitForSeconds(firstWaitTime);
        EjectRandomCapsule();

        while (true)
        {
            float ejectWaitTime = UnityEngine.Random.Range(minCapsuleEjectTime, maxCapsuleEjectTime);
            yield return new WaitForSeconds(ejectWaitTime);

            EjectRandomCapsule();
        }
    }

    void EjectRandomCapsule()
    {
        List<CableAttachPoint> connectedPoints = new List<CableAttachPoint>();

        foreach (CableAttachPoint point in attachPoints)
        {
            if (point.IsConnected())
            {
                connectedPoints.Add(point);
            }
        }

        if (connectedPoints.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, connectedPoints.Count);
            connectedPoints[randomIndex].Disconnect();
            OnCapsuleEjected?.Invoke(this, EventArgs.Empty);
        }
    }

    public int GetNumCapsulesAlive()
    {
        return numCapsulesAlive;
    }

    public int GetTotalCapsuleAmt()
    {
        return totalCapsuleAmt;
    }
}
