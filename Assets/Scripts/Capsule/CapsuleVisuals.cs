using System;
using UnityEngine;

public class CapsuleVisuals : MonoBehaviour
{
    [SerializeField] SpriteRenderer splashSprite;
    [SerializeField] SpriteColorFlasher flashed;
    Capsule capsule;
    void Awake()
    {
        capsule = GetComponentInParent<Capsule>();
        splashSprite.gameObject.SetActive(false);
    }

    void Start()
    {
        capsule.OnDeath += Capsule_OnDeath;
        capsule.OnCableDisconnected += Capsule_OnCableDisconnected;
    }

    void Capsule_OnCableDisconnected(object sender, EventArgs e)
    {
        float flashTime = 2f;
        float flashSpeed = 3f;
        flashed.Flash(flashTime, flashSpeed);
    }

    void Capsule_OnDeath(object sender, EventArgs e)
    {
        splashSprite.gameObject.SetActive(true);
    }
}
