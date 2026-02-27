using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject stonePrefab;
    public GameObject bushPrefab;
    public GameObject treePrefab;

    [Header("Spawn indstillinger")]
    public float minDistance = 1f; // Minimum afstand mellem objekter
    public Camera mainCamera;

    void Start()
    {
        // Hvis kamera ikke er sat i inspector, bruger vi Main Camera
        if (mainCamera == null)
            mainCamera = Camera.main;

        GenerateObjects();
    }

    void GenerateObjects()
    {
        SpawnMultiple(stonePrefab, 1, 3);
        SpawnMultiple(bushPrefab, 3, 5);
        SpawnMultiple(treePrefab, 2, 4);
    }

    void SpawnMultiple(GameObject prefab, int minCount, int maxCount)
    {
        int count = Random.Range(minCount, maxCount + 1);

        for (int i = 0; i < count; i++)
        {
            bool spawned = false;
            int attempts = 0;

            while (!spawned && attempts < 50)
            {
                Vector2 position = GetRandomPositionInCamera();

                // Tjek at der ikke allerede er et objekt tæt på
                if (!Physics2D.OverlapCircle(position, minDistance))
                {
                    Instantiate(prefab, position, Quaternion.identity);
                    spawned = true;
                }

                attempts++;
            }

            if (!spawned)
                Debug.LogWarning("Kunne ikke spawn objekt uden overlap efter 50 forsøg");
        }
    }

    Vector2 GetRandomPositionInCamera()
    {
        float camHeight = 2f * mainCamera.orthographicSize;
        float camWidth = camHeight * mainCamera.aspect;

        float x = Random.Range(mainCamera.transform.position.x - camWidth / 2f,
                               mainCamera.transform.position.x + camWidth / 2f);
        float y = Random.Range(mainCamera.transform.position.y - camHeight / 2f,
                               mainCamera.transform.position.y + camHeight / 2f);

        return new Vector2(x, y);
    }
}