using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public int Liv = 1; // Enemy's liv

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Handle hit logic in one place
    private void HandleHit(GameObject other)
    {
        if (other == null) return;

        // If we hit a bullet, destroy both the bullet and this enemy immediately.
        if (other.CompareTag("Bullet"))
        {
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
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

    // 3D trigger
    private void OnTriggerEnter(Collider other)
    {
        HandleHit(other.gameObject);
    }

    // 3D collision
    private void OnCollisionEnter(Collision collision)
    {
        HandleHit(collision.gameObject);
    }
}
