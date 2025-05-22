using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class NavMeshEdgeDetector : MonoBehaviour
{
    public List<Vector3> edgePoints = new List<Vector3>();

    void Start()
    {
        FindEdges();
    }

    void FindEdges()
    {
        NavMeshTriangulation triangulation = NavMesh.CalculateTriangulation();
        Dictionary<Edge, int> edgeCount = new Dictionary<Edge, int>();

        // Count all edges from triangles
        for (int i = 0; i < triangulation.indices.Length; i += 3)
        {
            int i1 = triangulation.indices[i];
            int i2 = triangulation.indices[i + 1];
            int i3 = triangulation.indices[i + 2];

            AddEdge(triangulation.vertices[i1], triangulation.vertices[i2], edgeCount);
            AddEdge(triangulation.vertices[i2], triangulation.vertices[i3], edgeCount);
            AddEdge(triangulation.vertices[i3], triangulation.vertices[i1], edgeCount);
        }

        // Extract boundary edges (those used only once)
        foreach (var pair in edgeCount)
        {
            if (pair.Value == 1)
            {
                edgePoints.Add((pair.Key.v1 + pair.Key.v2) / 2); // center of edge
            }
        }

        Debug.Log("Edge points found: " + edgePoints.Count);
    }

    struct Edge
    {
        public Vector3 v1;
        public Vector3 v2;

        public Edge(Vector3 a, Vector3 b)
        {
            v1 = a;
            v2 = b;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Edge)) return false;
            Edge e = (Edge)obj;
            return (v1 == e.v1 && v2 == e.v2) || (v1 == e.v2 && v2 == e.v1);
        }

        public override int GetHashCode()
        {
            return v1.GetHashCode() ^ v2.GetHashCode();
        }
    }

    void AddEdge(Vector3 a, Vector3 b, Dictionary<Edge, int> edgeCount)
    {
        Edge e = new Edge(a, b);
        if (edgeCount.ContainsKey(e))
            edgeCount[e]++;
        else
            edgeCount[e] = 1;
    }
}
