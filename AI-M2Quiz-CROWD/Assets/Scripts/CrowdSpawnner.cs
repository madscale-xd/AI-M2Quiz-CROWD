using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowdSpawnner : MonoBehaviour
{
    public GameObject agentPrefab;
    public int numberOfAgents = 50;

    void Start()
    {
        for (int i = 0; i < numberOfAgents; i++)
        {
            Vector3 spawnPosition = new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
            Instantiate(agentPrefab, spawnPosition, Quaternion.identity);
        }
    }
}
