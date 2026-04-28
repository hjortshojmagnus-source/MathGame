using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public Vector3[] waypoints; // Punkter som kulen skal følge
    public float speed = 12f;
    public float maxLifetime = 6f; // seconds before auto-despawn
    
    private int currentWaypointIndex = 0;
    private float distanceTraveled = 0f;
    private float distanceToNextWaypoint = 0f;
    private Vector3 currentDirection = Vector3.zero;
    
    private bool isEnemyBullet = false;
    private Shoot shootController;
    private EnemyScript enemyScript;
    private bool hasNotifiedDespawn = false;
    private float lifeTimer = 0f;

    void Start()
    {
        if (waypoints != null && waypoints.Length > 0 && waypoints.Length > 1)
        {
            SetDirectionToNextWaypoint();
        }
        isEnemyBullet = CompareTag("EnemyBullet");

        // Always capture references to Shoot and EnemyScript for reliable callbacks
        shootController = FindFirstObjectByType<Shoot>();
        enemyScript = FindFirstObjectByType<EnemyScript>();

        if (!isEnemyBullet)
        {
            Debug.Log("Player bullet spawned");
        }
        else
        {
            Debug.Log("Enemy bullet spawned");
        }

        lifeTimer = maxLifetime;
    }

    void Update()
    {
        // Countdown lifetime even if waypoints are missing
        lifeTimer -= Time.deltaTime;
        if (lifeTimer <= 0f)
        {
            NotifyBulletDespawned();
            Destroy(gameObject);
            return;
        }

        if (waypoints == null || waypoints.Length <= 1)
            return;

        // Flyt kulen i den nuværende retning
        transform.position += currentDirection * speed * Time.deltaTime;
        distanceTraveled += speed * Time.deltaTime;

        // Tjek om vi har nået næste waypoint
        if (distanceTraveled >= distanceToNextWaypoint)
        {
            currentWaypointIndex++;
            
            // Hvis alle punkter er passeret, ødelæg kulen
            if (currentWaypointIndex >= waypoints.Length)
            {
                NotifyBulletDespawned();
                Destroy(gameObject);
                return;
            }
            
            // Sæt retning mod næste waypoint
            SetDirectionToNextWaypoint();
        }
    }
    
    void SetDirectionToNextWaypoint()
    {
        if (currentWaypointIndex < waypoints.Length - 1)
        {
            Vector3 nextWaypoint = waypoints[currentWaypointIndex + 1];
            Vector3 currentPosition = transform.position;
            
            Vector3 directionVector = nextWaypoint - currentPosition;
            distanceToNextWaypoint = directionVector.magnitude;
            currentDirection = directionVector.normalized;
            distanceTraveled = 0f;
        }
    }
    
    void OnDestroy()
    {
        if (hasNotifiedDespawn) return;

        if (isEnemyBullet)
        {
            Debug.Log("Enemy bullet destroyed - calling NotifyBulletDespawned");
        }
        else
        {
            Debug.Log("Player bullet destroyed - calling NotifyBulletDespawned");
        }

        // Ensure turn notification occurs even if bullet was destroyed via collision
        NotifyBulletDespawned();
    }
    
    void NotifyBulletDespawned()
    {
        if (hasNotifiedDespawn) return;
        hasNotifiedDespawn = true;

        if (isEnemyBullet)
        {
            Debug.Log("Enemy bullet despawnet! Notifying Shoot controller or EnemyScript.");

            // Prefer direct Shoot controller notification for reliability
            if (shootController == null)
                shootController = FindFirstObjectByType<Shoot>();

            if (shootController != null)
            {
                shootController.OnEnemyBulletDespawned();
                return;
            }

            if (enemyScript == null)
                enemyScript = FindFirstObjectByType<EnemyScript>();

            if (enemyScript != null)
            {
                enemyScript.NotifyEnemyBulletDespawned();
                return;
            }

            Debug.LogError("Ingen Shoot eller EnemyScript fundet til at afslutte enemy turen!");
        }
        else
        {
            Debug.Log("Player bullet despawnet! Kalder NotifyPlayerBulletDespawned()");

            if (shootController == null)
                shootController = FindFirstObjectByType<Shoot>();

            if (shootController != null)
            {
                shootController.NotifyPlayerBulletDespawned();
                return;
            }

            Debug.LogWarning("Shoot-controller ikke fundet ved bullet-despawn. Forsøger fallback via EnemyScript.");
            var allEnemies = FindObjectsByType<EnemyScript>(FindObjectsSortMode.None);
            if (allEnemies.Length > 0)
            {
                allEnemies[0].NotifyEnemyBulletDespawned();
            }
        }
    }
}
