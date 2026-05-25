using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] AudioClip mainMenuMusic;
    [SerializeField][Range(0f, 1f)] float mainMenuVolume = 1f;
    [Space]
    [SerializeField] AudioClip gamePlayMusic;
    [SerializeField][Range(0f, 1f)] float gamePlayVolume = 0.8f;

    [Header("Settings")]
    [SerializeField] float fadeDuration = 1.5f;

    public static MusicPlayer Instance;

    AudioSource musicPlayer;
    Coroutine fadeCoroutine;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        musicPlayer = GetComponent<AudioSource>();

        DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
    }
    void OnDisable()
    {
        SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
    }

    private void SceneManager_sceneLoaded(Scene loadedScene, LoadSceneMode mode)
    {
        PlayMusic(loadedScene.name);
    }

    void PlayMusic(string currentSceneName)
    {
        if (SceneManager.GetActiveScene().name == Loader.Scene.MainMenu.ToString())
        {
            PlayMainMenuMusic();
        }
        else if (SceneManager.GetActiveScene().name == Loader.Scene.GameScene.ToString())
        {
            PlayGameMusic();
        }
    }
    public void PlayMainMenuMusic()
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeToNewMusic(mainMenuMusic, mainMenuVolume));
    }

    public void PlayGameMusic()
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeToNewMusic(gamePlayMusic, gamePlayVolume));
    }

    private IEnumerator FadeToNewMusic(AudioClip newClip, float targetVolume)
    {
        if (musicPlayer.clip == newClip && musicPlayer.isPlaying)
        {
            float currentVol = musicPlayer.volume;
            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                musicPlayer.volume = Mathf.Lerp(currentVol, targetVolume, t / fadeDuration);
                yield return null;
            }
            musicPlayer.volume = targetVolume;
            yield break;
        }

        // 1. Fade out current music
        if (musicPlayer.isPlaying)
        {
            float startVolume = musicPlayer.volume;

            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                musicPlayer.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
                yield return null;
            }
            musicPlayer.volume = 0;
            musicPlayer.Stop();
        }

        // 2. Change the clip
        musicPlayer.clip = newClip;
        musicPlayer.Play();

        // 3. Fade in new music
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            musicPlayer.volume = Mathf.Lerp(0, targetVolume, t / fadeDuration);
            yield return null;
        }
        musicPlayer.volume = targetVolume;
    }

}
