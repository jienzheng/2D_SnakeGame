using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandSpawner : MonoBehaviour
{
    public GameObject mapItemPrefab;
    public GameObject bootItemPrefab;
    public GameObject yarnItemPrefab;
    public GameObject shrinkerItemPrefab;
    public float spawnInterval = 30f; // Time interval for spawning new items

    private GameObject currentItem; // Reference to the currently spawned item
    private Camera mainCamera;
    private Dictionary<GameObject, float> itemProbabilities;

    private void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main; // Fallback to Camera.main if not assigned
        }

        // Initialize item probabilities
        itemProbabilities = new Dictionary<GameObject, float>
        {
            { mapItemPrefab, 0.25f },
            { bootItemPrefab, 0.35f },
            { yarnItemPrefab, 0.25f },
            { shrinkerItemPrefab, 0.15f }
        };

        StartCoroutine(SpawnItem());
    }

    private IEnumerator SpawnItem()
    {
        while (true)
        {
            // Wait for the specified interval
            yield return new WaitForSeconds(spawnInterval);

            // Remove current item if it exists
            if (currentItem != null)
            {
                Destroy(currentItem);
            }

            // Spawn a new random item within the camera's view bounds
            currentItem = SpawnRandomItemWithinCamera();
        }
    }

    private GameObject SpawnRandomItemWithinCamera()
    {
        // Calculate camera bounds
        float cameraHeight = mainCamera.orthographicSize;
        float cameraWidth = cameraHeight * mainCamera.aspect;

        // Generate a random position within the camera's view
        Vector3 randomPosition = new Vector3(
            Random.Range(mainCamera.transform.position.x - cameraWidth, mainCamera.transform.position.x + cameraWidth),
            0.5f, // Adjust the Y position if necessary to match the gameplay plane
            Random.Range(mainCamera.transform.position.z - cameraHeight, mainCamera.transform.position.z + cameraHeight)
        );

        // Select a random item based on probabilities
        float randomValue = Random.value;
        float cumulativeProbability = 0f;

        foreach (var item in itemProbabilities)
        {
            cumulativeProbability += item.Value;
            if (randomValue <= cumulativeProbability)
            {
                // Instantiate the item at the calculated random position
                GameObject spawnedItem = Instantiate(item.Key, randomPosition, Quaternion.identity);
                return spawnedItem;
            }
        }

        return null; // Fallback (should never reach here if probabilities sum to 1)
    }
}