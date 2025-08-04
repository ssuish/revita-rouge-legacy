using System.Collections;
using UnityEngine;
using TMPro; // Import TextMeshPro namespace

public class NPCSalvage : MonoBehaviour
{
    private PlayerActionMap _inputActions; // Reference to input actions
    public GameObject SalvageUI; // Panel for dialogue
    public GameObject Inventory;
    public GameObject Close;
    public GameObject Crafting;
    public GameObject inventoryBut;

   
    private void Awake()
    {
        _inputActions = new PlayerActionMap(); // Instantiate your input actions
    }

    private void OnEnable()
    {
        _inputActions.Enable(); // Enable input actions
        //_inputActions.UI.Click.performed += OnInteract; // Subscribe to the interact action
    }

    private void OnDisable()
    {
        _inputActions.Disable(); // Disable input actions
        //_inputActions.UI.Click.performed -= OnInteract; // Unsubscribe from the interact action
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _inputActions.Player.Move.Disable();
            SalvageUI.SetActive(true);
            Inventory.SetActive(true);
            Close.SetActive(true);
            Crafting.SetActive(false);
            inventoryBut.SetActive(false);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _inputActions.Player.Move.Enable();
            SalvageUI.SetActive(false);
            Inventory.SetActive(false);
            Close.SetActive(true);
            Crafting.SetActive(true);
            inventoryBut.SetActive(true);
        }
    }
}
