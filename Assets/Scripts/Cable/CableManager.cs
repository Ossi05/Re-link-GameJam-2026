using UnityEngine;

public class CableManager : Singleton<CableManager>
{
    [SerializeField] Cable cablePrefab;

    public void Connect(AnchorPoint anchor, TowablePoint towable, float cableLength)
    {
        Cable cable = ObjectPoolManager.SpawnObject(cablePrefab, Vector3.zero, Quaternion.identity, ObjectPoolManager.PoolType.Cable);
        cable.Connect(anchor, towable, cableLength);
    }
}
