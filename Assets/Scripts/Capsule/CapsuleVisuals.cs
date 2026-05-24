using System;
using UnityEngine;

public class CapsuleVisuals : MonoBehaviour
{
    [SerializeField] SpriteRenderer splashSprite;
    Capsule capsule;
    void Awake()
    {
        capsule = GetComponentInParent<Capsule>();
        splashSprite.gameObject.SetActive(false);
    }

    void Start()
    {
        capsule.OnDeath += Capsule_OnDeath;
    }

    void Capsule_OnDeath(object sender, EventArgs e)
    {
        splashSprite.gameObject.SetActive(true);
    }
}
