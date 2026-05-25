
using UnityEngine;
using UnityEngine.UI;

public class LoadSceneButton : MonoBehaviour {

    [Header("Settings")]
    [SerializeField] Loader.Scene sceneToLoad;
    Button button;
    void Awake()
    {
        button = GetComponent<Button>();

        button.onClick.AddListener(() =>
        {
            button.interactable = false;
            Loader.Load(sceneToLoad);
        });
    }
}
