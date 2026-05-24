using UnityEngine;

public class OxygenDropOffPoint : MonoBehaviour
{
    [SerializeField] CableAttachPoint cableAttachPoint;

    private void OnEnable()
    {
        if (cableAttachPoint != null)
        {
            cableAttachPoint.OnConnectionChanged += CableAttachPoint_OnConnectionChanged;
        }
    }

    private void OnDisable()
    {
        if (cableAttachPoint != null)
        {
            cableAttachPoint.OnConnectionChanged -= CableAttachPoint_OnConnectionChanged;
        }
    }

    private void CableAttachPoint_OnConnectionChanged(object sender, CableAttachPoint.OnConnectionChangedEventArgs e)
    {
        CableAttachPoint connectedPoint = e.connectedPoint;

        if (connectedPoint == null)
        {
            return;
        }

        CableConnectionType connectedType = connectedPoint.GetPointType();

        if (connectedType == CableConnectionType.OxygenContainer && connectedPoint.TryGetComponentFromParent(out OxygenContainer oxygenContainer))
        {
            LifeSupportHub.Instance.AddOxygen(oxygenContainer.GetOxygenAmt());
            connectedPoint.Disconnect();
            ObjectPoolManager.ReturnObjectToPool(oxygenContainer.gameObject, ObjectPoolManager.PoolType.OxygenContainer);
        }

    }
}
