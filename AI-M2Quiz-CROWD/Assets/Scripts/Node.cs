using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public Vector3 worldPosition; // The world position of the node.
    public bool walkable = true; // Indicates whether the node is walkable or not.
    public int gridX; // The X coordinate of this node in the grid.
    public int gridY; // The Y coordinate of this node in the grid.
    // Cost values for A* pathfinding.
    public int gCost; // Cost from the starting node to this node.
    public int hCost; // Heuristic cost from this node to the target node.
    public Node parent; // The parent node in the path.

    public int fCost
    {
        get { return gCost + hCost; } // The total cost of this node.
    }
    public void SetPosition(Vector3 position)
    {
        worldPosition = position;
    }
    public void SetNodeAsWalkable(bool isWalkable)
    {
        walkable = isWalkable;
    }

    public void SetGridCoordinates(int x, int y)
    {
        gridX = x;
        gridY = y;
    }
}
