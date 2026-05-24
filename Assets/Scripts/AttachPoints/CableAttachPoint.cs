using System;
using UnityEngine;
public class CableAttachPoint : MonoBehaviour
{
    public enum AttachPointRole { Anchor, Towable }

    [Header("Settings")]
    [Tooltip("What type of object is this point attached to?")]
    [SerializeField] CableConnectionType pointType = CableConnectionType.None;
    [Tooltip("What types of objects are allowed to connect to this point?")]
    [SerializeField] CableConnectionType acceptedConnections = CableConnectionType.Any;

    [SerializeField] AttachPointRole role = AttachPointRole.Towable;

    [Tooltip("Only Anchors will spawn a cable at Start")]
    [SerializeField] CableAttachPoint startingConnection;

    [Header("References")]
    [SerializeField] Rigidbody2D parentRb;

    Cable connectedCable;
    bool isDisabled;

    public event EventHandler<OnConnectionChangedEventArgs> OnConnectionChanged;
    public class OnConnectionChangedEventArgs : EventArgs
    {
        public CableAttachPoint connectedPoint;
    }

    void Start()
    {
        if (role == AttachPointRole.Anchor && startingConnection != null)
        {
            ConnectTo(startingConnection);
        }
    }

    public bool CanConnectTo(CableAttachPoint otherPoint)
    {
        if (isDisabled || otherPoint == null || otherPoint.IsDisabled()) return false;
        if (this.role == otherPoint.role) return false;

        bool thisAcceptsOther = (acceptedConnections & otherPoint.pointType) != 0;
        bool otherAcceptsThis = (otherPoint.acceptedConnections & this.pointType) != 0;

        return thisAcceptsOther && otherAcceptsThis;
    }

    public void ConnectTo(CableAttachPoint otherPoint)
    {
        if (!CanConnectTo(otherPoint))
        {
            Debug.LogWarning($"Connection failed: {name} cannot connect to {otherPoint.name} due to type or role mismatch.");
            return;
        }

        CableManager.Instance.Connect(this, otherPoint);
    }

    public void MoveOwnershipTo(CableAttachPoint newTarget)
    {
        if (!IsConnected() || isDisabled) return;

        CableAttachPoint towedPoint = GetConnectedPoint();
        if (towedPoint == null) return;

        if (!newTarget.CanConnectTo(towedPoint))
        {
            Debug.LogWarning($"Move ownership failed: {name} cannot move ownership to {newTarget.name} because it can't connect to the currently towed point.");
            return;
        }

        connectedCable.MoveOwnershipTo(this, newTarget);
    }

    public void SetCable(Cable cable)
    {
        if (connectedCable != null)
            connectedCable.OnCableEndPointChanged -= HandleCableEndpointsChanged;

        connectedCable = cable;

        if (connectedCable != null)
            connectedCable.OnCableEndPointChanged += HandleCableEndpointsChanged;

        FireConnectionEvent();
    }

    void HandleCableEndpointsChanged(object sender, EventArgs e)
    {
        FireConnectionEvent();
    }

    void FireConnectionEvent()
    {
        OnConnectionChanged?.Invoke(this, new OnConnectionChangedEventArgs
        {
            connectedPoint = GetConnectedPoint()
        });
    }

    public void Disconnect()
    {
        if (connectedCable == null) return;
        Cable cableToDisconnect = connectedCable;
        SetCable(null);
        cableToDisconnect.Disconnect();
    }

    public CableAttachPoint GetConnectedPoint()
    {
        if (connectedCable == null) return null;
        return role == AttachPointRole.Anchor
            ? connectedCable.GetTowablePoint()
            : connectedCable.GetAnchorPoint();
    }

    public void Disable()
    {
        isDisabled = true;
        Disconnect();
    }

    public Vector2 GetLocalAnchorPosition() => parentRb ? parentRb.transform.InverseTransformPoint(transform.position) : Vector2.zero;
    public Rigidbody2D GetParentRb() => parentRb;
    public CableConnectionType GetPointType() => pointType;
    public bool IsConnected() => connectedCable != null;
    public bool IsDisabled() => isDisabled;
    public bool IsAnchor() => role == AttachPointRole.Anchor;
    public bool IsTowable() => role == AttachPointRole.Towable;

    public bool TryGetComponentFromParent<T>(out T component)
    {
        if (parentRb == null)
        {
            component = default;
            return false;
        }
        return parentRb.TryGetComponent(out component);
    }
}