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
    public LayerMask Wall; // LayerMask for walls to avoid spawning near them
    public float minDistanceFromWall = 0.5f; // Minimum distance from walls for spawning items

    private void Start()
    {
        if (mainCamera == null) mainCamera = Camera.main;

        // Initialize item probabilities for each prefab
        itemProbabilities = new Dictionary<GameObject, float>
        {
            { mapItemPrefab, 0.25f },
            { bootItemPrefab, 0.25f },
            { yarnItemPrefab, 0.25f },
            { shrinkerItemPrefab, 0.25f }
        };

        // Start the coroutine for periodic item spawning
        StartCoroutine(SpawnItem());
    }

    private IEnumerator SpawnItem()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (currentItem != null) Destroy(currentItem);

            // Spawn a new random item within the camera's view bounds
            currentItem = SpawnRandomItemWithinCamera();
        }
    }

    private GameObject SpawnRandomItemWithinCamera()
    {
        float cameraHeight = mainCamera.orthographicSize;
        float cameraWidth = cameraHeight * mainCamera.aspect;

        Vector3 randomPosition;
        bool validPosition;

        do
        {
            validPosition = true;
            randomPosition = new Vector3(
                Random.Range(mainCamera.transform.position.x - cameraWidth, mainCamera.transform.position.x + cameraWidth),
                0.5f,
                Random.Range(mainCamera.transform.position.z - cameraHeight, mainCamera.transform.position.z + cameraHeight)
            );

            // Check for walls within minDistanceFromWall
            Collider[] colliders = Physics.OverlapSphere(randomPosition, minDistanceFromWall, Wall);
            if (colliders.Length > 0) validPosition = false;

        } while (!validPosition);

        float randomValue = Random.value;
        float cumulativeProbability = 0f;

        foreach (var item in itemProbabilities)
        {
            cumulativeProbability += item.Value;
            if (randomValue <= cumulativeProbability)
            {
                Quaternion spawnRotation = item.Key == bootItemPrefab ? Quaternion.identity : Quaternion.Euler(-90f, 0f, 0f);
                GameObject spawnedItem = Instantiate(item.Key, randomPosition, spawnRotation);
                return spawnedItem;
            }
        }

        return null;
    }
}