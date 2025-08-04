using System;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem; // New Input System

public class Dialogue : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public string[] lines;
    public float textSpeed;
    private bool inputProcessed = false;
    private int index;
    private PlayerActionMap inputActions; // Input actions for the new Input System

    private void Awake()
    {
        // Initialize the input actions
        inputActions = new PlayerActionMap();
    }

    private void OnEnable()
    {
        // Enable input actions and register the callback for click/tap input
        inputActions.UI.Click.performed += OnInputClick;
        inputActions.UI.Enable();
    }

    private void OnDisable()
    {
        // Unregister the callback and disable input actions
        inputActions.UI.Click.performed -= OnInputClick;
        inputActions.UI.Disable();
    }

    private void Start()
    {
        textComponent.text = string.Empty;
        StartDialogue();
    }

    // This function is called when either a mouse click or touch tap is detected
    private void OnInputClick(InputAction.CallbackContext context)
    {
        if (inputProcessed) return; // Prevent multiple input detections
        inputProcessed = true;
        
        if (textComponent.text == lines[index])
        {
            // If the current line is fully displayed, go to the next line
            NextLine();
        }
        else
        {
            // If the current line is not fully displayed, show it immediately
            StopAllCoroutines();
            textComponent.text = lines[index];
        }
        
        StartCoroutine(ResetInputProcessed());
    }

    // Starts the dialogue typing coroutine
    void StartDialogue()
    {
        index = 0;
        StartCoroutine(TypeLine());
    }

    // Coroutine to type each character one by one
    IEnumerator TypeLine()
    {
        foreach (char c in lines[index].ToCharArray())
        {
            textComponent.text += c;

            // Adjust the wait time based on the character
            float waitTime = textSpeed;
            if (c == '.' || c == ',' || c == '!' || c == '?')
            {
                waitTime = textSpeed * 2; // Slow down for punctuation
            }

            yield return new WaitForSeconds(waitTime); // Wait for the specified time between characters
        }
    }

    // Proceed to the next line of dialogue or end the dialogue
    void NextLine()
    {
        if (index < lines.Length - 1)
        {
            index++;
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine()); // Start typing the next line
        }
        else
        {
            gameObject.SetActive(false); // Disable the game object when dialogue ends
        }
    }
    
    private IEnumerator ResetInputProcessed()
    {
        yield return new WaitForSeconds(2f);
        inputProcessed = false;
    }
}
