using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public Vector3[] waypoints; // Punkter som kulen skal følge
    public float speed = 5f;
    
    private int currentWaypointIndex = 0;
    private float distanceTraveled = 0f;
    private float distanceToNextWaypoint = 0f;
    private Vector3 currentDirection = Vector3.zero;
    
    private bool isEnemyBullet = false;
    private Shoot shootController;
    private EnemyScript enemyScript;

    void Start()
    {
        if (waypoints != null && waypoints.Length > 0 && waypoints.Length > 1)
        {
            SetDirectionToNextWaypoint();
        }

        isEnemyBullet = CompareTag("EnemyBullet");

        if (!isEnemyBullet)
        {
            shootController = FindFirstObjectByType<Shoot>();
            Debug.Log("Player bullet spawned");
        }
        else
        {
            enemyScript = FindFirstObjectByType<EnemyScript>();
            Debug.Log("Enemy bullet spawned");
        }
    }

    void Update()
    {
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
        if (isEnemyBullet)
        {
            Debug.Log("Enemy bullet destroyed - calling NotifyBulletDespawned");
        }
        else
        {
            Debug.Log("Player bullet destroyed - calling NotifyBulletDespawned");
        }
        
    }
    
    void NotifyBulletDespawned()
    {

        if (isEnemyBullet)
        {
            Debug.Log("Enemy bullet despawnet! Kalder NotifyEnemyBulletDespawned()");

            if (enemyScript == null)
            {
                enemyScript = FindFirstObjectByType<EnemyScript>();
            }

            if (enemyScript != null)
            {
                enemyScript.NotifyEnemyBulletDespawned();
                return;
            }

            // Fallback: direkte til Shoot-controller, hvis EnemyScript ikke er tilgængelig
            Debug.LogWarning("EnemyScript ikke fundet. Bruger direkte Shoot-kald.");
            Shoot fallbackShoot = FindFirstObjectByType<Shoot>();
            if (fallbackShoot != null)
            {
                fallbackShoot.OnEnemyBulletDespawned();
            }
            else
            {
                Debug.LogError("Ingen Shoot-controller fundet til at afslutte enemy turen!");
            }
        }
        else
        {
            Debug.Log("Player bullet despawnet! Kalder NotifyPlayerBulletDespawned()");
            if (shootController == null)
            {
                shootController = FindFirstObjectByType<Shoot>();
                if (shootController == null)
                {
                    // Sidste resort: søg gennem alleGameObjects
                    Shoot[] allShooters = FindObjectsByType<Shoot>(FindObjectsSortMode.None);
                    if (allShooters.Length > 0)
                    {
                        shootController = allShooters[0];
                    }
                }
            }
            if (shootController != null)
            {
                shootController.NotifyPlayerBulletDespawned();
            }
            else
            {
                Debug.LogWarning("Shoot-controller ikke fundet ved bullet-despawn. Forsøger direkte tur-ændring.");
                // Fallback: sæt player turn direkte hvis vi ikke kan finde shootController
                var allEnemies = FindObjectsByType<EnemyScript>(FindObjectsSortMode.None);
                if (allEnemies.Length > 0)
                {
                    allEnemies[0].NotifyEnemyBulletDespawned();
                }
            }
        }
    }
}
