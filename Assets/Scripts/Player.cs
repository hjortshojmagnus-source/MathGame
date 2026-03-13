using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Player Stats")]
    public int Liv = 1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void HandleHit(GameObject other)
    {
        if (other == null) return;

        if (other.CompareTag("EnemyBullet"))
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
