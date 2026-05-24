using UnityEngine;

public class CableManager : Singleton<CableManager>
{
    [SerializeField] Cable cablePrefab;

    public void Connect(CableAttachPoint point1, CableAttachPoint point2)
    {
        if (point1 == null || point2 == null) return;

        if (point1.IsAnchor() == point2.IsAnchor())
        {
            return;
        }

        // Sort the points based on their role
        CableAttachPoint anchor = point1.IsAnchor() ? point1 : point2;
        CableAttachPoint towable = point1.IsTowable() ? point1 : point2;

        Cable cable = ObjectPoolManager.SpawnObject(cablePrefab, Vector3.zero, Quaternion.identity, ObjectPoolManager.PoolType.Cable);
        // Pass the sorted points to the cable
        cable.Connect(anchor, towable);
    }
}
