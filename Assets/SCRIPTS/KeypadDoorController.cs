using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class KeypadDoorController : MonoBehaviour
{
    public string correctCode = "1"; // Set your desired code
    public GameObject door;
    public Cinemachine.CinemachineVirtualCamera mainCamera;
    public Cinemachine.CinemachineVirtualCamera keypadCamera;
    public float proximityDistance = 3f; // Adjust the distance for player proximity
    public LayerMask keypadButtonLayer; // Set the layer for the keypad buttons
    public AudioClip hoverSound; // Sound when hovering over a button
    public Color hoverColor = Color.yellow; // Color when hovering over a button
    private bool isKeypadActive = false;
    private Material originalMaterial; // Store the original material of the button
    private bool isHovering = false;

    private string enteredCode = "";
  private void Start()
{
    // Subscribe to button click events and add hover callbacks
    KeypadButton[] keypadButtons = FindObjectsOfType<KeypadButton>(true);
    foreach (KeypadButton button in keypadButtons)
    {
        Button uiButton = button.gameObject.GetComponent<Button>();

        // Check if a Button component is present before proceeding
        if (uiButton != null)
        {
            uiButton.onClick.AddListener(() => OnKeypadButtonClick(button.Value));

            EventTrigger eventTrigger = uiButton.gameObject.AddComponent<EventTrigger>();
            AddHoverCallbacks(eventTrigger, uiButton);
        }
        else
        {
            Debug.LogError("Button component not found on KeypadButton object: " + button.gameObject.name);
        }
    }
}



private void AddHoverCallbacks(EventTrigger eventTrigger, Button button)
    {
        // Add the OnPointerEnter callback
        EventTrigger.Entry entryEnter = new EventTrigger.Entry();
        entryEnter.eventID = EventTriggerType.PointerEnter;
        entryEnter.callback.AddListener((data) => { OnButtonHoverEnter(button); });
        eventTrigger.triggers.Add(entryEnter);

        // Add the OnPointerExit callback
        EventTrigger.Entry entryExit = new EventTrigger.Entry();
        entryExit.eventID = EventTriggerType.PointerExit;
        entryExit.callback.AddListener((data) => { OnButtonHoverExit(button); });
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
                OnButtonHoverEnter(keypadButton.GetComponent<Button>());
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
            OnButtonHoverExit(hit.collider.GetComponent<Button>());
        }
    }
    else if (isHovering)
    {
        Debug.Log("Mouse not over any button.");
        OnButtonHoverExit(null);
    }
}

private void OnButtonHoverEnter(Button button)
{
    isHovering = true;

    // Play sound
    if (hoverSound != null)
    {
        AudioSource.PlayClipAtPoint(hoverSound, Camera.main.transform.position);
    }

    // Change color
    if (button != null)
    {
        // Assuming you have a Renderer component on your 3D button
        Renderer buttonRenderer = button.GetComponent<Renderer>();

        if (buttonRenderer != null)
        {
            originalMaterial = buttonRenderer.material;
            buttonRenderer.material.color = hoverColor;
        }
        else
        {
            Debug.LogError("Renderer component is null in OnButtonHoverEnter for button: " + button.gameObject.name);
        }
    }
    else
    {
        Debug.LogError("Button is null in OnButtonHoverEnter.");
    }
}



private void OnButtonHoverExit(Button button)
{
    isHovering = false;

    // Restore original color
    if (button != null)
    {
        // Assuming you have a Renderer component on your 3D button
        Renderer buttonRenderer = button.GetComponent<Renderer>();

        if (buttonRenderer != null)
        {
            buttonRenderer.material = originalMaterial;
        }
        else
        {
            Debug.LogError("Renderer component is null in OnButtonHoverExit for button: " + button.gameObject.name);
        }
    }
    else
    {
        Debug.Log("Button is null in OnButtonHoverExit.");
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
        // Implement your door animation logic here
        // Example: door.GetComponent<Animator>().SetTrigger("Open");
    }


}