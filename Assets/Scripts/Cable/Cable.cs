using GogoGaga.OptimizedRopesAndCables;
using UnityEngine;

// Require both joints so they are guaranteed to be on the GameObject
[RequireComponent(typeof(DistanceJoint2D), typeof(FixedJoint2D))]
public class Cable : MonoBehaviour
{
    [SerializeField] Rope ropeVisuals;

    AnchorPoint anchorPoint;
    TowablePoint towablePoint;
    DistanceJoint2D distanceJoint;
    FixedJoint2D fixedJoint;

    void Awake()
    {
        distanceJoint = GetComponent<DistanceJoint2D>();
        fixedJoint = GetComponent<FixedJoint2D>();

        distanceJoint.enabled = false;
        fixedJoint.enabled = false;
    }

    public void Connect(AnchorPoint sourceAnchor, TowablePoint targetTowable, float cableLength)
    {
        anchorPoint = sourceAnchor;
        towablePoint = targetTowable;

        // 1. Add this cable to the attach points
        anchorPoint.ConnectToCable(this);
        towablePoint.ConnectToCable(this);

        // 2. Setup the Visual Rope
        if (ropeVisuals != null)
        {
            ropeVisuals.SetStartPoint(anchorPoint.transform);
            ropeVisuals.SetEndPoint(towablePoint.transform);
        }

        // 3. Setup the Distance Joint
        distanceJoint.connectedBody = towablePoint.GetRigidBody();
        distanceJoint.autoConfigureConnectedAnchor = false;
        distanceJoint.connectedAnchor = towablePoint.GetLocalAnchorPosition();
        distanceJoint.distance = cableLength;
        distanceJoint.autoConfigureDistance = false;
        distanceJoint.maxDistanceOnly = true;
        distanceJoint.enabled = true;

        // 4. Setup the Fixed Joint
        fixedJoint.connectedBody = anchorPoint.GetRigidBody();
        fixedJoint.autoConfigureConnectedAnchor = false;
        fixedJoint.anchor = Vector2.zero;
        fixedJoint.connectedAnchor = anchorPoint.GetLocalAnchorPosition();
        fixedJoint.enabled = true;
    }

    public void MoveOwnershipTo(AnchorPoint newTarget)
    {
        anchorPoint.DisconnectCable();
        anchorPoint = newTarget;
        anchorPoint.ConnectToCable(this);

        if (ropeVisuals != null)
        {
            ropeVisuals.SetStartPoint(anchorPoint.transform);
        }

        fixedJoint.connectedBody = anchorPoint.GetRigidBody();
        fixedJoint.connectedAnchor = anchorPoint.GetLocalAnchorPosition();
    }

    public void Disconnect()
    {
        if (anchorPoint != null) anchorPoint.DisconnectCable();
        if (towablePoint != null) towablePoint.DisconnectCable();

        distanceJoint.enabled = false;
        distanceJoint.connectedBody = null;

        fixedJoint.enabled = false;
        fixedJoint.connectedBody = null;

        ObjectPoolManager.ReturnObjectToPool(gameObject, ObjectPoolManager.PoolType.Cable);
    }
}