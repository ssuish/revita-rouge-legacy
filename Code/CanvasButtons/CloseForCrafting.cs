using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseForCrafting : MonoBehaviour
{
    public GameObject PauseBut;

    // Method called when the Tabs button is clicked
    public void HidingButtonsForCraftandothers()
    {
        // Hindi papa kita yung mga buttons
        PauseBut.SetActive(false);
    }
}
