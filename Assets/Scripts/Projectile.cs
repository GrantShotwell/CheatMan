using UnityEngine;

public class Projectile : MonoBehaviour
{

    public bool playerProjectile = false;
    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (playerProjectile)
        {
            if (!collision.transform.CompareTag("PlayerProjectile") && !collision.transform.CompareTag("Player"))
            {
                Destroy(gameObject);
            }
        }
        else
        {
            if (!collision.transform.CompareTag("Enemy") && !collision.transform.CompareTag("EnemyProjectile"))
            {
                Destroy(gameObject);
            }
        }
    }
}
