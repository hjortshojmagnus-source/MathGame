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
        
        // Genkend om det er enemy bullet eller player bullet
        isEnemyBullet = CompareTag("EnemyBullet");
        
        if (!isEnemyBullet)
        {
            Debug.Log("Player bullet spawned");
            shootController = FindObjectOfType<Shoot>();
        }
        else
        {
            Debug.Log("Enemy bullet spawned");
            enemyScript = FindObjectOfType<EnemyScript>();
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
        NotifyBulletDespawned();
    }
    
    void NotifyBulletDespawned()
    {
        if (isEnemyBullet)
        {
            Debug.Log("Enemy bullet despawnet! Kalder NotifyEnemyBulletDespawned()");
            if (enemyScript != null)
            {
                enemyScript.NotifyEnemyBulletDespawned();
            }
            else
            {
                Debug.LogError("enemyScript er null!");
            }
        }
        else
        {
            Debug.Log("Player bullet despawnet! Kalder NotifyPlayerBulletDespawned()");
            if (shootController != null)
            {
                shootController.NotifyPlayerBulletDespawned();
            }
            else
            {
                Debug.LogError("shootController er null!");
            }
        }
    }
}
