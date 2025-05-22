using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float lifetime = 5f;
    public Transform target; // Assign the target this projectile should face

    void Start()
    {
        if (target != null)
        {
            transform.LookAt(target);
        }

        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Obstacle"))
        {
            Destroy(gameObject);
        }
    }
}
