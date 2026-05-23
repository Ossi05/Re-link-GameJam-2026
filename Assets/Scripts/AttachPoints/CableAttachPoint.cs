using System;
using UnityEngine;
public class CableAttachPoint : MonoBehaviour
{
    public enum PointRole { Anchor, Towable }

    [Header("Settings")]
    public PointRole role = PointRole.Towable;

    [Tooltip("Only Anchors will spawn a cable at Start")]
    [SerializeField] CableAttachPoint startingConnection;

    [Header("References")]
    [SerializeField] Rigidbody2D parentRb;

    Cable connectedCable;

    public event EventHandler<OnConnectedCableChangedEventArgs> OnConnectedCableChanged;
    public class OnConnectedCableChangedEventArgs : EventArgs
    {
        public Cable attachedCable;
    }

    void Start()
    {
        if (role == PointRole.Anchor && startingConnection != null)
        {
            ConnectTo(startingConnection);
        }
    }

    public void ConnectTo(CableAttachPoint otherPoint)
    {
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
        connectedCable = cable;
        OnConnectedCableChanged?.Invoke(this, new OnConnectedCableChangedEventArgs
        {
            attachedCable = cable
        });
    }

    public Vector2 GetLocalAnchorPosition()
    {
        if (parentRb == null) return Vector2.zero;
        return parentRb.transform.InverseTransformPoint(transform.position);
    }

    public void MoveOwnershipTo(CableAttachPoint newTarget)
    {
        if (!IsConnected()) return;

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

        connectedCable.MoveOwnershipTo(this, newTarget);
    }

    public bool IsAnchor() => role == PointRole.Anchor;
    public bool IsTowable() => role == PointRole.Towable;
}