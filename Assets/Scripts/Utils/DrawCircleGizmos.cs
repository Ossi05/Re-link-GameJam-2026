using UnityEngine;

public class DrawCircleGizmos : MonoBehaviour
{
    [SerializeField] float radius = 1f;
    [SerializeField] Color32 color = Color.green;

    void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawSphere(transform.position, radius);
    }
}