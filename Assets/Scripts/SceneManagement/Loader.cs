using System;
using UnityEngine.SceneManagement;

public static class Loader
{
    public enum Scene
    {
        MainMenu,
        GameScene,
    }

    static Action onLoaderCallback;

    public static void Load(Scene scene)
    {

        string sceneName = scene.ToString();

        if (FadeTransition.Instance != null)
        {
            FadeTransition.Instance.FadeInOnSceneChange();

            onLoaderCallback = () =>
            {
                SceneManager.LoadSceneAsync(sceneName);
                FadeTransition.Instance.FadeOut();
            };
        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }

    }

    public static void LoaderCallback()
    {
        if (onLoaderCallback != null)
        {
            onLoaderCallback();
            onLoaderCallback = null;
        }
    }
}
