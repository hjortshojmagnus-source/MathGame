using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public int Liv = 1;

    [Header("Skyd indstillinger")]
    public GameObject enemyBulletPrefab; // Prefab af enemy bullet
    public float bulletSpeed = 10f;

    [Header("Random offset")]
    public float maxRandomOffset = 2f; // starter tilfældig variation
    public float offsetReductionPerRound = 0.2f; // hvor meget det falder per runde

    void Start()
    {
    }

    void Update()
    {
    }

    // Her kaldes når player skyder og rammer enemy
    private void HandleHit(GameObject other)
    {
        if (other == null) return;

        if (other.CompareTag("Bullet"))
        {
            // Bullet rammer enemy
            Destroy(other.gameObject);

            // Skyder efter bullet
            ShootAtPlayerBullet(other.transform.position);

            // Liv logik
            Liv--;
            if (Liv <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    private void ShootAtPlayerBullet(Vector3 bulletPosition)
    {
        // Beregn direction mod bullet
        Vector3 direction = (bulletPosition - transform.position).normalized;

        // Tilføj tilfældig offset
        float randomX = Random.Range(-maxRandomOffset, maxRandomOffset);
        float randomY = Random.Range(-maxRandomOffset, maxRandomOffset);
        Vector3 randomOffset = new Vector3(randomX, randomY, 0f);

        Vector3 finalTarget = bulletPosition + randomOffset;

        Vector3 shootDir = (finalTarget - transform.position).normalized;

        // Instantiér bullet
        GameObject bullet = Instantiate(enemyBulletPrefab, transform.position, Quaternion.identity);
        bullet.GetComponent<Rigidbody2D>().velocity = shootDir * bulletSpeed;
    }

    // Reducer maxRandomOffset efter runde
    public void NextRound()
    {
        maxRandomOffset = Mathf.Max(0f, maxRandomOffset - offsetReductionPerRound);
    }

    // 2D trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        HandleHit(other.gameObject);
    }

    // 2D collision
    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandleHit(collision.gameObject);
    }
}