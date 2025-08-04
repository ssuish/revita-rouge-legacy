using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Close : MonoBehaviour
{
    public GameObject closetabs;
    public GameObject InventoryButclose;
    //public GameObject CraftBut;
    public GameObject InterBut;
    public GameObject AtkBut;
    public GameObject TorchBut;
    public GameObject Controls;
    public GameObject PauseBut;
    public GameObject Stats;
    public GameObject sprinting;
    public GameObject extendedInventory;
    public GameObject questProg;
    public GameObject questLog;
    
    
    public void HideAllTabs()
    {
        extendedInventory.SetActive(false);
        InventoryButclose.SetActive(false);
        
        //pakita mga buttons after closing
        closetabs.SetActive(true);
        //CraftBut.SetActive(true); 
        InterBut.SetActive(true);
        AtkBut.SetActive(true);
        TorchBut.SetActive(true);
        Controls.SetActive(true);
        PauseBut.SetActive(true);
        sprinting.SetActive(true);
        Stats.SetActive(true);
        questLog.SetActive(true);
        questProg.SetActive(true);
        
    }
}
