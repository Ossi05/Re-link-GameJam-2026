using UnityEngine;

public class LifeSupportHubVisuals : MonoBehaviour
{
    [SerializeField] SpriteColorFlasher flasher;

    void Start()
    {
        LifeSupportHub.Instance.OnCapsuleEjected += LifeSupportHub_OnCapsuleEjected;
    }

    void LifeSupportHub_OnCapsuleEjected(object sender, System.EventArgs e)
    {
        float flashTime = 2f;
        float flashSpeed = 2f;
        flasher.Flash(flashTime, flashSpeed);
    }
}
