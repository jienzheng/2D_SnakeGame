using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeMovement : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float steerSpeed = 1000;
    public int Gap = 50;
    public float BodySpeed = 1f;
    public GameObject BodyPrefab;
    public AppleSpawner appleSpawner;

    private List<GameObject> BodyParts = new List<GameObject>();
    private List<Vector3> PositionsHistory = new List<Vector3>();

    // Camera boundaries
    private float xMin, xMax, zMin, zMax;

    void Start()
    {
        CalculateCameraBounds(); // Calculate the camera boundaries based on the camera position
    }

    void Update()
    {
        // Move forward
        transform.position += transform.forward * moveSpeed * Time.deltaTime;

        float steerDirection = Input.GetAxis("Horizontal");
        transform.Rotate(Vector3.up * steerDirection * steerSpeed * Time.deltaTime);

        PositionsHistory.Insert(0, transform.position);

        int index = 0;
        foreach (var body in BodyParts) {
            Vector3 point = PositionsHistory[Mathf.Min(index * Gap, PositionsHistory.Count - 1)];
            Vector3 moveDirection = point - body.transform.position;
            body.transform.position += moveDirection * BodySpeed;
            body.transform.LookAt(point);
            index++;
        }

        // Ensure the snake stays within camera boundaries
        KeepWithinCameraBounds();
    }

    private void GrowSnake() {
        GameObject body = Instantiate(BodyPrefab);
        BodyParts.Add(body);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Food")) // Check if the collided object is tagged as "Apple"
        {
            appleSpawner.RemoveApple(); // Remove the eaten apple and spawn a new one
            GrowSnake(); // Grow the snake by one body part
        }
    }

    // Calculate camera bounds based on the main orthographic camera
    private void CalculateCameraBounds()
    {
        Camera mainCamera = Camera.main;
        float cameraHeight = mainCamera.orthographicSize;
        float cameraWidth = cameraHeight * mainCamera.aspect;

        Vector3 cameraPosition = mainCamera.transform.position;

        // Set the X boundaries using camera width, and Z boundaries using camera height
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
}