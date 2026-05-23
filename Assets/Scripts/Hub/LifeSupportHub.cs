using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class LifeSupportHub : Singleton<LifeSupportHub>
{
    [SerializeField] List<CableAttachPoint> attachPoints = new List<CableAttachPoint>();
    [SerializeField] float minCapsuleEjectTime = 0.5f;
    [SerializeField] float maxCapsuleEjectTime = 1.5f;

    private void Start()
    {
        StartCoroutine(RandomEjectionRoutine());
    }

    IEnumerator RandomEjectionRoutine()
    {
        while (true)
        {
            // 1. Wait for a random amount of time
            float ejectWaitTime = Random.Range(minCapsuleEjectTime, maxCapsuleEjectTime);
            yield return new WaitForSeconds(ejectWaitTime);

            // 2. Find all currently connected points
            List<CableAttachPoint> connectedPoints = new List<CableAttachPoint>();
            foreach (CableAttachPoint point in attachPoints)
            {
                if (point.IsConnected())
                {
                    connectedPoints.Add(point);
                }
            }

            // 3. Pick a random connected point and eject it
            if (connectedPoints.Count > 0)
            {
                int randomIndex = Random.Range(0, connectedPoints.Count);
                connectedPoints[randomIndex].Disconnect();
            }
        }
    }
}
