using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class KeypadDoorController : MonoBehaviour
{
    public string correctCode = "1234"; // Set your desired code
    public GameObject door;
    public Cinemachine.CinemachineVirtualCamera mainCamera;
    public Cinemachine.CinemachineVirtualCamera keypadCamera;
    public float proximityDistance = 3f; // Adjust the distance for player proximity
    public LayerMask keypadButtonLayer; // Set the layer for the keypad buttons
    public AudioClip hoverSound; // Sound when hovering over a button

    private bool isKeypadActive = false;
    private bool isHovering = false;

    private string enteredCode = "";
    public Material normalMaterial;
    public Material hoverMaterial;
    public Color hoverColor = Color.yellow; // Color when hovering over a button
    private GameObject lastHoveredObject;

    //keypad animation
    public float buttonPressDistance = 0.01f;
    public float interpolationTime = 0.2f;
    private bool isButtonPressed = false;
    public float doorAnimationDelay = 1.0f;
    public bool allowZAxis = true;

    private int buttonPressCount = 0;
    public CorrectInputDisplay correctInputDisplay;
    public WrongInputDisplay wrongInputDisplay;
    
    private void Start()
    {
        isButtonPressed = false; // Make sure to set it to false
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
    Debug.Log("Button clicked: " + value);

    if (isKeypadActive && !isButtonPressed)
    {
        GameObject pressedButton = lastHoveredObject;

        if (pressedButton != null)
        {
            StartCoroutine(PressButton(pressedButton));
            isButtonPressed = true;

            buttonPressCount++;
            enteredCode += value;

            if (enteredCode == correctCode)
            {
                Debug.Log("Correct code entered. Triggering animation with delay.");

                // Display correct input object
                correctInputDisplay.DisplayCorrectInputObject();

                StartCoroutine(DelayedDoorAnimation());
                enteredCode = "";
            }
            else if (enteredCode.Length >= correctCode.Length)
            {
                Debug.Log("Wrong code entered. Resetting the code and marking objects for deletion.");

                // Display wrong input object
                wrongInputDisplay.DisplayWrongInputObject();

                // Deactivate all digit objects
            

                enteredCode = "";
            }

            StartCoroutine(DelayedReleaseButton(pressedButton));
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

            // Deactivate the keypad after successful entry (you may adjust this based on
// Optionally, play a sound or perform other actions related to the door opening

            // You can add sound or other actions here

            // Reset the entered code for subsequent attempts
            enteredCode = "";

            // Deactivate the keypad after successful entry (you may adjust this based on your game logic)
            ToggleKeypadActivation();

            Debug.Log("Door opening: animation triggered.");
        }
        else
        {
            Debug.LogError("Animator component not found on the door GameObject.");
        }
    }

    private IEnumerator PressButton(GameObject buttonObject)
    {
        // Store the original position of the button
        Vector3 originalPosition = buttonObject.transform.position;

        // Calculate the target position based on buttonPressDistance
        Vector3 targetPosition = allowZAxis
            ? originalPosition - new Vector3(0f, 0f, buttonPressDistance)
            : originalPosition;

        float elapsedTime = 0f;

        while (elapsedTime < interpolationTime)
        {
            buttonObject.transform.position = Vector3.Lerp(originalPosition, targetPosition, elapsedTime / interpolationTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        buttonObject.transform.position = targetPosition; // Ensure the final position is set

        // Reverse animation after a short delay
        StartCoroutine(ReverseButtonAnimation(buttonObject, originalPosition));
    }

    private IEnumerator DelayedReleaseButton(GameObject buttonObject)
    {
        yield return new WaitForSeconds(0.5f); // Adjust the delay time as needed
        ReleaseButton(buttonObject);
    }

    private IEnumerator InterpolatePosition(GameObject buttonObject, Vector3 targetPosition)
    {
        float elapsedTime = 0f;

        while (elapsedTime < interpolationTime)
        {
            buttonObject.transform.position = Vector3.Lerp(buttonObject.transform.position, targetPosition, elapsedTime / interpolationTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        buttonObject.transform.position = targetPosition; // Ensure the final position is set
    }

    private IEnumerator DelayedDoorAnimation()
    {
        yield return new WaitForSeconds(doorAnimationDelay); // Adjust the delay time as needed

        // Trigger the door animation
        TriggerDoorAnimation();
    }

    private void ReleaseButton(GameObject buttonObject)
    {
        // Implement any necessary actions to release the button here
        Debug.Log("Button released: " + buttonObject.name);
        isButtonPressed = false;
    }

    private IEnumerator ReverseButtonAnimation(GameObject buttonObject, Vector3 originalPosition)
    {
        yield return new WaitForSeconds(0.5f); // Adjust the delay time as needed

        float elapsedTime = 0f;
        Vector3 currentPosition = buttonObject.transform.position;

        while (elapsedTime < interpolationTime)
        {
            buttonObject.transform.position = Vector3.Lerp(currentPosition, originalPosition, elapsedTime / interpolationTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        buttonObject.transform.position = originalPosition; // Ensure the final position is set

        // Reset the flag to allow another transformation on the next button click
        isButtonPressed = false;
    }

  
}
