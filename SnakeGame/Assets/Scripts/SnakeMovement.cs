using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SnakeMovement : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float steerSpeed = 1000f;
    public int Gap = 50;
    public float BodySpeed = 1f;
    public GameObject BodyPrefab;
    public AppleSpawner appleSpawner;
    public ScoreCounter scoreCounter;
    private const float maxCameraSize = 20f; // Maximum camera size

    private List<GameObject> BodyParts = new List<GameObject>();
    private List<Vector3> PositionsHistory = new List<Vector3>();

    private float xMin, xMax, zMin, zMax;
    private bool hasEatenApple = false;
    private Dictionary<string, float> itemCooldowns = new Dictionary<string, float>(); // To track cooldowns for each item type
    private const float itemCooldownTime = 0.5f; // 0.5 seconds cooldown between item triggers

    private float startDelay = 1f; // Delay to prevent immediate game-over due to wall collision
    private bool canCollideWithWall = false;

    void Start()
    {
        StartCoroutine(EnableWallCollision()); // Start delay for wall collision
        UpdateCameraBounds(); // Calculate initial camera boundaries
        GrowSnake();

        if (scoreCounter != null)
        {
            scoreCounter.score = 0; // Initialize the score to 0
            scoreCounter.UpdateScoreText(); // Update the displayed score
        }
    }

    void Update()
    {
        // Move forward
        transform.position += transform.forward * moveSpeed * Time.deltaTime;

        float steerDirection = Input.GetAxis("Horizontal");
        transform.Rotate(Vector3.up * steerDirection * steerSpeed * Time.deltaTime);

        PositionsHistory.Insert(0, transform.position);

        int index = 0;
        foreach (var body in BodyParts)
        {
            Vector3 point = PositionsHistory[Mathf.Min(index * Gap, PositionsHistory.Count - 1)];
            Vector3 moveDirection = point - body.transform.position;
            body.transform.position += moveDirection * BodySpeed;
            body.transform.LookAt(point);
            index++;
        }

        // Ensure the snake stays within camera boundaries
        KeepWithinCameraBounds();

        // Update cooldowns for items
        UpdateItemCooldowns();
    }

    private void GrowSnake()
    {
        GameObject body = Instantiate(BodyPrefab);
        BodyParts.Add(body);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collision detected with: " + other.gameObject.name + " (Tag: " + other.tag + ")");

        // Check if the item is on cooldown
        if (IsItemOnCooldown(other.tag)) return;

        if (other.CompareTag("Food") && !hasEatenApple)
        {
            hasEatenApple = true;
            appleSpawner.RemoveApple();
            GrowSnake();
            IncreaseScore();
            Debug.Log("Collected Food item (chicken): Snake grows.");
            StartCoroutine(ResetEatenAppleFlag());
        }
        else if (other.CompareTag("MapItem"))
        {
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
            Destroy(other.gameObject);
            SetItemOnCooldown(other.tag);
        }
        else if (other.CompareTag("BootItem"))
        {
            moveSpeed += 1.5f;
            steerSpeed += 50;
            Gap -= 5;
            Debug.Log("Collected BootItem: Speed increased.");
            Destroy(other.gameObject);
            SetItemOnCooldown(other.tag);
        }
        else if (other.CompareTag("YarnItem"))
        {
            moveSpeed = Mathf.Max(1, moveSpeed - 0.2f);
            Debug.Log("Collected YarnItem: Speed decreased.");
            Destroy(other.gameObject);
            SetItemOnCooldown(other.tag);
        }
        else if (other.CompareTag("ShrinkerItem"))
        {
            int shrinkAmount = Random.Range(1, 5);
            ShrinkSnake(shrinkAmount);
            Debug.Log("Collected ShrinkerItem: Snake shrinks by " + shrinkAmount);
            Destroy(other.gameObject);
            SetItemOnCooldown(other.tag);
        }
        else if (other.gameObject.CompareTag("Wall") && canCollideWithWall)
        {
            Debug.Log("Snake hit the wall! Game Over.");
            SceneManager.LoadScene(2); // Load the Game Over scene
        }
    }

    private void IncreaseScore()
    {
        if (scoreCounter != null)
        {
            scoreCounter.score += 50;
            scoreCounter.UpdateScoreText();
            HighScore.TRY_SET_HIGH_SCORE(scoreCounter.score);
        }
    }

    private IEnumerator ResetEatenAppleFlag()
    {
        yield return new WaitForEndOfFrame();
        hasEatenApple = false;
    }

    private IEnumerator EnableWallCollision()
    {
        yield return new WaitForSeconds(startDelay);
        canCollideWithWall = true; // Enable wall collision after delay
    }

    public void UpdateCameraBounds()
    {
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
        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, xMin, xMax);
        clampedPosition.z = Mathf.Clamp(clampedPosition.z, zMin, zMax);

        transform.position = clampedPosition;
    }

    public void ShrinkSnake(int amount)
    {
        for (int i = 0; i < amount && BodyParts.Count > 0; i++)
        {
            GameObject partToRemove = BodyParts[BodyParts.Count - 1];
            BodyParts.RemoveAt(BodyParts.Count - 1);
            Destroy(partToRemove);
        }
    }

    // Check if an item is on cooldown
    private bool IsItemOnCooldown(string itemTag)
    {
        return itemCooldowns.ContainsKey(itemTag) && itemCooldowns[itemTag] > 0;
    }

    // Set an item on cooldown
    private void SetItemOnCooldown(string itemTag)
    {
        itemCooldowns[itemTag] = itemCooldownTime;
    }

    // Update cooldowns for each item type
    private void UpdateItemCooldowns()
    {
        List<string> keys = new List<string>(itemCooldowns.Keys);
        foreach (string key in keys)
        {
            itemCooldowns[key] = Mathf.Max(0, itemCooldowns[key] - Time.deltaTime);
        }
    }
}