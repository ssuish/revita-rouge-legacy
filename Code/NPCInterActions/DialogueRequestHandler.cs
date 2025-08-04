using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueRequestHandler : MonoBehaviour
{
    private void OnEnable()
    {
        GameEventsManagerSO.instance.miscEvents.OnDialogueRequested += GetDialogueRequest;
    }
    
    private void OnDisable()
    {
        GameEventsManagerSO.instance.miscEvents.OnDialogueRequested -= GetDialogueRequest;
    }
    
    private void HandleDialogueRequest(string dialogueName)
    {
        // Get the dialogue game object and set it active to start the dialogue
        
        Transform dialogueTransform = transform.Find(dialogueName);

        if (dialogueTransform != null)
        {
            // Set the child GameObject active
            dialogueTransform.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Dialogue child GameObject not found.");
        }
    }
    
    private void GetDialogueRequest(bool isDialogueRequested, string dialogueName)
    {
        if (isDialogueRequested)
        {
            HandleDialogueRequest(dialogueName);
        }
    }
}
