using System;
using UnityEngine;

namespace Code.CanvasButtons
{
    public class InGameMenu : MonoBehaviour
    {
        public GameObject TabsCanvas;
        public GameObject closingtabs;
        public GameObject AttackBut;
        public GameObject TorchBut;
        public GameObject Controls;
        public GameObject PauseBut;
        public GameObject sprinting;
        public GameObject stats;
        public GameObject InventoryBut;
        //public GameObject Features;

        public GameObject questLog;
        public GameObject questProg;
        

        // Method called when the Tabs button is clicked
        public void TabsButtonClicked()
        {
            InventoryBut.SetActive(false);
            // Hindi papa kita yung mga buttons
            TabsCanvas.SetActive(!TabsCanvas.activeSelf);
            closingtabs.SetActive(false);
            AttackBut.SetActive(false);
            TorchBut.SetActive(false);
            Controls.SetActive(false);
            PauseBut.SetActive(false);
            sprinting.SetActive(false);
            stats.SetActive(false);
            questLog.SetActive(false);
            questProg.SetActive(false);
        }
        public void TabsButtonClickedForFeatures()
        {
            // Hindi papa kita yung mga buttons
            closingtabs.SetActive(false);
            AttackBut.SetActive(false);
            TorchBut.SetActive(false);
            Controls.SetActive(false);
            PauseBut.SetActive(false);
            sprinting.SetActive(false);
            stats.SetActive(false);
        }
    
    
    
    }
}
