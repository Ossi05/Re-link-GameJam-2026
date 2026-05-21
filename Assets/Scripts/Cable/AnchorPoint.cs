using UnityEngine;

public class AnchorPoint : BaseAttachPoint
{
    [SerializeField] float cableLength = 3f;
    [SerializeField] TowablePoint startingConnection;

    void Start()
    {
        ConnectTo(startingConnection);
    }

    public void ConnectTo(TowablePoint towablePoint)
    {
        if (towablePoint != null)
        {
            CableManager.Instance.Connect(this, towablePoint, cableLength);
        }
    }

    public void MoveOwnershipTo(AnchorPoint newTarget)
    {
        if (GetConnectedCable() == null) return;

        if (newTarget.IsConnected())
        {
            Debug.LogWarning($"{newTarget.name} is already connected to something!");
            return;
        }

        GetConnectedCable().MoveOwnershipTo(newTarget);
    }

}