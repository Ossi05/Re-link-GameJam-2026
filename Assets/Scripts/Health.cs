using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField][Range(1, 1000)] int maxHealth;

    int health;

    void Awake()
    {
        health = maxHealth;
    }

    public event EventHandler OnHealthChanged;
    public event EventHandler OnDie;


    public void TakeDamage(int damage)
    {
        if (health == 0 || damage == 0)
        {
            return;
        }

        int newHealth = health - damage;
        ChangeHealth(newHealth);
    }

    public void Heal(int healAmt)
    {
        if (health == 0 || healAmt <= 0)
        {
            return;
        }

        int newHealth = health + healAmt;
        ChangeHealth(newHealth);
    }


    void ChangeHealth(int newHealth)
    {
        health = Mathf.Clamp(newHealth, 0, maxHealth);
        OnHealthChanged?.Invoke(this, EventArgs.Empty);
        if (health == 0)
        {
            OnDie?.Invoke(this, EventArgs.Empty);
        }
    }

    public int GetCurrentHealth() => health;

    public int GetMaxHealth() => maxHealth;
}