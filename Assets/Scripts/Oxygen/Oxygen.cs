using System;
using UnityEngine;

public class Oxygen : MonoBehaviour
{
    [SerializeField] private float maxOxygen = 100f;
    [SerializeField] private float currentOxygenLevel = 100f;

    public event EventHandler OnOxygenChanged;
    public event EventHandler OnOutOfOxygen;

    bool isOutOfOxygen = false;

    void Start()
    {
        currentOxygenLevel = maxOxygen;
        OnOxygenChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Consume(float amount)
    {
        if (isOutOfOxygen) return;
        if (currentOxygenLevel <= 0)
        {
            isOutOfOxygen = true;
            currentOxygenLevel = 0f;
            OnOutOfOxygen?.Invoke(this, EventArgs.Empty);
            return;

        }

        currentOxygenLevel = Mathf.Max(currentOxygenLevel - amount, 0f);
        OnOxygenChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Refill(float amount)
    {
        if (currentOxygenLevel >= maxOxygen) return;

        currentOxygenLevel = Mathf.Min(currentOxygenLevel + amount, maxOxygen);
        if (currentOxygenLevel > 0f)
        {
            isOutOfOxygen = false;
        }
        OnOxygenChanged?.Invoke(this, EventArgs.Empty);
    }

    public float GetCurrentOxygenLevel() => currentOxygenLevel;
    public float GetMaxOxygen() => maxOxygen;
    public float GetOxygenLevel()
    {
        if (maxOxygen <= 0) return 0f;
        return currentOxygenLevel / maxOxygen;
    }
}