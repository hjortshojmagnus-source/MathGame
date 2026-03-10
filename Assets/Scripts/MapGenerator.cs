using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject stonePrefab;
    public GameObject bushPrefab;
    public GameObject treePrefab;
    public GameObject enemyPrefab;
    public GameObject playerPrefab;
    public float minPlayerEnemyDistance = 5f; // Minimum afstand mellem spiller og fjende ved spawn

    [Header("Spawn indstillinger")]
    public float minDistance = 1f; // Minimum afstand mellem objekter
    public Camera mainCamera;
    [Header("Player/Enemy Margins")]
    public float xMargin = 1f; // afstand fra venstre/højre kant
    public float yMargin = 1f; // afstand fra top/bund

    void Start()
    {
        // Hvis kamera ikke er sat i inspector, bruger vi Main Camera
        if (mainCamera == null)
            mainCamera = Camera.main;

        GenerateObjects();
        spawnCharacter();
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
    void spawnCharacter()
    {
        Vector2 playerPosition = GetSafePosition(true);
        Instantiate(playerPrefab, playerPosition, Quaternion.identity);

        Vector2 enemyPosition;
        int attempts = 0;

        do
        {
            enemyPosition = GetSafePosition(false);
            attempts++;
        }
        while (Vector2.Distance(playerPosition, enemyPosition) < minPlayerEnemyDistance && attempts < 50);

        Instantiate(enemyPrefab, enemyPosition, Quaternion.identity);
    }
    Vector2 GetRandomPositionInCameraSide(bool leftSide)
    {
        float camHeight = 2f * mainCamera.orthographicSize;
        float camWidth = camHeight * mainCamera.aspect;
        float centerX = mainCamera.transform.position.x;

        float minX, maxX;

        if (leftSide)
        {
            minX = centerX - camWidth / 2f + xMargin; // margin fra venstre kant
            maxX = centerX - xMargin;                 // margin fra midten
        }
        else
        {
            minX = centerX + xMargin;                 // margin fra midten
            maxX = centerX + camWidth / 2f - xMargin; // margin fra højre kant
        }

        float x = Random.Range(minX, maxX);
        float y = Random.Range(mainCamera.transform.position.y - camHeight / 2f + yMargin,
                               mainCamera.transform.position.y + camHeight / 2f - yMargin);

        return new Vector2(x, y);
    }
    Vector2 GetSafePosition(bool leftSide)
    {
        int attempts = 0;

        while (attempts < 50)
        {
            Vector2 pos = GetRandomPositionInCameraSide(leftSide);

            if (!Physics2D.OverlapCircle(pos, minDistance))
            {
                return pos;
            }

            attempts++;
        }

        Debug.LogWarning("Kunne ikke finde en sikker spawn position");
        return GetRandomPositionInCameraSide(leftSide);
    }
}