using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public PathfindingGrid grid; // Reference to the grid that contains nodes for pathfinding

    void Awake()
    {
        // Initialize the grid component on Awake
        grid = GetComponent<PathfindingGrid>();
    }

    // Finds a path from the start position to the target position and returns it as a list of Vector3 points
    public List<Vector3> FindPath(Vector3 startPos, Vector3 targetPos)
    {
        // Convert start and target positions to nodes in the grid
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        List<Node> openSet = new List<Node>(); // Nodes to be evaluated
        HashSet<Node> closedSet = new HashSet<Node>(); // Nodes already evaluated
        openSet.Add(startNode); // Start with the start node in the open set

        // Main loop to process nodes until the open set is empty or path is found
        while (openSet.Count > 0)
        {
            // Find the node in the open set with the lowest fCost
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || 
                    (openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost))
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode); // Remove the current node from the open set
            closedSet.Add(currentNode); // Add it to the closed set as it has been processed

            // If we've reached the target node, retrace the path and return it
            if (currentNode == targetNode)
            {
                return RetracePath(startNode, targetNode);
            }

            // Process each neighbor of the current node
            foreach (Node neighbour in grid.GetNeighbours(currentNode))
            {
                // Ignore non-walkable neighbors or those already processed
                if (!neighbour.walkable || closedSet.Contains(neighbour))
                {
                    continue;
                }

                // Calculate new movement cost to the neighboring node
                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    // Update neighbor's cost and set its parent to the current node
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    // If neighbor is not in the open set, add it for evaluation
                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }

        // Return null if no path was found
        return null;
    }

    // Retraces the path from the end node to the start node and converts it to a list of Vector3 waypoints
    List<Vector3> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        // Trace back from the end node to the start node
        while (currentNode != startNode)
        {
            path.Add(currentNode); // Add current node to the path
            currentNode = currentNode.parent; // Move to the parent node
        }
        path.Reverse(); // Reverse the path to start from the start node

        // Convert the Node path to a Vector3 path
        List<Vector3> waypoints = new List<Vector3>();
        foreach (Node node in path)
        {
            waypoints.Add(node.worldPosition); // Add the world position of each node to the waypoints
        }
        return waypoints; // Return the calculated waypoints
    }

    // Calculates the distance between two nodes using Manhattan or diagonal distance method
    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX); // Horizontal distance
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY); // Vertical distance

        // Use diagonal distance calculation
        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY); // Moving diagonally costs 14, straight costs 10
        return 14 * dstX + 10 * (dstY - dstX);
    }
}