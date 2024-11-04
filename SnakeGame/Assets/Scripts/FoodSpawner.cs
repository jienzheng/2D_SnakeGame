using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppleSpawner : MonoBehaviour
{
    public GameObject applePrefab; // Reference to the chicken/apple prefab
    public Camera mainCamera;      // Reference to the main camera
    private GameObject currentApple; // Reference to the current chicken/apple in the scene

    void Start()
    {
        SpawnApple();
    }

    public void SpawnApple()
    {
        // Check if an apple already exists
        if (currentApple != null)
        {
            return; // Do not spawn a new apple if one already exists
        }

        if (mainCamera == null)
        {
            Debug.LogWarning("Main camera not assigned in AppleSpawner.");
            return;
        }

        // Calculate boundaries based on the camera's orthographic size and aspect ratio
        float cameraHeight = mainCamera.orthographicSize;
        float cameraWidth = cameraHeight * mainCamera.aspect;

        // Get the camera's position
        Vector3 cameraPosition = mainCamera.transform.position;

        // Define spawn boundaries based on the camera's size and position
        float minX = cameraPosition.x - cameraWidth;
        float maxX = cameraPosition.x + cameraWidth;
        float minZ = cameraPosition.z - cameraHeight;
        float maxZ = cameraPosition.z + cameraHeight;

        // Generate a random position within these boundaries
        Vector3 randomPosition = new Vector3(
            Random.Range(minX, maxX), // X position within bounds
            0.5f,                     // Fixed Y position
            Random.Range(minZ, maxZ)  // Z position within bounds
        );

        // Instantiate the apple at the calculated random position and store a reference to it
        currentApple = Instantiate(applePrefab, randomPosition, Quaternion.identity);
    }

    public void RemoveApple()
    {
        // Called when the apple is eaten
        if (currentApple != null)
        {
            Destroy(currentApple);
            currentApple = null;
        }

        // Spawn a new apple after removing the old one
        SpawnApple();
    }
}