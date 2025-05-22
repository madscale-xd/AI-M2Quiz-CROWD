using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float lifetime = 5f;
    public Transform target; // Assign the target this projectile should face
    public float speed = 10f; // Speed of the projectile

    void Start()
    {
        if (target != null)
        {
            transform.LookAt(target);
        }

        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // Move forward in the direction it is facing
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Obstacle"))
        {
            Destroy(gameObject);
        }
    }
}
