using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioClipsSO audioClips;

    public static AudioManager Instance;

    float volume = 1f;
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        Capsule.OnAnyDeath += Capsule_OnAnyDeath;
        LifeSupportHub.Instance.OnCapsuleEjected += LifeSupportHub_OnCapsuleEjected;
        Cable.OnAnyCableChangedOwnership += Cable_OnAnyCableChangedOwnership;
        Cable.OnAnyCableConnected += Cable_OnAnyCableConnected;
    }

    void OnDestroy()
    {
        Capsule.OnAnyDeath -= Capsule_OnAnyDeath;
    }

    void Capsule_OnAnyDeath(object sender, System.EventArgs e)
    {
        PlaySound(audioClips.Splash, audioClips.SplashVolume, Camera.main.transform.position);
    }

    void LifeSupportHub_OnCapsuleEjected(object sender, System.EventArgs e)
    {
        PlaySound(audioClips.Alarm, audioClips.AlarmVolume, Camera.main.transform.position);
    }

    void Cable_OnAnyCableChangedOwnership(object sender, System.EventArgs e)
    {
        PlaySound(audioClips.PlugOut, audioClips.PlugOutVolume, Camera.main.transform.position);
    }

    void Cable_OnAnyCableConnected(object sender, System.EventArgs e)
    {
        PlaySound(audioClips.PlugIn, audioClips.PlugInVolume, Camera.main.transform.position);
    }

    void PlaySound(AudioClip[] sounds, float volume, Vector3 position)
    {
        AudioClip randomSound = sounds[Random.Range(0, sounds.Length)];
        PlaySound(randomSound, volume, position);
    }

    void PlaySound(AudioClip sound, float volumeMultiplier, Vector3 position)
    {
        if (position == null)
        {
            position = Camera.main.transform.position;
        }

        AudioSource.PlayClipAtPoint(sound, position, volume * volumeMultiplier);
    }

}
