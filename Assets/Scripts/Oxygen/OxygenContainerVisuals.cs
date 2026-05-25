using UnityEngine;

public class OxygenContainerVisuals : BaseCableAttachPointVisuals
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Color32 selectedColor;
    Color32 defaultColor;

    protected override void Awake()
    {
        base.Awake();
        defaultColor = spriteRenderer.color;
    }

    protected override void HandleCableConnected()
    {
        SetColor(selectedColor);
    }

    protected override void HandleInteractionAvailable()
    {
        SetColor(selectedColor);
    }

    protected override void HandleInteractionUnavailable()
    {
        SetColor(defaultColor);
    }

    void SetColor(Color32 color)
    {
        spriteRenderer.color = color;
    }
}
