using UnityEngine;

public abstract class BaseCableAttachPointVisuals : MonoBehaviour
{
    [Header("Base References")]
    [SerializeField] protected CableAttachPoint attachPoint;

    protected bool isPlayerLookingAtThisPoint;

    protected virtual void Awake()
    {
        attachPoint.OnConnectionChanged += AttachPoint_OnConnectionChanged;
    }

    protected virtual void Start()
    {
        if (PlayerCableController.Instance != null)
        {
            PlayerCableController.Instance.OnSelectedPointChanged += PlayerCableManager_OnSelectedPointChanged;
        }
    }

    private void AttachPoint_OnConnectionChanged(object sender, CableAttachPoint.OnConnectionChangedEventArgs e)
    {
        if (attachPoint.IsDisabled())
        {
            OnInteractionUnavailable();
        }
        else if (attachPoint.IsConnected())
        {
            OnCableConnected();
        }
        else
        {
            if (isPlayerLookingAtThisPoint)
            {
                OnInteractionAvailable();
            }
            else
            {
                OnInteractionUnavailable();
            }
        }
    }

    private void PlayerCableManager_OnSelectedPointChanged(object sender, PlayerCableController.OnSelectedPointChangedEventArgs e)
    {
        if (attachPoint.IsDisabled()) return;

        CableAttachPoint playerSelectedPoint = e.selectedPoint;

        // If we were looking at this point
        if (playerSelectedPoint != attachPoint && isPlayerLookingAtThisPoint)
        {
            isPlayerLookingAtThisPoint = false;

            if (!attachPoint.IsConnected())
            {
                OnInteractionUnavailable();
            }
            return;
        }

        // If we are already connected
        if (attachPoint.IsConnected())
        {
            if (isPlayerLookingAtThisPoint)
            {
                OnCableConnected();
            }
            return;
        }

        // If we are looking at this point
        if (playerSelectedPoint == attachPoint)
        {
            isPlayerLookingAtThisPoint = true;

            bool isPlayerTowing = PlayerCableController.Instance.IsTowing();
            bool canPickUpTowable = !isPlayerTowing && attachPoint.IsTowable();

            bool canDropOffAtAnchor = false;
            if (isPlayerTowing && attachPoint.IsAnchor())
            {
                CableAttachPoint carriedPoint = PlayerCableController.Instance.GetTowedPoint();

                if (carriedPoint != null && attachPoint.CanConnectTo(carriedPoint))
                {
                    canDropOffAtAnchor = true;
                }
            }

            if (canPickUpTowable || canDropOffAtAnchor)
            {
                OnInteractionAvailable();
            }
            else
            {
                OnInteractionUnavailable();
            }
        }
    }

    protected virtual void OnDestroy()
    {
        if (PlayerCableController.Instance != null)
        {
            PlayerCableController.Instance.OnSelectedPointChanged -= PlayerCableManager_OnSelectedPointChanged;
        }
        if (attachPoint != null)
        {
            attachPoint.OnConnectionChanged -= AttachPoint_OnConnectionChanged;
        }
    }

    protected abstract void OnInteractionAvailable();
    protected abstract void OnInteractionUnavailable();
    protected abstract void OnCableConnected();
}