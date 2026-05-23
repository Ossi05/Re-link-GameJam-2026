using System;
using UnityEngine;

public class PlayerCableController : Singleton<PlayerCableController>
{

    [SerializeField] float interactRadius = 0.5f;
    [SerializeField] float interactDistance = 2f;
    [SerializeField] LayerMask interactableLayers;

    [Header("References")]
    [SerializeField] CableAttachPoint playerCableAttachPoint;

    CableAttachPoint selectedPoint;

    public event EventHandler<OnSelectedPointChangedEventArgs> OnSelectedPointChanged;
    public event EventHandler<CableAttachPoint.OnConnectedCableChangedEventArgs> OnAttachedCableChanged;
    public class OnSelectedPointChangedEventArgs : EventArgs
    {
        public CableAttachPoint selectedPoint;
    }

    void Player_OnInteractAction(object sender, EventArgs e)
    {
        HandleAttachCable();
    }

    void Start()
    {
        PlayerControls.Instance.OnAttachCableAction += Player_OnInteractAction;
        playerCableAttachPoint.OnConnectedCableChanged += PlayerCableAttachPoint_OnConnectedCableChanged;
    }

    void PlayerCableAttachPoint_OnConnectedCableChanged(object sender, CableAttachPoint.OnConnectedCableChangedEventArgs e)
    {

        OnAttachedCableChanged?.Invoke(this, new CableAttachPoint.OnConnectedCableChangedEventArgs
        {
            attachedCable = e.attachedCable
        });
    }

    void Update()
    {
        HandleInteractions();
    }

    void HandleInteractions()
    {
        RaycastHit2D raycastHit = Physics2D.CircleCast(
            transform.position,
            interactRadius,
            transform.up,
            interactDistance,
            interactableLayers
        );

        if (raycastHit.collider != null && raycastHit.collider.TryGetComponent(out CableAttachPoint hitPoint))
        {
            SetSelectedPoint(hitPoint);
        }
        else
        {
            SetSelectedPoint(null);
        }

    }

    void HandleAttachCable()
    {
        if (selectedPoint == null) return;

        // The player is looking at a Towable Object
        if (selectedPoint.IsTowable())
        {
            if (selectedPoint.IsConnected()) return;

            if (!playerCableAttachPoint.IsConnected())
            {
                playerCableAttachPoint.ConnectTo(selectedPoint);
            }
        }

        // The player is looking at an Anchor Point
        else if (selectedPoint.IsAnchor())
        {
            if (selectedPoint == playerCableAttachPoint) return;

            if (playerCableAttachPoint.IsConnected())
            {
                playerCableAttachPoint.MoveOwnershipTo(selectedPoint);
            }
        }
    }

    void SetSelectedPoint(CableAttachPoint newSelectedPoint)
    {
        if (selectedPoint == newSelectedPoint) return;
        selectedPoint = newSelectedPoint;
        OnSelectedPointChanged?.Invoke(this, new OnSelectedPointChangedEventArgs
        {
            selectedPoint = selectedPoint
        });
    }

    public bool IsTowing()
    {
        return playerCableAttachPoint.IsConnected();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawWireSphere(transform.position, interactRadius);

        Vector3 endPosition = transform.position + (transform.up * interactDistance);
        Gizmos.DrawWireSphere(endPosition, interactRadius);

        Vector3 rightOffset = transform.right * interactRadius;
        Vector3 leftOffset = -transform.right * interactRadius;

        Gizmos.DrawLine(transform.position + rightOffset, endPosition + rightOffset);
        Gizmos.DrawLine(transform.position + leftOffset, endPosition + leftOffset);
    }
}
