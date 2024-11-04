using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotMovement : MonoBehaviour
{
    // Public variables for movement speed, spawn point, and body part prefab
    public float moveSpeed = 5f;
    public Transform spawnPoint; // The point where the bot respawns
    public GameObject bodyPrefab; // Prefab for each body part of the bot snake
    private Camera mainCamera; // Reference to the main camera for boundary checks

    // Private variables for managing bot body parts and movement history
    private List<GameObject> bodyParts = new List<GameObject>(); // List of body parts
    private List<Vector3> positionsHistory = new List<Vector3>(); // List of previous positions for body movement
    private bool isRespawning = false; // Flag to prevent multiple respawn attempts
    private float respawnCooldown = 2f; // Delay in seconds between respawns
    
    [SerializeField] private int bodySpacing = 10; // Controls the distance between body parts
    [SerializeField] private float viewportMargin = 0.05f; // Margin for camera boundary check

    void Start()
    {
        // Initialize main camera reference
        mainCamera = Camera.main;

        // Warn if the spawn point is missing
        if (spawnPoint == null)
        {
            Debug.LogWarning("Spawn point is not set for the bot.");
        }

        // Add initial body parts to the bot
        for (int i = 0; i < 12; i++)
        {
            AddBodyPart();
        }
    }

    void Update()
    {
        // Move the bot head in the positive x-direction
        transform.position += Vector3.right * moveSpeed * Time.deltaTime;

        // Record the current position for body part alignment
        positionsHistory.Insert(0, transform.position);

        // Limit the history size for memory efficiency, removing older positions if needed
        if (positionsHistory.Count > bodyParts.Count * bodySpacing)
        {
            positionsHistory.RemoveAt(positionsHistory.Count - 1);
        }

        // Update each body part's position based on position history
        for (int i = 0; i < bodyParts.Count; i++)
        {
            GameObject body = bodyParts[i];
            Vector3 point = positionsHistory[Mathf.Min(i * bodySpacing, positionsHistory.Count - 1)];
            body.transform.position = point;
        }

        // Check if the bot head is within the camera's view, respawn if it's out of bounds
        if (!IsWithinCameraView() && !isRespawning)
        {
            StartCoroutine(RespawnWithCooldown());
        }
    }

    private void AddBodyPart()
    {
        // Instantiate a new body part and add it to the list of body parts
        if (bodyPrefab != null)
        {
            GameObject body = Instantiate(bodyPrefab, transform.position, Quaternion.identity);
            bodyParts.Add(body);
        }
        else
        {
            Debug.LogWarning("Body prefab is not assigned for the bot.");
        }
    }

    private bool IsWithinCameraView()
    {
        // Return false if the main camera is not assigned
        if (mainCamera == null) return false;

        // Get the position of the bot head in viewport coordinates
        Vector3 viewportPos = mainCamera.WorldToViewportPoint(transform.position);

        // Check if the bot is within the bounds of the camera view with a margin
        return viewportPos.x > -viewportMargin && viewportPos.x < 1 + viewportMargin 
               && viewportPos.y > -viewportMargin && viewportPos.y < 1 + viewportMargin;
    }

    private IEnumerator RespawnWithCooldown()
    {
        // Set the respawning flag to prevent multiple respawn attempts
        isRespawning = true;
        Respawn(); // Call the respawn function

        // Wait for the cooldown period before allowing another respawn
        yield return new WaitForSeconds(respawnCooldown);
        isRespawning = false;
    }

    private void Respawn()
    {
        // Set the bot's position and rotation to the spawn point location
        if (spawnPoint != null)
        {
            transform.position = spawnPoint.position;
            transform.rotation = spawnPoint.rotation;
            Debug.Log("Bot respawned at spawn point.");
        }
        else
        {
            Debug.LogWarning("No spawn point assigned for the bot.");
        }
    }
}