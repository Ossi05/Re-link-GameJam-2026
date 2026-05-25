using UnityEngine;

public class OxygenContainerSpawner : MonoBehaviour
{
    [SerializeField] OxygenContainer oxygenContainerPrefab;
    [Tooltip("How far past the play area radius should containers spawn? (Set this high enough to be off-camera)")]
    [SerializeField] float offScreenBuffer = 15f;

    [SerializeField] float minTimeBetweenSpawns = 5f;
    [SerializeField] float maxTimeBetweenSpawns = 10f;

    [Tooltip("How much the aim varies (in degrees) from the exact center")]
    [SerializeField] float aimVariance = 20f;

    float spawnTimer;
    float spawnTime;

    void Awake()
    {
        spawnTime = 2; // First spawn happens after 2 seconds
    }

    void Update()
    {
        if (!GameManager.Instance.IsGamePlaying()) return;
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnTime)
        {
            spawnTimer = 0f;
            SpawnOxygenContainer();
            SetRandomSpawnTime();
        }
    }

    void SpawnOxygenContainer()
    {
        if (PlayArea.Instance == null) return;

        // 1. Calculate the boundary 
        float halfWidth = (PlayArea.Instance.GetWidth() / 2f) + offScreenBuffer;
        float halfHeight = (PlayArea.Instance.GetHeight() / 2f) + offScreenBuffer;

        float spawnX = 0f;
        float spawnY = 0f;

        // 2. Pick a random edge to spawn on
        int randomEdge = Random.Range(0, 4);

        switch (randomEdge)
        {
            case 0: // Top Edge
                spawnX = Random.Range(-halfWidth, halfWidth);
                spawnY = halfHeight;
                break;
            case 1: // Right Edge
                spawnX = halfWidth;
                spawnY = Random.Range(-halfHeight, halfHeight);
                break;
            case 2: // Bottom Edge
                spawnX = Random.Range(-halfWidth, halfWidth);
                spawnY = -halfHeight;
                break;
            case 3: // Left Edge
                spawnX = -halfWidth;
                spawnY = Random.Range(-halfHeight, halfHeight);
                break;
        }

        Vector3 spawnPosition = new Vector3(
            transform.position.x + spawnX,
            transform.position.y + spawnY,
            0f
        );

        // 3. Spawn the Object
        GameObject containerObj = ObjectPoolManager.SpawnObject(
            oxygenContainerPrefab.gameObject,
            spawnPosition,
            Quaternion.identity,
            ObjectPoolManager.PoolType.OxygenContainer
        );

        // 4. Push it towards the center
        if (containerObj != null && containerObj.TryGetComponent(out OxygenContainer container))
        {
            Vector2 directionToCenter = (transform.position - spawnPosition).normalized;

            float randomDrift = Random.Range(-aimVariance, aimVariance);
            Vector2 pushedDirection = Quaternion.Euler(0, 0, randomDrift) * directionToCenter;

            container.Push(pushedDirection);
        }
    }

    void SetRandomSpawnTime()
    {
        spawnTime = Random.Range(minTimeBetweenSpawns, maxTimeBetweenSpawns);
    }

    void OnDrawGizmosSelected()
    {
        if (PlayArea.Instance == null) return;

        Gizmos.color = Color.cyan;

        float totalWidth = PlayArea.Instance.GetWidth() + (offScreenBuffer * 2f);
        float totalHeight = PlayArea.Instance.GetHeight() + (offScreenBuffer * 2f);

        Gizmos.DrawWireCube(transform.position, new Vector3(totalWidth, totalHeight, 0f));
    }
}