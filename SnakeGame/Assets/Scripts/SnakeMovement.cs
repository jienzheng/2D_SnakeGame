using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SnakeMovement : MonoBehaviour
{
    // Public variables for movement, steering, body gap, and body speed
    public float moveSpeed = 1f;
    public float steerSpeed = 1000f;
    public int Gap = 50; // Gap between each body parts
    public float BodySpeed = 1f;
    public GameObject BodyPrefab; // Prefab for snake body parts
    public FoodSpawner foodSpawner; // Reference to food spawner for removing food
    public ScoreCounter scoreCounter; // Reference to score counter UI
    private const float maxCameraSize = 20f; // Maximum camera zoom out limit

    // List to store snake body parts and history of positions for body parts to follow
    private List<GameObject> BodyParts = new List<GameObject>();
    private List<Vector3> PositionsHistory = new List<Vector3>();

    // Variables for camera bounds
    private float xMin, xMax, zMin, zMax;
    private bool hasEatenFood = false; // Flag to prevent eating multiple food at once
    private Dictionary<string, float> itemCooldowns = new Dictionary<string, float>();
    private const float itemCooldownTime = 0.5f; // Cooldown time for items

    // Audio components
    public AudioSource audioSource;
    public AudioClip eatChickenSound; // Sound for eating food
    public AudioClip specialItemSound; // Sound for collecting special items

    void Start()
    {
        // Calculate camera bounds and initialize snake body
        UpdateCameraBounds();
        GrowSnake(); //Initialize a body segment of the snake
        
        // Initialize score counter if available
        if (scoreCounter != null)
        {
            scoreCounter.score = 0;
            scoreCounter.UpdateScoreText();
        }

        // Ensure audioSource is assigned, add component if not
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        // Move forward in the current direction
        transform.position += transform.forward * moveSpeed * Time.deltaTime;

        // Rotate the snake based on player input
        float steerDirection = Input.GetAxis("Horizontal");
        transform.Rotate(Vector3.up * steerDirection * steerSpeed * Time.deltaTime);

        // Add the current position to position history
        PositionsHistory.Insert(0, transform.position);

        // Move each body part based on position history
        int index = 0;
        foreach (var body in BodyParts)
        {
            Vector3 point = PositionsHistory[Mathf.Min(index * Gap, PositionsHistory.Count - 1)];
            Vector3 moveDirection = point - body.transform.position;
            body.transform.position += moveDirection * BodySpeed;
            body.transform.LookAt(point);
            index++;
        }

        // Ensure the snake remains within camera bounds
        KeepWithinCameraBounds();

        // Update cooldown timers for items
        UpdateItemCooldowns();
    }

    private void GrowSnake()
    {
        // Instantiate a new body part and add it to the list
        GameObject body = Instantiate(BodyPrefab);
        BodyParts.Add(body);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Collision Detected: {other.gameObject.tag} with {this.gameObject.tag}");
        
        // Skip processing if the item is on cooldown
        if (IsItemOnCooldown(other.tag)) return;

        // Check for different item types based on tag
        if (other.CompareTag("Food") && !hasEatenFood)
        {
            // Handle eating food
            hasEatenFood = true;
            foodSpawner.RemoveFood();
            GrowSnake();
            IncreaseScore();
            audioSource.PlayOneShot(eatChickenSound); // Play eat sound
            Debug.Log("Collected Food item (chicken): Snake grows.");
            StartCoroutine(ResetEatenFoodFlag());
        }
        else if (other.CompareTag("MapItem"))
        {
            // Increase camera zoom size if below limit
            if (Camera.main.orthographicSize < maxCameraSize)
            {
                Camera.main.orthographicSize += 1;
                UpdateCameraBounds();
                Debug.Log("Collected MapItem: Camera size increased.");
            }
            else
            {
                Debug.Log("Camera size limit reached. Cannot increase further.");
            }
            audioSource.PlayOneShot(specialItemSound);
            Destroy(other.gameObject); // Destroy item after collection
            SetItemOnCooldown(other.tag);
        }
        else if (other.CompareTag("BootItem"))
        {
            // Increase speed, steerSpeed, Gap and play sound
            moveSpeed += 1.5f;
            steerSpeed += 50;
            Gap -= 5;
            audioSource.PlayOneShot(specialItemSound);
            Debug.Log("Collected BootItem: Speed increased.");
            Destroy(other.gameObject);
            SetItemOnCooldown(other.tag);
        }
        else if (other.CompareTag("YarnItem"))
        {
            // Decrease speed and play sound
            moveSpeed = Mathf.Max(1, moveSpeed - 0.6f);
            audioSource.PlayOneShot(specialItemSound);
            Debug.Log("Collected YarnItem: Speed decreased.");
            Destroy(other.gameObject);
            SetItemOnCooldown(other.tag);
        }
        else if (other.CompareTag("ShrinkerItem"))
        {
            // Shrink snake and play sound
            int shrinkAmount = Random.Range(1, 3);
            ShrinkSnake(shrinkAmount);
            audioSource.PlayOneShot(specialItemSound);
            Debug.Log("Collected ShrinkerItem: Snake shrinks by " + shrinkAmount);
            Destroy(other.gameObject);
            SetItemOnCooldown(other.tag);
        }
        else if (other.CompareTag("Wall") && this.CompareTag("PlayerSnake"))
        {
            // Handle player snake hitting a wall (game over)
            Debug.Log("Player snake hit the wall! Game Over.");
            SceneManager.LoadScene(2); // Load Game Over scene
        }
        else if (other.CompareTag("Bot") && this.CompareTag("PlayerSnake"))
        {
            // Handle player snake hitting the bot (game over)
            Debug.Log("Player snake hit the bot! Game Over.");
            SceneManager.LoadScene(2); // Load Game Over scene
        }
        else if (other.CompareTag("BotSegment") && this.CompareTag("PlayerSnake"))
        {
            // Game over if the player hits any segment of the bot
            Debug.Log("Player hit the bot segment! Game Over.");
            SceneManager.LoadScene(2); // Load the Game Over scene
        }
    }

    private void IncreaseScore()
    {
        // Increase score and update high score if necessary
        if (scoreCounter != null)
        {
            scoreCounter.score += 100;
            scoreCounter.UpdateScoreText();
            HighScore.TRY_SET_HIGH_SCORE(scoreCounter.score);
        }
    }

    private IEnumerator ResetEatenFoodFlag()
    {
        // Reset the food eaten flag after a frame to prevent immediate re-eating
        yield return new WaitForEndOfFrame();
        hasEatenFood = false;
    }

    public void UpdateCameraBounds()
    {
        // Calculate the camera boundaries for keeping the snake in bounds
        // Makes sure that the snake stays within the camera's pov
        Camera mainCamera = Camera.main;
        float cameraHeight = mainCamera.orthographicSize;
        float cameraWidth = cameraHeight * mainCamera.aspect;

        Vector3 cameraPosition = mainCamera.transform.position;

        xMin = cameraPosition.x - cameraWidth;
        xMax = cameraPosition.x + cameraWidth;
        zMin = cameraPosition.z - cameraHeight;
        zMax = cameraPosition.z + cameraHeight;
    }

    private void KeepWithinCameraBounds()
    {
        // Clamp the snake's position within the calculated camera bounds
        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, xMin, xMax);
        clampedPosition.z = Mathf.Clamp(clampedPosition.z, zMin, zMax);

        transform.position = clampedPosition;
    }

    public void ShrinkSnake(int amount)
    {
        // Remove the specified amount of body parts from the snake
        for (int i = 0; i < amount && BodyParts.Count > 0; i++)
        {
            GameObject partToRemove = BodyParts[BodyParts.Count - 1];
            BodyParts.RemoveAt(BodyParts.Count - 1);
            Destroy(partToRemove);
        }
    }

    private bool IsItemOnCooldown(string itemTag)
    {
        // Check if an item is still on cooldown based on tag
        return itemCooldowns.ContainsKey(itemTag) && itemCooldowns[itemTag] > 0;
    }

    private void SetItemOnCooldown(string itemTag)
    {
        // Set the cooldown for a specific item tag
        itemCooldowns[itemTag] = itemCooldownTime;
    }

    private void UpdateItemCooldowns()
    {
        // Reduce the cooldown time for each item over time
        List<string> keys = new List<string>(itemCooldowns.Keys);
        foreach (string key in keys)
        {
            itemCooldowns[key] = Mathf.Max(0, itemCooldowns[key] - Time.deltaTime);
        }
    }
}