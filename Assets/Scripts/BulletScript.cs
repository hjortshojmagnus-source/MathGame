using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public Vector3[] waypoints; // Punkter som kulen skal følge
    public float speed = 5f;
    
    private int currentWaypointIndex = 0;

    void Start()
    {
        if (waypoints != null && waypoints.Length > 0)
        {
            transform.position = waypoints[0];
        }
    }

    void Update()
    {
        if (waypoints == null || waypoints.Length == 0)
            return;

        Vector3 targetWaypoint = waypoints[currentWaypointIndex];
        Vector3 direction = (targetWaypoint - transform.position).normalized;

        // Flyt kulen mod målet
        transform.position += direction * speed * Time.deltaTime;

        // Tjek om vi har nået målet
        if (Vector3.Distance(transform.position, targetWaypoint) < 0.1f)
        {
            currentWaypointIndex++;
            
            // Hvis alle punkter er passeret, ødelæg kulen
            if (currentWaypointIndex >= waypoints.Length)
            {
                Destroy(gameObject);
            }
        }
    }
}
