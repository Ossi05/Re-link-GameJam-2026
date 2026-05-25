using UnityEngine;

[CreateAssetMenu(fileName = "New AudioClipsSO", menuName = "AudioClips/New AudioClipSO")]
public class AudioClipsSO : ScriptableObject
{
    [Header("Splash")]
    public AudioClip Splash;
    [Range(0f, 1f)]
    public float SplashVolume = 1f;

    [Header("Plug In")]
    public AudioClip PlugIn;
    [Range(0f, 1f)]
    public float PlugInVolume = 1f;

    [Header("Plug Out")]
    public AudioClip PlugOut;
    [Range(0f, 1f)]
    public float PlugOutVolume = 1f;

    [Header("Alarm")]
    public AudioClip Alarm;
    [Range(0f, 1f)]
    public float AlarmVolume = 1f;

}
