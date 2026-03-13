using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    [Header("Enemy Stats")]
    public int Liv = 1;

    [Header("Shooting")]
    public GameObject enemyBulletPrefab;
    public float bulletSpeed = 10f;

    [Header("Random offset")]
    public float maxRandomOffset = 10f;          // start-variation
    public float offsetReductionPerRound = 1f; // hvor meget offset falder per runde

    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Enemy skyder når player har skudt½
    public void ShootAtPlayer()
{
    if (player == null) return;

    Vector3 playerPos = player.position;

    // Tilføj tilfældig offset
    float randomX = Random.Range(-maxRandomOffset, maxRandomOffset);
    float randomY = Random.Range(-maxRandomOffset, maxRandomOffset);
    Vector3 randomOffset = new Vector3(randomX, randomY, 0f);

    Vector3 targetPos = playerPos + randomOffset;

    // Instantiér bullet
    GameObject bullet = Instantiate(enemyBulletPrefab, transform.position, Quaternion.identity);

    BulletScript bulletScript = bullet.GetComponent<BulletScript>();

    if (bulletScript != null)
    {
        bulletScript.waypoints = new Vector3[]
        {
            transform.position,
            targetPos
        };
    }
}

    // Reducer random område efter hver runde
    public void NextRound()
    {
        maxRandomOffset = Mathf.Max(0f, maxRandomOffset - offsetReductionPerRound);
    }

    // Hit logik
    private void HandleHit(GameObject other)
    {
        if (other == null) return;

        if (other.CompareTag("Bullet"))
        {
            Destroy(other.gameObject);
            Liv--;
            if (Liv <= 0)
                Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        HandleHit(other.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandleHit(collision.gameObject);
    }
}