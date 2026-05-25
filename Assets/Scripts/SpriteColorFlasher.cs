using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteColorFlasher : MonoBehaviour
{
    [SerializeField] private Color flashingColor = Color.red;
    Color32 defaultColor;
    SpriteRenderer spriteRenderer;
    Coroutine flashCoroutine;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        defaultColor = spriteRenderer.color;
    }
    public void Flash(float duration, float flashSpeed)
    {
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }
        flashCoroutine = StartCoroutine(FlashRoutine(duration, flashSpeed));
    }

    private IEnumerator FlashRoutine(float duration, float flashSpeed)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.PingPong(elapsed * flashSpeed, 1.0f);
            spriteRenderer.color = Color.Lerp(defaultColor, flashingColor, t);

            yield return null;
        }

        spriteRenderer.color = defaultColor;
        flashCoroutine = null;
    }
}