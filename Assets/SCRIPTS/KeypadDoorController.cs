using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class KeypadDoorController : MonoBehaviour
{
    public string correctCode = "1"; // Set your desired code
    public GameObject door;
    public Cinemachine.CinemachineVirtualCamera mainCamera;
    public Cinemachine.CinemachineVirtualCamera keypadCamera;
    public float proximityDistance = 3f; // Adjust the distance for player proximity
    public LayerMask keypadButtonLayer; // Set the layer for the keypad buttons
    public AudioClip hoverSound; // Sound when hovering over a button
   
    private bool isKeypadActive = false;
    private Material originalMaterial; // Store the original material of the button
    private bool isHovering = false;

    private string enteredCode = "";
    public Material normalMaterial;
    public Material hoverMaterial;
   // public Color hoverColor = Color.yellow;
    private Color originalColor;
    
    private MaterialPropertyBlock hoverMaterialPropertyBlock;
    public Color hoverColor = Color.yellow; // Color when hovering over a button
    private GameObject lastHoveredObject;
  
   private void Start()
{
    // Subscribe to button click events
    KeypadButton[] keypadButtons = FindObjectsOfType<KeypadButton>(true);
    foreach (KeypadButton button in keypadButtons)
    {
        Button uiButton = button.gameObject.GetComponent<Button>();

        // Check if a Button component is present before proceeding
        if (uiButton != null)
        {
            uiButton.onClick.AddListener(() => OnKeypadButtonClick(button.Value));

            // Add EventTrigger to handle hover events
            EventTrigger eventTrigger = uiButton.gameObject.AddComponent<EventTrigger>();
            AddHoverCallbacks(eventTrigger, button.gameObject);
        }
        else
        {
            Debug.LogError("Button component not found on KeypadButton object: " + button.gameObject.name);
        }
    }
}



private void AddHoverCallbacks(EventTrigger eventTrigger, GameObject buttonObject)
{
    // Add the OnPointerEnter callback
    EventTrigger.Entry entryEnter = new EventTrigger.Entry();
    entryEnter.eventID = EventTriggerType.PointerEnter;
    entryEnter.callback.AddListener((data) => { OnButtonHoverEnter(buttonObject); });
    eventTrigger.triggers.Add(entryEnter);

    // Add the OnPointerExit callback
    EventTrigger.Entry entryExit = new EventTrigger.Entry();
    entryExit.eventID = EventTriggerType.PointerExit;
    entryExit.callback.AddListener((data) => { OnButtonHoverExit(buttonObject); });
    eventTrigger.triggers.Add(entryExit);
}


    private void Update()
    {
        // Check for player proximity and 'Q' key press
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, proximityDistance);

            foreach (var collider in colliders)
            {
                if (collider.CompareTag("Player"))
                {
                    ToggleKeypadActivation();
                    break;
                }
            }
        }

        // Check for input only when the keypad is active
        if (isKeypadActive)
        {
            HandleMouseInput();
        }
    }

    private void HandleMouseInput()
{
    // Lock and make the cursor visible when interacting with the keypad
    Cursor.lockState = CursorLockMode.None;
    Cursor.visible = true;

    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    RaycastHit hit;

    // Raycast only on objects in the specified layer (keypadButtonLayer)
   if (Physics.Raycast(ray, out hit, Mathf.Infinity, keypadButtonLayer))
    {
        KeypadButton keypadButton = hit.collider.GetComponent<KeypadButton>();

        if (keypadButton != null)
        {
            if (!isHovering)
            {
                Debug.Log("Mouse hovering over button: " + keypadButton.Value);
                OnButtonHoverEnter(keypadButton.gameObject); // Pass the GameObject representing the button
            }

            if (Input.GetMouseButtonDown(0)) // Assuming left mouse button is used
            {
                Debug.Log("Mouse clicked on button: " + keypadButton.Value);
                OnKeypadButtonClick(keypadButton.Value);
            }
        }
        else if (isHovering)
        {
            Debug.Log("Mouse not hovering over a button anymore.");
            OnButtonHoverExit(hit.collider.gameObject); // Pass the GameObject representing the button
        }
    }
    else if (isHovering)
    {
        Debug.Log("Mouse not over any button.");
        OnButtonHoverExit(null);
    }
}

private void OnButtonHoverEnter(GameObject buttonObject)
    {
        isHovering = true;
        lastHoveredObject = buttonObject; // Store the last hovered object

        Debug.Log("OnButtonHoverEnter called");

        // Play sound
        if (hoverSound != null)
        {
            AudioSource.PlayClipAtPoint(hoverSound, Camera.main.transform.position);
        }

        // Get the Renderer component
        Renderer buttonRenderer = buttonObject.GetComponent<Renderer>();

        if (buttonRenderer != null)
        {
            buttonRenderer.material = hoverMaterial;
        }
        else
        {
            Debug.LogError("Renderer component is null in OnButtonHoverEnter for button: " + buttonObject.name);
        }
    }


private void OnButtonHoverExit(GameObject buttonObject)
    {
        isHovering = false;

        Debug.Log("OnButtonHoverExit called");

        // Use the last hovered object if the current buttonObject is null
        GameObject exitObject = buttonObject != null ? buttonObject : lastHoveredObject;

        if (exitObject != null)
        {
            // Get the Renderer component
            Renderer buttonRenderer = exitObject.GetComponent<Renderer>();

            if (buttonRenderer != null)
            {
                buttonRenderer.material = normalMaterial;
            }
            else
            {
                Debug.LogError("Renderer component is null in OnButtonHoverExit for button: " + exitObject.name);
            }
        }
        else
        {
            Debug.LogError("Both buttonObject and lastHoveredObject are null in OnButtonHoverExit.");
        }
    }
    // Call this function when a keypad button is clicked
     public void OnKeypadButtonClick(string value)
    {
        if (isKeypadActive)
        {
            // Add the clicked button's value to the entered code
            enteredCode += value;

            Debug.Log("Button Clicked: " + value);

            // Check if the entered code matches the correct code
            if (enteredCode == correctCode)
            {
                Debug.Log("Correct code entered. Triggering animation.");

                // Trigger your animation here
                TriggerDoorAnimation();

                // Optionally, reset the entered code for subsequent attempts
                enteredCode = "";
            }
        }
    }

    private void ToggleKeypadActivation()
    {
        if (!isKeypadActive)
        {
            // Activate the keypad
            isKeypadActive = true;
            // Optionally, switch to the keypad camera
            mainCamera.Priority = 0;
            keypadCamera.Priority = 10;

            Debug.Log("Keypad activated. Use the mouse to enter the code.");
        }
        else
        {
            // Deactivate the keypad
            isKeypadActive = false;
            // Switch back to the main camera
            mainCamera.Priority = 10;
            keypadCamera.Priority = 0;

            // Lock and hide the cursor when deactivating the keypad
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            Debug.Log("Keypad deactivated.");
        }
    }
    
     private void TriggerDoorAnimation()
{
    // Assuming your door has an Animator component
    Animator doorAnimator = door.GetComponent<Animator>();

    if (doorAnimator != null)
    {
        // Set the boolean parameter to trigger the door opening animation
        doorAnimator.SetBool("IsDoorOpen", true);

        // Optionally, play a sound or perform other actions related to the door opening

        // Reset the entered code for subsequent attempts
        enteredCode = "";

        // Deactivate the keypad after successful entry (you may adjust this based on your game logic)
        ToggleKeypadActivation();

        Debug.Log("Door opening animation triggered.");
    }
    else
    {
        Debug.LogError("Animator component not found on the door GameObject.");
    }
}



}