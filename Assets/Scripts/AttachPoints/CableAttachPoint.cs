using System;
using UnityEngine;
public class CableAttachPoint : MonoBehaviour
{
    public enum OwnerType { Other, Player, LifeSupportHub }
    public enum AttachPointRole { Anchor, Towable }

    [Header("Settings")]
    [SerializeField] OwnerType ownerType = OwnerType.Other;
    [SerializeField] AttachPointRole role = AttachPointRole.Towable;


    [Tooltip("Only Anchors will spawn a cable at Start")]
    [SerializeField] CableAttachPoint startingConnection;

    [Header("References")]
    [SerializeField] Rigidbody2D parentRb;

    Cable connectedCable;
    bool isDisabled;

    public event EventHandler<OnConnectedCableChangedEventArgs> OnConnectedCableChanged;
    public event EventHandler<OnConnectedCableChangedEventArgs> OnCableEndPointChanged;

    public class OnConnectedCableChangedEventArgs : EventArgs
    {
        public Cable attachedCable;
    }

    void Start()
    {
        if (role == AttachPointRole.Anchor && startingConnection != null)
        {
            ConnectTo(startingConnection);
        }
    }

    public void ConnectTo(CableAttachPoint otherPoint)
    {
        if (isDisabled) return;
        if (otherPoint == null) return;
        if (this.role == otherPoint.role)
        {
            Debug.LogWarning($"Connection failed: Cannot connect a {this.role} to another {otherPoint.role}.");
            return;
        }

        CableManager.Instance.Connect(this, otherPoint);
    }

    public void Disconnect()
    {
        if (connectedCable == null) return;
        Cable cableToDisconnect = connectedCable;

        SetCable(null);
        cableToDisconnect.Disconnect();
    }
    public Rigidbody2D GetParentRb() => parentRb;
    public bool IsConnected() => connectedCable != null;
    public void SetCable(Cable cable)
    {

        if (connectedCable != null)
        {
            connectedCable.OnCableEndPointChanged -= HandleCableEndPointChanged;
        }

        connectedCable = cable;
        if (connectedCable != null)
        {
            connectedCable.OnCableEndPointChanged += HandleCableEndPointChanged;
        }

        OnConnectedCableChanged?.Invoke(this, new OnConnectedCableChangedEventArgs
        {
            attachedCable = connectedCable
        });
    }

    void HandleCableEndPointChanged(object sender, EventArgs e)
    {
        OnCableEndPointChanged?.Invoke(this, new OnConnectedCableChangedEventArgs
        {
            attachedCable = connectedCable
        });
    }

    public Vector2 GetLocalAnchorPosition()
    {
        if (parentRb == null) return Vector2.zero;
        return parentRb.transform.InverseTransformPoint(transform.position);
    }

    public void MoveOwnershipTo(CableAttachPoint newTarget)
    {
        if (!IsConnected() || IsDisabled()) return;

        if (newTarget.IsConnected())
        {
            Debug.LogWarning($"{newTarget.name} is already connected to a cable!");
            return;
        }

        if (this.role != newTarget.role)
        {
            Debug.LogWarning($"Transfer failed: Cannot pass a {this.role} cable end to a {newTarget.role} node.");
            return;
        }

        Cable cableToMove = connectedCable;

        cableToMove.MoveOwnershipTo(this, newTarget);
    }

    public void Disable()
    {
        isDisabled = true;
        Disconnect();
    }
    public bool IsDisabled() => isDisabled;

    public OwnerType GetOwnerType() => ownerType;

    public bool IsAnchor() => role == AttachPointRole.Anchor;
    public bool IsTowable() => role == AttachPointRole.Towable;
    public CableAttachPoint GetAnchor() => connectedCable.GetAnchorPoint();

    public AttachPointRole GetRole() => role;
}