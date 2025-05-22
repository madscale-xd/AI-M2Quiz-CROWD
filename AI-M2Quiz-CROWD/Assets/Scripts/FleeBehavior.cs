using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FleeBehavior : MonoBehaviour
{
    public Transform target; // The threat to flee from
    private NavMeshAgent agent;
    public float fleeDistance = 10f;

    public static bool isFleeing;

    private Renderer rend;
    private float hue;
    private Color originalColor;
    private bool wasFleeingLastFrame = false;

    public GameObject projectilePrefab; // Assign in Inspector
    public float shootCooldown = 5f;
    public Transform firePoint; // Optional: where the projectile spawns from

    private float fleeTimer = 0f;

    void Start()
    {
        target = FindObjectOfType<NavmeshPlayerController>().transform;
        agent = GetComponent<NavMeshAgent>();
        rend = GetComponentInChildren<Renderer>();
        hue = 0f;

        if (rend != null)
        {
            originalColor = rend.material.color;
        }

        if (IsTargetClose())
        {
            FleeFromTheTarget();
        }
    }

    void Update()
    {
        bool targetClose = IsTargetClose();

        if (targetClose)
        {
            FleeFromTheTarget();
            RainbowEffect();

            fleeTimer += Time.deltaTime;
            if (fleeTimer >= shootCooldown)
            {
                ShootAtTarget();
                fleeTimer = 0f; // Reset cooldown
            }
        }
        else
        {
            if (wasFleeingLastFrame)
            {
                ResetColor();
            }

            fleeTimer = 0f; // Reset if no longer fleeing
        }

        wasFleeingLastFrame = targetClose;
    }

    private bool IsTargetClose()
    {
        float distanceTarget = Vector3.Distance(transform.position, target.position);
        isFleeing = distanceTarget <= fleeDistance;
        return isFleeing;
    }

    private void FleeFromTheTarget()
    {
        if (target != null)
        {
            Vector3 fleeDirection = transform.position - target.position;
            Vector3 fleePosition = transform.position + fleeDirection.normalized * 10f;

            NavMesh.SamplePosition(fleePosition, out NavMeshHit navHit, 10, NavMesh.AllAreas);
            agent.SetDestination(navHit.position);
        }
    }

    private void RainbowEffect()
    {
        if (rend != null)
        {
            hue += Time.deltaTime * 0.5f;
            if (hue > 1f) hue = 0f;
            Color rainbowColor = Color.HSVToRGB(hue, 1f, 1f);
            rend.material.color = rainbowColor;
        }
    }

    private void ResetColor()
    {
        if (rend != null)
        {
            rend.material.color = originalColor;
        }
    }

    private void ShootAtTarget()
    {
        if (projectilePrefab != null && target != null)
        {
            Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position + Vector3.up;
            GameObject projectile = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);

            // Calculate direction to target
            Vector3 direction = (target.position - spawnPos).normalized;

            // Make projectile face the target direction
            projectile.transform.rotation = Quaternion.LookRotation(direction);

            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = direction * 10f; // Adjust speed as needed
            }
        }
    }
}
