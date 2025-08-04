using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.OnScreen;


public class PlayerSubmitQuest : MonoBehaviour
{
    private PlayerActionMap _playerActionMap;

    // Update is called once per frame
    void Awake()
    {
        _playerActionMap = new PlayerActionMap();
        _playerActionMap.Quest.Enable();
    }

    private void OnEnable()
    {
        _playerActionMap.Enable();
        _playerActionMap.Quest.Submit.performed += OnSubmitQuest;
    }

    private void OnDisable()
    {
        _playerActionMap.Quest.Submit.performed -= OnSubmitQuest;
        _playerActionMap.Disable();
    }
    
    private void OnSubmitQuest(InputAction.CallbackContext context)
    {
        SubmitQuest();
    }
    
    private void SubmitQuest()
    {
        Debug.Log("Quest Submitted");
        GameEventsManagerSO.instance.inputEvents.SubmitPressed();
    }
}
