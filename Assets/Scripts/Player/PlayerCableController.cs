using System;
using UnityEngine;

public class PlayerCableController : MonoBehaviour
{
    [SerializeField] float interactRadius = 0.5f;
    [SerializeField] float interactDistance = 2f;
    [SerializeField] LayerMask interactableLayers;

    [Header("References")]
    [SerializeField] AnchorPoint playerAnchor;

    BaseAttachPoint selectedPoint;

    private void Player_OnInteractAction(object sender, EventArgs e)
    {
        HandleAttachCable();
    }

    void Start()
    {
        PlayerControls.Instance.OnAttachCableAction += Player_OnInteractAction;
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

        if (raycastHit.collider != null && raycastHit.collider.TryGetComponent(out BaseAttachPoint hitPoint))
        {
            selectedPoint = hitPoint;
        }
        else
        {
            selectedPoint = null;
        }
    }

    void HandleAttachCable()
    {
        if (selectedPoint == null) return;

        // The player is looking at a Towable Object
        if (selectedPoint is TowablePoint towableTarget)
        {
            if (towableTarget.IsConnected()) return;

            if (!playerAnchor.IsConnected())
            {
                playerAnchor.ConnectTo(towableTarget);
            }
        }

        // The player is looking at an Anchor Point
        else if (selectedPoint is AnchorPoint newAnchorPoint)
        {
            if (newAnchorPoint == playerAnchor) return;

            if (playerAnchor.IsConnected())
            {
                playerAnchor.MoveOwnershipTo(newAnchorPoint);
            }
        }
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
