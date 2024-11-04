using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Transform spawnPoint; // The point where the bot respawns
    public GameObject bodyPrefab; // Prefab for each body part of the bot snake
    private Camera mainCamera;

    private List<GameObject> bodyParts = new List<GameObject>();
    private List<Vector3> positionsHistory = new List<Vector3>();

    private bool isRespawning = false; // Tracks if the bot is in a respawn cooldown
    private float respawnCooldown = 2f; // Delay in seconds between respawns

    void Start()
    {
        mainCamera = Camera.main;

        if (spawnPoint == null)
        {
            Debug.LogWarning("Spawn point is not set for the bot.");
        }

        // Spawn initial bot body parts
        for (int i = 0; i < 12; i++)
        {
            AddBodyPart();
        }
    }

    void Update()
    {
        // Move the bot continuously in the positive x-direction
        transform.position += Vector3.right * moveSpeed * Time.deltaTime;

        // Add the current position to history
        positionsHistory.Insert(0, transform.position);

        // Move each body part
        for (int i = 0; i < bodyParts.Count; i++)
        {
            GameObject body = bodyParts[i];
            Vector3 point = positionsHistory[Mathf.Min(i * 10, positionsHistory.Count - 1)];
            body.transform.position = point;
        }

        // Check if the bot is within camera bounds, respawn if it’s not
        if (!IsWithinCameraView() && !isRespawning)
        {
            StartCoroutine(RespawnWithCooldown());
        }
    }

    private void AddBodyPart()
    {
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
        if (mainCamera == null) return false;

        Vector3 viewportPos = mainCamera.WorldToViewportPoint(transform.position);

        // Check if the bot is within the horizontal and vertical bounds of the camera
        return viewportPos.x > 0 && viewportPos.x < 1 && viewportPos.y > 0 && viewportPos.y < 1;
    }

    private IEnumerator RespawnWithCooldown()
    {
        isRespawning = true;
        Respawn();
        yield return new WaitForSeconds(respawnCooldown);
        isRespawning = false;
    }

    private void Respawn()
    {
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