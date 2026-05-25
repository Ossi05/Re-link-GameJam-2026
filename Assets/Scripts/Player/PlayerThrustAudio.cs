using UnityEngine;

public class PlayerThrustAudio : MonoBehaviour
{
    [SerializeField] AudioSource thrusterAudioSource;

    PlayerMovement playerMovement;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        bool isThrusting = playerMovement.IsThrustingUp ||
                           playerMovement.IsThrustingDown ||
                           playerMovement.IsTurningLeft ||
                           playerMovement.IsTurningRight;

        if (isThrusting)
        {
            if (!thrusterAudioSource.isPlaying)
            {
                thrusterAudioSource.Play();
            }
        }
        else
        {
            if (thrusterAudioSource.isPlaying)
            {
                thrusterAudioSource.Stop();
            }
        }
    }

}
