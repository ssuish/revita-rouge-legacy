using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FootStepScript : MonoBehaviour
{
    public GameObject footstep;
    private PlayerActionMap _inputActions;
    private Vector2 _movement;

    void Awake()
    {
        _inputActions = new PlayerActionMap();
        _inputActions.Player.Enable();
        _inputActions.Player.Move.performed += ctx => _movement = ctx.ReadValue<Vector2>();
        _inputActions.Player.Move.canceled += ctx => _movement = Vector2.zero;
        
    }
    
    // Start is called before the first frame update
    void Start()
    {
        footstep.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
        
        if (_movement != Vector2.zero)
        {
            footsteps();
        }
        else
        {
            StopFootsteps();
        }
    }
    void footsteps()
    {
        footstep.SetActive(true);
    }

    void StopFootsteps()
    {
        footstep.SetActive(false);
    }
    
    void OnDisable()
    {
        _inputActions.Player.Disable();
    }
}
