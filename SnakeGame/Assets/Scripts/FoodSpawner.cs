using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    public GameObject foodPrefab; // Reference to the food prefab to be spawned
    public Camera mainCamera;     // Reference to the main camera for defining spawn boundaries
    private GameObject currentFood; // Reference to the current food instance in the scene

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
        float cameraHeight = mainCamera.orthographicSize; // Half the height of the camera's view
        float cameraWidth = cameraHeight * mainCamera.aspect; // Half the width, accounting for aspect ratio

        // Get the camera's current position in the world
        Vector3 cameraPosition = mainCamera.transform.position;

        // Define the spawn boundaries based on the camera's position and calculated width/height
        float minX = cameraPosition.x - cameraWidth;
        float maxX = cameraPosition.x + cameraWidth;
        float minZ = cameraPosition.z - cameraHeight;
        float maxZ = cameraPosition.z + cameraHeight;

        // Generate a random position within these boundaries for the new food item
        Vector3 randomPosition = new Vector3(
            Random.Range(minX, maxX), // Random X position within bounds
            0.5f,                     // Fixed Y position for ground level
            Random.Range(minZ, maxZ)  // Random Z position within bounds
        );

        // Instantiate the food prefab at the calculated random position and store it as the current food
        currentFood = Instantiate(foodPrefab, randomPosition, Quaternion.identity);
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