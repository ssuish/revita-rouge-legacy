using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class ButtonCraftSalvageRecipes : MonoBehaviour
{
    public Button CraftingReciBut;
    public Button SalvageBut;
    public Button CloseButton;

    public GameObject CraftingRecipes;
    public GameObject SalvageableItems;

    private void Start()
    {
        CraftingReciBut.onClick.AddListener(ShowCraftingRecipes);
        SalvageBut.onClick.AddListener(ShowSalvageableItems);
        CloseButton.onClick.AddListener(CloseUI);

        
        CraftingRecipes.SetActive(false);
        SalvageableItems.SetActive(false);
        CloseButton.gameObject.SetActive(false);
    }

    // Show crafting recipes and the close button
    public void ShowCraftingRecipes()
    {
        CraftingRecipes.SetActive(true);
        SalvageableItems.SetActive(false);  // Hide other menu
        CloseButton.gameObject.SetActive(true);  // Show close button
        CraftingReciBut.gameObject.SetActive(false);
        SalvageBut.gameObject.SetActive(false);
    }

    // Show salvageable items and the close button
    public void ShowSalvageableItems()
    {
        SalvageableItems.SetActive(true);
        CraftingRecipes.SetActive(false);  // Hide other menu
        CloseButton.gameObject.SetActive(true);  // Show close button
        CraftingReciBut.gameObject.SetActive(false);
        SalvageBut.gameObject.SetActive(false);
    }

    // Close both UI elements
    public void CloseUI()
    {
        CraftingRecipes.SetActive(false);
        SalvageableItems.SetActive(false);
        CloseButton.gameObject.SetActive(false);  // Hide close button
        CraftingReciBut.gameObject.SetActive(true);
        SalvageBut.gameObject.SetActive(true);
    }
}