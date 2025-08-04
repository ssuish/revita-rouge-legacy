using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class SwitchScript : MonoBehaviour
{
    [SerializeField] private GateScript[] gates;
    [SerializeField] private Sprite unpressedSprite;
    [SerializeField] private Sprite pressedSprite;
    [SerializeField] private bool toggleOnce = false;
    private SpriteRenderer _spriteRenderer;
    private bool _isPressed = false;
    
    [Header("For Waypoint Management")]
    [SerializeField] private bool enableWaypointManagement;
    [SerializeField] private int index = 0;
    [SerializeField] private bool isBefore = true;
    // Disable line loop
    [SerializeField] private bool toggleLineLoop = false;
    [SerializeField] private bool toggleLooping = false;

    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.sprite = unpressedSprite;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !_isPressed)
        {
            ToggleGates();
            _spriteRenderer.sprite = pressedSprite;
            _isPressed = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && _isPressed && !toggleOnce)
        {
            _spriteRenderer.sprite = unpressedSprite;
            _isPressed = false;
        }
    }

    private void ToggleGates()
    {
        foreach (var gate in gates)
        {
            gate.ToggleGate();
        }
        
        if (enableWaypointManagement)
        {
            IsGateClosed();
        }
    }
    
    private void IsGateClosed()
    {
        foreach (var gate in gates)
        {
            if (gate.IsClosed())
            {
                // Game Events to Waypoint Management
                GameEventsManagerSO.instance.miscEvents.WaypointGateClosed(index, isBefore, toggleLineLoop, toggleLooping);
                return;
            }
        }
    }
}