using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public Vector3[] waypoints; // Punkter som kulen skal følge
    public float speed = 5f;
    
    private int currentWaypointIndex = 0;
    private float distanceTraveled = 0f;
    private float distanceToNextWaypoint = 0f;
    private Vector3 currentDirection = Vector3.zero;

    void Start()
    {
        if (waypoints != null && waypoints.Length > 0)
        {
            transform.position = waypoints[0];
            if (waypoints.Length > 1)
            {
                SetDirectionToNextWaypoint();
            }
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
}
