using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    [Header("Enemy Stats")]
    public int Liv = 1;

    [Header("Shooting")]
    public GameObject enemyBulletPrefab;
    public float bulletSpeed = 10f;

    [Header("AI Accuracy")]
    public float maxRandomOffset = 50f;          // Stort start-område (AI er upræcis til at starte)
    public float offsetReductionPerRound = 2f;   // Hvor meget offset falder per runde
    
    private Shoot shootController;
    private int roundsElapsed = 0;
    private float currentMaxOffset;

    void Start()
    {
        shootController = FindObjectOfType<Shoot>();
        if (shootController == null)
        {
            Debug.LogError("FEJL: Shoot-controller ikke fundet!");
        }
        
        currentMaxOffset = maxRandomOffset;
        Debug.Log($"AI-modstander spawned. Offset område: ±{currentMaxOffset}");
    }

    // AI'en skyder når spillerens bullet despawner
    public void ShootAtPlayer(Vector3 playerShootPosition)
    {
        Debug.Log("Enemy.ShootAtPlayer() kaldt med position: " + playerShootPosition);
        
        if (enemyBulletPrefab == null)
        {
            Debug.LogError("FEJL: enemyBulletPrefab er ikke assignet i Inspector!");
            return;
        }

        // Find enemy position eksplicit
        GameObject enemyObject = GameObject.FindGameObjectWithTag("Enemy"); // Antag enemy har tag "Enemy"
        if (enemyObject == null)
        {
            enemyObject = gameObject; // Fallback til this
        }
        Vector3 enemyPosition = enemyObject.transform.position;
        Debug.Log("Enemy skyder fra position: " + enemyPosition);

        // Brug den gemte position fra spilleren
        Vector3 targetPos = playerShootPosition;

        // Tilføj tilfældig offset inden for det nuværende område
        float randomX = Random.Range(-currentMaxOffset, currentMaxOffset);
        float randomY = Random.Range(-currentMaxOffset, currentMaxOffset);
        Vector3 randomOffset = new Vector3(randomX, randomY, 0f);

        targetPos += randomOffset;

        Debug.Log($"Random offset: X={randomX:F2}, Y={randomY:F2}, currentMaxOffset={currentMaxOffset:F2}");
        Debug.Log($"Runde {roundsElapsed}: AI skyder mod position {targetPos} (offset område: ±{currentMaxOffset:F1})");

        // Instantiér bullet fra enemy position
        GameObject bullet = Instantiate(enemyBulletPrefab, enemyPosition, Quaternion.identity);
        bullet.tag = "EnemyBullet"; // Sørg for at bullet har det rigtige tag

        if (bullet == null)
        {
            Debug.LogError("FEJL: Kunne ikke instantiere enemy bullet!");
            return;
        }

        BulletScript bulletScript = bullet.GetComponent<BulletScript>();

        if (bulletScript != null)
        {
            bulletScript.waypoints = new Vector3[]
            {
                enemyPosition,
                targetPos
            };
            Debug.Log("Enemy bullet sendt afsted!");
        }
        else
        {
            Debug.LogError("FEJL: BulletScript ikke fundet på enemy bullet!");
        }
    }

    // Hit logik
    private void HandleHit(GameObject other)
    {
        if (other == null) return;

        if (other.CompareTag("Bullet"))
        {
            Destroy(other.gameObject);
            Liv--;
            Debug.Log($"AI ramt! Liv tilbage: {Liv}");
            if (Liv <= 0)
            {
                Debug.Log("AI besejret!");
                Destroy(gameObject);
            }
        }
    }

    public void NotifyEnemyBulletDespawned()
    {
        Debug.Log("EnemyScript.NotifyEnemyBulletDespawned() kaldt, shootController = " + shootController);
        if (shootController != null)
        {
            shootController.OnEnemyBulletDespawned();
        }
        else
        {
            Debug.LogError("shootController er null i NotifyEnemyBulletDespawned!");
        }
    }

    // Reducer offset efter hver runde (AI bliver mere præcis)
    public void NextRound()
    {
        roundsElapsed++;
        currentMaxOffset = Mathf.Max(0f, currentMaxOffset - offsetReductionPerRound);
        Debug.Log($"Runde afsluttet. AI offset reduceret til: ±{currentMaxOffset:F1}");
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