using UnityEngine;

public class CableAttachPointVisuals : MonoBehaviour
{
    static readonly int OPEN_ANIMATION_TRIGGER = Animator.StringToHash("Open");
    static readonly int CLOSE_ANIMATION_TRIGGER = Animator.StringToHash("Close");

    [SerializeField] Color32 availableColor = Color.green;
    [SerializeField] Color32 notAvailableColor = Color.red;

    [SerializeField] SpriteRenderer statusSpriteRenderer;
    [SerializeField] CableAttachPoint attachPoint;

    Animator animator;
    bool isPlayerLookingAtThisPoint;

    void Awake()
    {
        animator = GetComponent<Animator>();
        SetColor(notAvailableColor);
    }

    void Start()
    {
        PlayerCableController.Instance.OnSelectedPointChanged += PlayerCableManager_OnSelectedPointChanged;
        attachPoint.OnConnectedCableChanged += AttachPoint_OnConnectedCableChanged;
    }

    void AttachPoint_OnConnectedCableChanged(object sender, CableAttachPoint.OnAttachedCableChangedEventArgs e)
    {

        if (attachPoint.IsConnected())
        {
            PlayClosingAnimation();
            SetColor(availableColor);
        }
        else
        {
            if (isPlayerLookingAtThisPoint)
            {
                PlayOpenAnimation();
                SetColor(availableColor);
            }
            else
            {
                PlayClosingAnimation();
                SetColor(notAvailableColor);
            }
        }
    }

    void PlayerCableManager_OnSelectedPointChanged(object sender, PlayerCableController.OnSelectedPointChangedEventArgs e)
    {
        CableAttachPoint playerSelectedPoint = e.selectedPoint;

        // If we were looking at this point
        if (playerSelectedPoint != attachPoint && isPlayerLookingAtThisPoint)
        {
            isPlayerLookingAtThisPoint = false;

            if (!attachPoint.IsConnected())
            {
                SetColor(notAvailableColor);
                PlayClosingAnimation();
            }
        }

        // If we are now looking at this point
        if (playerSelectedPoint == attachPoint)
        {
            isPlayerLookingAtThisPoint = true;

            if (!attachPoint.IsConnected())
            {
                SetColor(availableColor);
                PlayOpenAnimation();
            }
        }
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

    void OnDestroy()
    {
        PlayerCableController.Instance.OnSelectedPointChanged -= PlayerCableManager_OnSelectedPointChanged;
        attachPoint.OnConnectedCableChanged -= AttachPoint_OnConnectedCableChanged;
    }
}
