using System.Collections.Generic;
using UnityEngine;

public class AStar : MonoBehaviour
{
    private Node[,] grid;
    [SerializeField] private GameObject nodePrefab;
    [SerializeField] private Vector2 gridSize;


    private void Start()
    {
        int gridWidth = Mathf.FloorToInt(gridSize.x);
        int gridHeight = Mathf.FloorToInt(gridSize.y);

        float nodeWidth = gridSize.x / gridWidth;
        float nodeHeight = gridSize.y / gridHeight;

        grid = new Node[gridWidth, gridHeight];

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                // Calculate the position of the node based on its grid coordinates.
                float xPos = x * nodeWidth + nodeWidth / 2 - gridSize.x / 2;
                float yPos = y * nodeHeight + nodeHeight / 2 - gridSize.y / 2;
                Vector3 nodePosition = new Vector3(xPos, 1, yPos);

                Node node = Instantiate(nodePrefab, nodePosition, Quaternion.identity).GetComponent<Node>();

                node.SetPosition(nodePosition);
                node.SetGridCoordinates(x, y);

                node.SetNodeAsWalkable(true);

                grid[x, y] = node;
            }
        }
    }
   
    public List<Node> FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Node startNode = NodeFromWorldPoint(startPos);
        Node targetNode = NodeFromWorldPoint(targetPos);

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || (openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost))
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                return RetracePath(startNode, targetNode);
            }

            foreach (Node neighbor in GetNeighbors(currentNode))
            {
                if (!neighbor.walkable || closedSet.Contains(neighbor))
                {
                    continue;
                }

                int newCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);
                if (newCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = newCostToNeighbor;
                    neighbor.hCost = GetDistance(neighbor, targetNode);
                    neighbor.parent = currentNode;

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }

        // No path found.
        return null;
    }

    private List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        path.Reverse();
        return path;
    }
    private int GetDistance(Node nodeA, Node nodeB)
    {
        int dx = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dy = Mathf.Abs(nodeA.gridY - nodeB.gridY);
        return dx + dy;
    }
    private List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();
        int x = node.gridX;
        int y = node.gridY;

        if (x > 0)
            neighbors.Add(grid[x - 1, y]); // Left
        if (x < grid.GetLength(0) - 1)
            neighbors.Add(grid[x + 1, y]); // Right
        if (y > 0)
            neighbors.Add(grid[x, y - 1]); // Down
        if (y < grid.GetLength(1) - 1)
            neighbors.Add(grid[x, y + 1]); // Up

        return neighbors;
    }


    private Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + gridSize.x / 2) / gridSize.x;
        float percentY = (worldPosition.z + gridSize.y / 2) / gridSize.y;

        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.FloorToInt((grid.GetLength(0) - 1) * percentX);
        int y = Mathf.FloorToInt((grid.GetLength(1) - 1) * percentY);

        return grid[x, y];
    }
}
