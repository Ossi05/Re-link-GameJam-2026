using GogoGaga.OptimizedRopesAndCables;
using System;
using UnityEngine;

[Flags]
public enum CableConnectionType
{
    None = 0,
    Player = 1 << 0,
    LifeSupportHub = 1 << 1,
    Capsule = 1 << 2,
    OxygenContainer = 1 << 3,
    Any = ~0
}

[RequireComponent(typeof(DistanceJoint2D), typeof(FixedJoint2D))]
public class Cable : MonoBehaviour
{
    public static event EventHandler OnAnyCableConnected;
    public static event EventHandler OnAnyCableChangedOwnership;

    [SerializeField] Rope ropeVisuals;
    [SerializeField] float cableLength = 4f;

    CableAttachPoint anchorPoint;
    CableAttachPoint towablePoint;
    DistanceJoint2D distanceJoint;
    FixedJoint2D fixedJoint;

    public event EventHandler OnCableEndPointChanged;

    void Awake()
    {
        distanceJoint = GetComponent<DistanceJoint2D>();
        fixedJoint = GetComponent<FixedJoint2D>();

        distanceJoint.enabled = false;
        fixedJoint.enabled = false;
    }

    public void Connect(CableAttachPoint sourceAnchor, CableAttachPoint targetTowable)
    {
        anchorPoint = sourceAnchor;
        towablePoint = targetTowable;

        // 1. Add this cable to the attach points
        anchorPoint.SetCable(this);
        towablePoint.SetCable(this);

        // 2. Setup the Visual Rope
        if (ropeVisuals != null)
        {
            ropeVisuals.SetStartPoint(anchorPoint.transform);
            ropeVisuals.SetEndPoint(towablePoint.transform);
        }

        // 3. Setup the Distance Joint
        distanceJoint.connectedBody = towablePoint.GetParentRb();
        distanceJoint.autoConfigureConnectedAnchor = false;
        distanceJoint.connectedAnchor = towablePoint.GetLocalAnchorPosition();
        distanceJoint.distance = cableLength;
        distanceJoint.autoConfigureDistance = false;
        distanceJoint.maxDistanceOnly = true;
        distanceJoint.enabled = true;

        // 4. Setup the Fixed Joint
        fixedJoint.connectedBody = anchorPoint.GetParentRb();
        fixedJoint.autoConfigureConnectedAnchor = false;
        fixedJoint.anchor = Vector2.zero;
        fixedJoint.connectedAnchor = anchorPoint.GetLocalAnchorPosition();
        fixedJoint.enabled = true;
        OnAnyCableConnected?.Invoke(this, EventArgs.Empty);
    }

    public void MoveOwnershipTo(CableAttachPoint oldPoint, CableAttachPoint newTarget)
    {
        oldPoint.SetCable(null);

        if (anchorPoint == oldPoint)
        {
            anchorPoint = newTarget;

            fixedJoint.connectedBody = anchorPoint.GetParentRb();
            fixedJoint.connectedAnchor = anchorPoint.GetLocalAnchorPosition();

            if (ropeVisuals != null) ropeVisuals.SetStartPoint(anchorPoint.transform);
        }
        else if (towablePoint == oldPoint)
        {
            towablePoint = newTarget;

            distanceJoint.connectedBody = towablePoint.GetParentRb();
            distanceJoint.connectedAnchor = towablePoint.GetLocalAnchorPosition();

            if (ropeVisuals != null) ropeVisuals.SetEndPoint(towablePoint.transform);
        }

        newTarget.SetCable(this);
        OnCableEndPointChanged?.Invoke(this, EventArgs.Empty);
        OnAnyCableChangedOwnership?.Invoke(this, EventArgs.Empty);
    }

    public CableAttachPoint GetTowablePoint()
    {
        return towablePoint;
    }
    public CableAttachPoint GetAnchorPoint()
    {
        return anchorPoint;
    }

    public void Disconnect()
    {
        if (anchorPoint != null) anchorPoint.SetCable(null);
        if (towablePoint != null) towablePoint.SetCable(null);

        anchorPoint = null;
        towablePoint = null;

        distanceJoint.enabled = false;
        distanceJoint.connectedBody = null;
        distanceJoint.connectedAnchor = Vector2.zero;

        fixedJoint.enabled = false;
        fixedJoint.connectedBody = null;
        fixedJoint.connectedAnchor = Vector2.zero;

        ObjectPoolManager.ReturnObjectToPool(gameObject, ObjectPoolManager.PoolType.Cable);
    }
}