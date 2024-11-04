using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotMovement : MonoBehaviour
{
    public float moveSpeed = 5f;                  // Movement speed of the bot
    public Transform spawnPoint;                  // The point where the bot respawns
    public GameObject bodyPrefab;                 // Prefab for each body part of the bot snake
    private Camera mainCamera;                    // Reference to the main camera for boundary checks

    private List<GameObject> bodyParts = new List<GameObject>();  // List of body parts
    private List<Vector3> positionsHistory = new List<Vector3>(); // List of previous positions for body movement
    private bool isRespawning = false;            // Flag to prevent multiple respawn attempts
    private float respawnCooldown = 2f;           // Delay in seconds between respawns
    private int bodySpacing = 10;                 // Controls the distance between body parts

    [SerializeField] private float viewportMargin = 0.05f;   // Margin for camera boundary check

    void Start()
    {
        mainCamera = Camera.main;

        if (spawnPoint == null)
        {
            Debug.LogWarning("Spawn point is not set for the bot.");
        }

        // Start with one initial body part
        AddBodyPart();
    }

    void Update()
    {
        Vector3 movementDirection = Vector3.right;

        // Rotate the bot to face the movement direction smoothly
        if (movementDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movementDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * moveSpeed);
        }

        // Move the bot head in the specified direction
        transform.position += movementDirection * moveSpeed * Time.deltaTime;

        positionsHistory.Insert(0, transform.position);

        // Limit history size to match the body part spacing
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

        // Respawn if the bot is out of the camera's view
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
            body.tag = "BotSegment"; // Tag the body part for collision detection
            bodyParts.Add(body);
        }
        else
        {
            Debug.LogWarning("Body prefab is not assigned for the bot.");
        }
    }

    private bool IsWithinCameraView()
    {
        if (mainCamera == null) return false;

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
        // Increase the spacing between body parts slightly on each respawn
        bodySpacing += 5;
        
        // Add only one new body part on respawn
        AddBodyPart();

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

        UpdateBodyPartsPosition();
    }

    private void UpdateBodyPartsPosition()
    {
        // Update each body part's position based on the updated history and spacing
        for (int i = 0; i < bodyParts.Count; i++)
        {
            GameObject body = bodyParts[i];
            Vector3 point = positionsHistory[Mathf.Min(i * bodySpacing, positionsHistory.Count - 1)];
            body.transform.position = point;
        }
    }
}