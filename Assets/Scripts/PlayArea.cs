using UnityEngine;

[RequireComponent(typeof(EdgeCollider2D))]
public class PlayArea : Singleton<PlayArea>
{
    [Header("Area Settings")]
    [Tooltip("The total width of the play area.")]
    [SerializeField] private float width = 100f;

    [Tooltip("The total height of the play area.")]
    [SerializeField] private float height = 60f;

    protected override void Awake()
    {
        base.Awake();
        GenerateBoundary();
    }

    void GenerateBoundary()
    {
        EdgeCollider2D edgeCollider = GetComponent<EdgeCollider2D>();
        Vector2[] colliderPoints = new Vector2[5];

        float halfWidth = width / 2f;
        float halfHeight = height / 2f;

        Vector2 topLeft = new Vector2(-halfWidth, halfHeight);
        Vector2 topRight = new Vector2(halfWidth, halfHeight);
        Vector2 bottomRight = new Vector2(halfWidth, -halfHeight);
        Vector2 bottomLeft = new Vector2(-halfWidth, -halfHeight);

        colliderPoints[0] = topLeft;
        colliderPoints[1] = topRight;
        colliderPoints[2] = bottomRight;
        colliderPoints[3] = bottomLeft;
        colliderPoints[4] = topLeft;

        edgeCollider.points = colliderPoints;
    }

    public float GetWidth() => width;
    public float GetHeight() => height;
    public Vector2 GetBounds() => new Vector2(width, height);

    public bool IsOutOfBounds(Vector2 position)
    {
        float halfWidth = width / 2f;
        float halfHeight = height / 2f;
        Vector2 localPosition = position - (Vector2)transform.position;

        return localPosition.x < -halfWidth ||
               localPosition.x > halfWidth ||
               localPosition.y < -halfHeight ||
               localPosition.y > halfHeight;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(width, height, 0f));
    }

}