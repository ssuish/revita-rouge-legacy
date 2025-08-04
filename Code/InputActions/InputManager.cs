using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;
using Vector2 = UnityEngine.Vector2;

[RequireComponent(typeof(PlayerInput))]
public class InputManager : MonoBehaviour
{
    /*public static Vector2 Movement;

    private PlayerInput _playerInput;
    private InputAction _moveAction;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _moveAction = _playerInput.actions["Move"];
    }

    private void Update()
    {
        Movement = _moveAction.ReadValue<Vector2>();
    }*/
    
    private PlayerActionMap _playerActionMap;
    private Vector2 moveInput;

    public Vector2 MoveInput => moveInput;
    
    private void Awake()
    {
        _playerActionMap = new PlayerActionMap();
    }

    private void OnEnable()
    {
        _playerActionMap.Enable();
    }

    private void OnDisable()
    {
        _playerActionMap.Disable();
    }

    private void Update()
    {
        moveInput = _playerActionMap.Player.Move.ReadValue<Vector2>();
    }
    
    public void MovePressed(InputAction.CallbackContext context)
    {
        if (context.performed || context.canceled)
        {
            GameEventsManagerSO.instance.inputEvents.MovePressed(context.ReadValue<Vector2>());
        }
    }

    public void SubmitPressed(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            GameEventsManagerSO.instance.inputEvents.SubmitPressed();
        }
    }

    public void QuestLogTogglePressed(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            GameEventsManagerSO.instance.inputEvents.QuestLogTogglePressed();
        }
    }
}
