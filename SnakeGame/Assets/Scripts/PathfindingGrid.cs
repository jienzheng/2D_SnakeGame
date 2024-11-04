using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class representing each individual node in the pathfinding grid
public class Node
{
    public bool walkable;          // Indicates if the node is traversable
    public Vector3 worldPosition;  // The position of this node in the game world
    public int gridX;              // X coordinate of this node in the grid
    public int gridY;              // Y coordinate of this node in the grid
    public int gCost;              // Cost from the start node to this node
    public int hCost;              // Heuristic cost from this node to the target node
    public Node parent;            // Parent node used for path retracing

    // Constructor to initialize a new node
    public Node(bool _walkable, Vector3 _worldPosition, int _gridX, int _gridY)
    {
        walkable = _walkable;
        worldPosition = _worldPosition;
        gridX = _gridX;
        gridY = _gridY;
    }

    // Property to calculate fCost (total cost) of this node, used in pathfinding
    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }
}

// Grid class to manage the pathfinding grid and node creation
public class PathfindingGrid : MonoBehaviour
{
    public LayerMask unwalkableMask;   // Mask to identify unwalkable areas
    public Vector2 gridWorldSize;      // Size of the grid in the world
    public float nodeRadius;           // Radius of each node
    public Node[,] grid;               // 2D array of nodes representing the grid

    private float nodeDiameter;        // Diameter of each node, calculated as twice the radius
    private int gridSizeX, gridSizeY;  // Number of nodes along each axis

    void Awake()
    {
        nodeDiameter = nodeRadius * 2;  // Calculate node diameter
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);  // Calculate grid size along X
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);  // Calculate grid size along Y
        CreateGrid();  // Create the grid
    }

    // Method to create the grid with nodes
    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        // Calculate the bottom-left corner of the grid in world space
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

        // Loop through each position in the grid to create nodes
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                // Calculate the world position of the node
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                // Determine if the node is walkable based on collision with unwalkableMask
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));
                grid[x, y] = new Node(walkable, worldPoint, x, y);  // Initialize and add the node to the grid
            }
        }
    }

    // Returns the node at a given world position by converting it to grid coordinates
    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        // Convert world position to grid percentage along each axis
        float percentX = Mathf.Clamp01((worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x);
        float percentY = Mathf.Clamp01((worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y);

        // Convert percentages to grid coordinates
        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        return grid[x, y];  // Return the node at the calculated coordinates
    }

    // Returns a list of neighboring nodes around a given node
    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        // Loop through possible neighbors within a 3x3 area around the node
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                // Skip the center node (itself)
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.gridX + x;  // Calculate the X position of the neighbor
                int checkY = node.gridY + y;  // Calculate the Y position of the neighbor

                // Check if the neighbor is within grid bounds
                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);  // Add the neighbor to the list
                }
            }
        }

        return neighbours;  // Return the list of neighbors
    }

    // Draws the grid in the editor for visual debugging
    void OnDrawGizmos()
    {
        // Draw a wireframe box representing the grid area
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

        // Visualize each node in the grid with colors based on walkability
        if (grid != null)
        {
            foreach (Node n in grid)
            {
                // White color for walkable nodes, red for unwalkable nodes
                Gizmos.color = (n.walkable) ? Color.white : Color.red;
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - 0.1f));  // Draw each node as a small cube
            }
        }
    }
}