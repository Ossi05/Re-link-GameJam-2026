using UnityEngine;

public class CableAttachPointVisuals : BaseCableAttachPointVisuals
{
    static readonly int OPEN_ANIMATION_TRIGGER = Animator.StringToHash("Open");
    static readonly int CLOSE_ANIMATION_TRIGGER = Animator.StringToHash("Close");

    [SerializeField] Color32 availableColor = Color.green;
    [SerializeField] Color32 notAvailableColor = Color.red;

    [SerializeField] SpriteRenderer statusSpriteRenderer;

    Animator animator;

    protected override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();
        SetColor(notAvailableColor);
    }

    protected override void OnInteractionAvailable()
    {
        PlayOpenAnimation();
        SetColor(availableColor);
    }

    protected override void OnInteractionUnavailable()
    {
        PlayClosingAnimation();
        SetColor(notAvailableColor);
    }

    protected override void OnCableConnected()
    {
        PlayClosingAnimation();
        SetColor(availableColor);
    }

    void SetColor(Color32 availableColor)
    {
        statusSpriteRenderer.color = availableColor;
    }

    void PlayOpenAnimation()
    {
        animator.ResetTrigger(CLOSE_ANIMATION_TRIGGER);
        animator.SetTrigger(OPEN_ANIMATION_TRIGGER);
    }

    void PlayClosingAnimation()
    {
        animator.ResetTrigger(OPEN_ANIMATION_TRIGGER);
        animator.SetTrigger(CLOSE_ANIMATION_TRIGGER);
    }


}
