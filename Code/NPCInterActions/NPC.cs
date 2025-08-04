using System.Collections;
using UnityEngine;
using TMPro; // Import TextMeshPro namespace

public class NPC : MonoBehaviour
{
    private PlayerActionMap _inputActions; // Reference to input actions
    
    [Header("Dialogue Settings")]
    [SerializeField] private GameObject dialoguePanel; // Panel for dialogue
    [SerializeField] private TextMeshProUGUI dialogueText; // Text component for displaying dialogue (using TextMeshPro)
    [SerializeField] private string[] dialogue; // Array of dialogue strings
    [SerializeField] private bool toggleDialogueOnce = false; // Flag to check if dialogue has been toggled once
    [SerializeField] private GameObject buttonNext; // Button for next dialogue
    [SerializeField] private int index; // Current dialogue index
    
    [SerializeField] private GameObject ActionsButs;
    [SerializeField] private GameObject FeaturesButs;

    [SerializeField] private float wordSpeed; // Speed of word display
    [SerializeField] private bool playerIsClose; // Flag to check if player is close
    
    private bool isTyping; // Flag to check if dialogue is currently typing
    private bool isToggled = false; // Flag to check if dialogue has been toggled

    // Two colors for simulating a conversation
    [Header("Dialogue Font Colors")]
    [SerializeField] private Color playerColor = Color.green; // Color for the player's text
    [SerializeField] private Color npcColor = Color.blue; // Color for the NPC's text

    private void Awake()
    {
        _inputActions = new PlayerActionMap(); // Instantiate your input actions
    }

    private void OnEnable()
    {
        _inputActions.Enable(); // Enable input actions
        _inputActions.UI.Click.performed += OnInteract; // Subscribe to the interact action
    }

    private void OnDisable()
    {
        _inputActions.Disable(); // Disable input actions
        _inputActions.UI.Click.performed -= OnInteract; // Unsubscribe from the interact action
    }

    private void OnInteract(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (playerIsClose && !isTyping) // Check if player is close and dialogue is not currently typing
        {
            NextLine(); // Move to the next line of dialogue when the button is pressed
        }
    }

    private IEnumerator Typing()
    {
        isTyping = true; // Set the typing flag to true
        dialogueText.text = ""; // Clear the previous text
        buttonNext.SetActive(false); // Ensure the button is hidden at the start
    
        bool insideTag = false; // Flag to track if we are inside a rich text tag
        string currentText = ""; // Variable to store the text as it's being typed

        foreach (char letter in dialogue[index])
        {
            if (letter == '<') // Start of a rich text tag
            {
                insideTag = true;
            }

            currentText += letter; // Add the letter to the current text
        
            if (letter == '>') // End of a rich text tag
            {
                insideTag = false;
            }

            // Only display the text one character at a time when not inside a rich text tag
            if (!insideTag)
            {
                dialogueText.text = currentText; // Update the displayed text
                yield return new WaitForSeconds(wordSpeed); // Wait for the specified speed
            }
        }

        buttonNext.SetActive(true); // Show the next button when the text is fully displayed
        isTyping = false; // Set the typing flag to false when done
    }



    private string ApplyTextColor(string text, Color color)
    {
        // Convert the Color object to a hex string recognized by TextMeshPro
        string hexColor = ColorUtility.ToHtmlStringRGB(color);
        // Return the text wrapped in a rich text color tag
        return $"<color=#{hexColor}>{text}</color>";
    }

    public void NextLine()
    {
        buttonNext.SetActive(false); // Hide the next button
        if (index < dialogue.Length - 1)
        {
            index++; // Move to the next dialogue index
            StartCoroutine(Typing()); // Start typing the next line
        }
        else
        {
            GameEventsManagerSO.instance.miscEvents.DialogueEnded();
            ZeroText(); // Reset dialogue when finished
        }
    }

    public void ZeroText()
    {
        _inputActions.Player.Move.Disable();
        dialogueText.text = ""; // Clear the text
        index = 0; // Reset the index
        isTyping = false; // Ensure typing flag is reset
        StopAllCoroutines(); // Stop any active typing coroutine
        dialoguePanel.SetActive(false); // Hide the dialogue panel
        ActionsButs.SetActive(true);
        FeaturesButs.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isToggled) // Check if the player has entered the trigger and dialogue has not been toggled
        {
            _inputActions.Player.Move.Disable();
            playerIsClose = true; // Player is close
            dialoguePanel.SetActive(true); // Automatically show the dialogue panel
            index = 0; // Reset index to show the first line of dialogue
            dialogueText.text = ""; // Clear the dialogue text immediately
            StopAllCoroutines(); // Stop any running Typing coroutine to prevent overlap
            StartCoroutine(Typing()); // Start typing out the first line of dialogue

            ActionsButs.SetActive(false);
            FeaturesButs.SetActive(false);

            if (toggleDialogueOnce)
            {
                isToggled = true; // Set the toggle flag to true
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _inputActions.Player.Move.Enable();
            playerIsClose = false; // Player has exited the trigger
            ZeroText(); // Clear dialogue when the player leaves
            ActionsButs.SetActive(true);
            FeaturesButs.SetActive(true);
        }
    }
}
