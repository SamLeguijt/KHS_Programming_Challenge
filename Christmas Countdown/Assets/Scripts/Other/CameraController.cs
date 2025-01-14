using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    [Tooltip("Object to put in center of the screen, child of camera")]
    [SerializeField] private Transform centerTarget;

    [Tooltip("Forward orientation object attached to the player object")]
    [SerializeField] private Transform playerForwardOrientation;

    [Tooltip("Right hand transform of object attached to player")]
    [SerializeField] private Hand playerRightHand;

    [Tooltip("Left hand transform of object attached to player")]
    [SerializeField] private Hand playerLeftHand;

    [Tooltip("Position offsets for right hand")]
    [SerializeField] private Vector3 rightHandPosOffset;

    [Tooltip("Position offsets for  hand")]
    [SerializeField] private Vector3 leftHandPosOffset;

    [Header("Sensitivity settings")]

    [Tooltip("UI slider component for the mouse X sensitivity setting")]
    [SerializeField] private Slider sensitivityXslider;

    [Tooltip("UI slider component for the mouse Y sensitivity setting")]
    [SerializeField] private Slider sensitivityYslider;

    [SerializeField] private float defaultMouseSensitivityX;
    [SerializeField] private float defaultMouseSensitivityY;

    [Space]
    [Tooltip("Max angle the camera is allowed to move up and down")]
    [SerializeField] private float maxVerticalAngle = 90f;

    // Private variables used for setting the rotations
    private float rotationX;
    private float rotationY;

    public LayerMask equippedcullingMask;
    public Camera myCam;

    public float CameraRotationX
    {
        get { return rotationX; }
        private set { rotationX = value; }
    }

    public float CameraRotationY
    {
        get { return rotationY; }
        private set { rotationY = value; }
    }

    public Transform PlayerOrientation
    {
        get { return playerForwardOrientation; }
    }

    public Transform CenterTarget
    {
        get { return centerTarget; }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Lock the cursor on start 
        Cursor.lockState = CursorLockMode.Locked;

        // Disable the hardware pointer being visible
        Cursor.visible = false;

        // Set values of sliders to the default values
        sensitivityXslider.value = defaultMouseSensitivityX;
        sensitivityYslider.value = defaultMouseSensitivityY;
    }

    // Update is called once per frame
    void Update()
    {
        // Return if the settings menu is open to stop rotating
        if (SettingsManager.instance.IsOpenSettingsMenu()) return;


        Rotate();
        MaintainPositions();
    }

    /// <summary>
    /// Method for rotating the object according to mouse inputs
    /// Rotates this game object and the player's orientation object. 
    /// </summary>
    private void Rotate()
    {
        // Get mouse inputs
        float mouseX = Input.GetAxisRaw("Mouse X");
        float mouseY = Input.GetAxisRaw("Mouse Y");

        // Get our targets by multiplying with delta time and sensitivity of sliders
        float targetX = mouseX * Time.deltaTime * sensitivityXslider.value;
        float targetY = mouseY * Time.deltaTime * sensitivityYslider.value;

        // Add target values to our player rotation variables
        rotationY += targetX;
        rotationX -= targetY;

        // Clamp the vertical rotation to x degrees
        rotationX = Mathf.Clamp(rotationX, -maxVerticalAngle, maxVerticalAngle);

        // Rotate this object around the target rotations.
        transform.rotation = Quaternion.Euler(rotationX, rotationY, 0);

        // Rotate the player's orientation object around the target rotations.
        playerForwardOrientation.rotation = Quaternion.Euler(0, rotationY, 0);
    }

    /// <summary>
    /// Method for staying in the correct positions
    /// Sets the position of the camera to the player's forward orientation position. 
    /// Sets the position of the player hands to the corner of the screen
    /// </summary>
    private void MaintainPositions()
    {
        // Set position of this transform to orientation object
        transform.position = playerForwardOrientation.transform.position;
        // Get the worldposition of the camera view for both hands (offsets)
        Vector3 worldPositionLeftHand = Camera.main.ViewportToWorldPoint(leftHandPosOffset);
        Vector3 worldPositionRightHand = Camera.main.ViewportToWorldPoint(rightHandPosOffset);

        // Get position of the center of the camera's view, z distnce away from camera.
        Vector3 centerPosition = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 5f));

        // Set position of the hands to the viewport positions
        playerLeftHand.transform.position = worldPositionLeftHand;
        playerRightHand.transform.position = worldPositionRightHand;
        centerTarget.transform.position = centerPosition;

    }
}
