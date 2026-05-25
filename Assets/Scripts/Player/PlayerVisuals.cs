using UnityEngine;

public class PlayerVisuals : MonoBehaviour
{
    [SerializeField] ParticleSystem leftThrusterParticleSystem;
    [SerializeField] ParticleSystem middleThrusterParticleSystem;
    [SerializeField] ParticleSystem rightThrusterParticleSystem;
    [SerializeField] ParticleSystem reverseThrusterParticleSystem;

    PlayerMovement playerMovement;

    void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        leftThrusterParticleSystem.Stop();
        middleThrusterParticleSystem.Stop();
        rightThrusterParticleSystem.Stop();
        reverseThrusterParticleSystem.Stop();
    }

    void Update()
    {
        ToggleParticle(middleThrusterParticleSystem, playerMovement.IsThrustingUp);
        ToggleParticle(rightThrusterParticleSystem, playerMovement.IsTurningLeft);
        ToggleParticle(leftThrusterParticleSystem, playerMovement.IsTurningRight);
        ToggleParticle(reverseThrusterParticleSystem, playerMovement.IsThrustingDown);
    }

    private void ToggleParticle(ParticleSystem particleSystem, bool isThrusting)
    {
        if (isThrusting)
        {
            if (!particleSystem.isPlaying)
            {
                particleSystem.Play();
            }
        }
        else
        {
            if (particleSystem.isPlaying)
            {
                particleSystem.Stop();
            }
        }
    }
}
