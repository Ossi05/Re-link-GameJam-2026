using UnityEngine;

public abstract class BaseAttachPoint : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    Cable connectedCable;

    protected Cable GetConnectedCable() => connectedCable;

    public Rigidbody2D GetRigidBody() => rb;

    public void ConnectToCable(Cable cable)
    {
        connectedCable = cable;
    }
    public void DisconnectCable()
    {
        connectedCable = null;
    }
    public Vector2 GetLocalAnchorPosition()
    {
        return GetRigidBody().transform.InverseTransformPoint(transform.position);
    }
    public bool IsConnected()
    {
        return connectedCable != null;
    }
}