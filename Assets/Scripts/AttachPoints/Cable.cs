using GogoGaga.OptimizedRopesAndCables;
using UnityEngine;

[RequireComponent(typeof(DistanceJoint2D), typeof(FixedJoint2D))]
public class Cable : MonoBehaviour
{
    [SerializeField] Rope ropeVisuals;
    [SerializeField] float cableLength = 4f;

    CableAttachPoint anchorPoint;
    CableAttachPoint towablePoint;
    DistanceJoint2D distanceJoint;
    FixedJoint2D fixedJoint;

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
    }

    public void MoveOwnershipTo(CableAttachPoint newTarget)
    {
        anchorPoint.Disconnect();
        anchorPoint = newTarget;
        anchorPoint.SetCable(this);

        if (ropeVisuals != null)
        {
            ropeVisuals.SetStartPoint(anchorPoint.transform);
        }

        fixedJoint.connectedBody = anchorPoint.GetParentRb();
        fixedJoint.connectedAnchor = anchorPoint.GetLocalAnchorPosition();
    }

    public CableAttachPoint GetTowablePoint()
    {
        return towablePoint;
    }

    public void Disconnect()
    {
        if (anchorPoint != null) anchorPoint.Disconnect();
        if (towablePoint != null) towablePoint.Disconnect();

        distanceJoint.enabled = false;
        distanceJoint.connectedBody = null;

        fixedJoint.enabled = false;
        fixedJoint.connectedBody = null;

        ObjectPoolManager.ReturnObjectToPool(gameObject, ObjectPoolManager.PoolType.Cable);
    }
}