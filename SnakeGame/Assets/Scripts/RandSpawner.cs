using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandSpawner : MonoBehaviour
{
    // References to different item prefabs to spawn
    public GameObject mapItemPrefab;
    public GameObject bootItemPrefab;
    public GameObject yarnItemPrefab;
    public GameObject shrinkerItemPrefab;
    public float spawnInterval = 30f; // Time interval for spawning new items

    private GameObject currentItem; // Holds the reference to the currently spawned item
    private Camera mainCamera; // Reference to the main camera for determining spawn bounds
    private Dictionary<GameObject, float> itemProbabilities; // Dictionary to hold each item's probability

    private void Start()
    {
        // Set mainCamera to Camera.main if not manually assigned
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        // Initialize item probabilities for each prefab
        itemProbabilities = new Dictionary<GameObject, float>
        {
            { mapItemPrefab, 0.25f },    // 25% chance to spawn map item
            { bootItemPrefab, 0.35f },   // 35% chance to spawn boot item
            { yarnItemPrefab, 0.25f },   // 25% chance to spawn yarn item
            { shrinkerItemPrefab, 0.15f } // 15% chance to spawn shrinker item
        };

        // Start the coroutine for periodic item spawning
        StartCoroutine(SpawnItem());
    }

    // Coroutine to handle timed item spawning
    private IEnumerator SpawnItem()
    {
        while (true)
        {
            // Wait for the specified spawn interval
            yield return new WaitForSeconds(spawnInterval);

            // Destroy the existing item, if present
            if (currentItem != null)
            {
                Destroy(currentItem);
            }

            // Spawn a new random item within the camera's view bounds
            currentItem = SpawnRandomItemWithinCamera();
        }
    }

    // Method to spawn a random item within the camera's bounds
    private GameObject SpawnRandomItemWithinCamera()
    {
        // Calculate the camera's boundaries for spawning
        float cameraHeight = mainCamera.orthographicSize;
        float cameraWidth = cameraHeight * mainCamera.aspect;

        // Generate a random position within these bounds
        Vector3 randomPosition = new Vector3(
            Random.Range(mainCamera.transform.position.x - cameraWidth, mainCamera.transform.position.x + cameraWidth),
            0.5f, // Y position is set to match the ground level
            Random.Range(mainCamera.transform.position.z - cameraHeight, mainCamera.transform.position.z + cameraHeight)
        );

        // Select a random item based on predefined probabilities
        float randomValue = Random.value; // Generates a value between 0 and 1
        float cumulativeProbability = 0f;

        // Loop through each item and check against cumulative probability
        foreach (var item in itemProbabilities)
        {
            cumulativeProbability += item.Value;
            if (randomValue <= cumulativeProbability)
            {
                // Instantiate the selected item at the random position
                GameObject spawnedItem = Instantiate(item.Key, randomPosition, Quaternion.identity);
                return spawnedItem;
            }
        }

        return null; // Fallback, though ideally never reached if probabilities sum to 1
    }
}