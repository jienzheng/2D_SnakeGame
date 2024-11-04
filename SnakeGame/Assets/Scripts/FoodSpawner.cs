using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    public GameObject foodPrefab; // Reference to the food prefab to be spawned
    public Camera mainCamera;     // Reference to the main camera for defining spawn boundaries
    private GameObject currentFood; // Reference to the current food instance in the scene
    public LayerMask Wall;        // Layer mask for walls to avoid spawning food inside

    void Start()
    {
        // Spawn initial food when the game starts
        SpawnFood();
    }

    public void SpawnFood()
    {
        // Check if a food item already exists in the scene
        if (currentFood != null)
        {
            return; // Do not spawn a new food item if one already exists
        }

        // Check if the main camera is assigned
        if (mainCamera == null)
        {
            Debug.LogWarning("Main camera not assigned in FoodSpawner.");
            return;
        }

        // Calculate the camera's visible area boundaries for spawning the food
        float cameraHeight = mainCamera.orthographicSize;
        float cameraWidth = cameraHeight * mainCamera.aspect;

        Vector3 cameraPosition = mainCamera.transform.position;
        float margin = 0.5f; // Margin to keep food away from walls

        float minX = cameraPosition.x - cameraWidth + margin;
        float maxX = cameraPosition.x + cameraWidth - margin;
        float minZ = cameraPosition.z - cameraHeight + margin;
        float maxZ = cameraPosition.z + cameraHeight - margin;

        bool validPositionFound = false;
        Vector3 randomPosition = Vector3.zero;

        // Try multiple times to find a valid position
        for (int attempts = 0; attempts < 10 && !validPositionFound; attempts++)
        {
            // Generate a random position within the defined boundaries
            randomPosition = new Vector3(
                Random.Range(minX, maxX),
                0.5f, // Fixed Y position for ground level
                Random.Range(minZ, maxZ)
            );

            // Check for any overlap with walls using a small sphere cast
            if (!Physics.CheckSphere(randomPosition, margin, Wall))
            {
                validPositionFound = true; // Found a valid spot
            }
        }

        if (validPositionFound)
        {
            // Instantiate the food at the calculated random position with the specified rotation
            currentFood = Instantiate(foodPrefab, randomPosition, Quaternion.Euler(-90f, 0f, 180f));
        }
        else
        {
            Debug.LogWarning("Could not find a valid position to spawn food away from walls.");
        }
    }

    public void RemoveFood()
    {
        // Called when the food is eaten
        if (currentFood != null)
        {
            Destroy(currentFood); // Remove the existing food item
            currentFood = null; // Clear the reference
        }

        // Spawn a new food item after removing the old one
        SpawnFood();
    }
}