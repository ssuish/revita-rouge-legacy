using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateScript : MonoBehaviour
{
    private bool _isOpen = false;
    private GameObject _openGate;
    private GameObject _closeGate;
    public bool initialStateIsOpen = false;

    void Start()
    {
        // Assuming the first child is the open gate and the second child is the close gate
        _openGate = transform.GetChild(0).gameObject;
        _closeGate = transform.GetChild(1).gameObject;
        
        // Set the initial state of the gate
        if (initialStateIsOpen)
        {
            ToggleGate();
        }

        // Initialize the gate state
        UpdateGateState();
    }

    public void ToggleGate()
    {
        _isOpen = !_isOpen;
        UpdateGateState();
    }

    private void UpdateGateState()
    {
        if (_isOpen)
        {
            OpenGate();
        }
        else
        {
            CloseGate();
        }
    }

    private void OpenGate()
    {
        _openGate.SetActive(true);
        _closeGate.SetActive(false);
    }

    private void CloseGate()
    {
        _openGate.SetActive(false);
        _closeGate.SetActive(true);
    }
    
    public bool IsClosed()
    {
        return !_isOpen;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Boss2"))
        {
            if (IsClosed())
            {
                // Stop the movement of the boss
                GameEventsManagerSO.instance.miscEvents.GateClosedStopBoss();
            }
        }
    }
}