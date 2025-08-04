using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    public Animator chestAnimator; 
    private const string openAnimationName = "ChestOpening";  
    private const string closeAnimationName = "ChestClosing"; 
    private const string opened = "ChestOpened";
    private const string closed = "ChestClosed";
    public float detectionRadius = 3f; 
    public Transform player; 

    private bool playerIsNear = false; 
    private bool chestIsOpen = false;  
    private bool chestIsClosed = true; 


    public GameObject Inventory;
    public GameObject InventoryExtendedSlots;
    public GameObject craftingUI;

    public GameObject actions;
    public GameObject features;
    
    private void Awake()
    {
        chestAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        // Check the distance between player and chest
        float distance = Vector3.Distance(transform.position, player.position);

        // If the player is within the detection radius and the chest is not open
        if (distance <= detectionRadius && !playerIsNear && chestIsClosed)
        {
            OpenChest();
            features.gameObject.SetActive(false);

        }
        // If the player is outside the detection radius and the chest is open
        else if (distance > detectionRadius && playerIsNear && chestIsOpen)
        {
            CloseChest();
            Inventory.gameObject.SetActive(false);
            InventoryExtendedSlots.gameObject.SetActive(false);
            actions.SetActive(true);
            features.SetActive(true);
        }
    }

    void OpenChest()
    {
        chestAnimator.Play(openAnimationName); 
        chestIsClosed = false;  
        playerIsNear = true;    
        
        
        StartCoroutine(SetChestOpenedAfterAnimation(openAnimationName));
    }

    void CloseChest()
    {
        chestAnimator.Play(closeAnimationName); 
        chestIsOpen = false; 
        playerIsNear = false; 
        
        StartCoroutine(SetChestClosedAfterAnimation(closeAnimationName));
    }

    // Coroutine to wait for the open animation to complete
    IEnumerator SetChestOpenedAfterAnimation(string animationName)
    {
        yield return new WaitForSeconds(chestAnimator.GetCurrentAnimatorStateInfo(0).length); // Wait for the animation duration
        chestIsOpen = true; // Mark the chest as fully opened
        chestAnimator.SetBool(opened, true); // Set the "opened" state
        Inventory.gameObject.SetActive(true);
        InventoryExtendedSlots.gameObject.SetActive(true);
        craftingUI.gameObject.SetActive(false);
    }

    // Coroutine to wait for the close animation to complete
    IEnumerator SetChestClosedAfterAnimation(string animationName)
    {
        yield return new WaitForSeconds(chestAnimator.GetCurrentAnimatorStateInfo(0).length); // Wait for the animation duration
        chestIsClosed = true; // Mark the chest as fully closed
        chestAnimator.SetBool(closed, true); // Set the "closed" state
        craftingUI.SetActive(true);
    }

    void OnDrawGizmosSelected()
    {
        // Draw a sphere in the editor to show detection radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
