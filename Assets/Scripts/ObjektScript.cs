using UnityEngine;

public class ObjektScript : MonoBehaviour
{
    // lives for this object, initialized based on its tag
    public int lives;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // assign lives according to tag
        if (CompareTag("Stone"))
        {
            lives = 3;
        }
        else if (CompareTag("Tree"))
        {
            lives = 2;
        }
        else if (CompareTag("Bush"))
        {
            lives = 1;
        }
        else
        {
            // default fallback
            lives = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // automatically destroy when lives drop to zero or below
        if (lives <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // assume bullets are tagged "Bullet"
        if (collision.gameObject.CompareTag("Bullet") || collision.gameObject.CompareTag("EnemyBullet"))
        {
            lives -= 1;
            // destroy the bullet as well
            Destroy(collision.gameObject);
        }
    }
}
