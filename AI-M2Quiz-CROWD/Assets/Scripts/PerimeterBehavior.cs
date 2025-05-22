using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class PerimeterBehavior : MonoBehaviour
{
    private static bool perimeterActivated = false;
    private static bool rotatingClockwise = false;
    private static List<PerimeterBehavior> allAgents = new List<PerimeterBehavior>();

    public float fireInterval = 5f;
    public GameObject projectilePrefab;
    public Transform firePoint;
    private Transform target;
    private float fireTimer;

    private NavMeshAgent agent;
    private Renderer rend;
    private Color originalColor;

    private float hue = 0f;

    private int followAgentIndex = -1;
    private Vector3 currentTargetPosition;

    private bool fireRateIncreased = false;

    void Start()
    {
        target = FindObjectOfType<NavmeshPlayerController>().transform;
        agent = GetComponent<NavMeshAgent>();
        rend = GetComponentInChildren<Renderer>();

        if (rend != null)
            originalColor = rend.material.color;

        allAgents.Add(this);
    }

    void Update()
    {
        if (!perimeterActivated && SceneTimerManager.SceneTime >= 30.4f)
        {
            ActivatePerimeterMode();
        }

        if (perimeterActivated && !rotatingClockwise && SceneTimerManager.SceneTime >= 57.2f)
        {
            StartRotatingPerimeter();
        }

        // ✅ Move this outside and check it independently
        if (!fireRateIncreased && SceneTimerManager.SceneTime >= 57.2f)
        {
            fireInterval *= 0.5f; // Halve the interval (double the firing rate)
            fireRateIncreased = true;
        }

        if (perimeterActivated)
        {
            fireTimer += Time.deltaTime;

            if (fireTimer >= fireInterval)
            {
                ShootAtTarget();
                fireTimer = 0f;
            }

            RainbowEffect();
        }

        if (rotatingClockwise)
        {
            MoveTowardsNextAgent();
        }
    }

    private void ActivatePerimeterMode()
    {
        perimeterActivated = true;

        foreach (var agent in allAgents)
        {
            if (agent.TryGetComponent<WanderingAgent>(out var wander))
                wander.enabled = false;

            if (agent.TryGetComponent<FleeBehavior>(out var flee))
                flee.enabled = false;

            agent.MoveToPerimeterPosition();
        }
    }

    private void StartRotatingPerimeter()
    {
        rotatingClockwise = true;

        for (int i = 0; i < allAgents.Count; i++)
        {
            int rightNeighborIndex = (i + 1) % allAgents.Count;
            allAgents[i].followAgentIndex = rightNeighborIndex;
            allAgents[i].currentTargetPosition = allAgents[rightNeighborIndex].transform.position;
        }
    }

    private void MoveTowardsNextAgent()
    {
        if (followAgentIndex < 0 || followAgentIndex >= allAgents.Count)
            return;

        Vector3 targetPos = allAgents[followAgentIndex].transform.position;
        float distance = Vector3.Distance(transform.position, targetPos);

        // Don't reset destination if already close
        if (distance > 1.5f)
        {
            agent.SetDestination(targetPos);
            currentTargetPosition = targetPos;
        }
    }

    private void MoveToPerimeterPosition()
    {
        Bounds groundBounds = GetGroundBounds();

        if (groundBounds.size == Vector3.zero) return;

        int index = allAgents.IndexOf(this);
        int total = allAgents.Count;

        float perimeterLength = (groundBounds.size.x + groundBounds.size.z) * 2f;
        float spacing = perimeterLength / total;
        float currentDistance = spacing * index;

        Vector3 pos = GetPointOnPerimeter(groundBounds, currentDistance);
        agent.SetDestination(pos);
    }

    private Bounds GetGroundBounds()
    {
        Renderer ground = GameObject.Find("Ground")?.GetComponent<Renderer>();
        return ground ? ground.bounds : new Bounds();
    }

    private Vector3 GetPointOnPerimeter(Bounds bounds, float dist)
    {
        float xMin = bounds.min.x;
        float xMax = bounds.max.x;
        float zMin = bounds.min.z;
        float zMax = bounds.max.z;

        float width = xMax - xMin;
        float depth = zMax - zMin;

        if (dist <= width)
            return new Vector3(xMin + dist, bounds.center.y, zMin);
        dist -= width;

        if (dist <= depth)
            return new Vector3(xMax, bounds.center.y, zMin + dist);
        dist -= depth;

        if (dist <= width)
            return new Vector3(xMax - dist, bounds.center.y, zMax);
        dist -= width;

        return new Vector3(xMin, bounds.center.y, zMax - dist);
    }

    private void ShootAtTarget()
    {
        if (projectilePrefab != null && target != null)
        {
            Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position + Vector3.up;
            GameObject projectile = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);

            Vector3 direction = (target.position - spawnPos).normalized;
            projectile.transform.rotation = Quaternion.LookRotation(direction);

            if (projectile.TryGetComponent<Rigidbody>(out var rb))
                rb.velocity = direction * 10f;
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
}
