using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseOutsideArea : MonoBehaviour
{
    public GameObject InventoryUI;
    public GameObject ContentPofQuestProgUI;
    public GameObject ContentPofQuestLogUI;

    public GameObject ActionsUI;


    public void CloseOutsideofArea()
    {
        InventoryUI.SetActive(false);
        ContentPofQuestLogUI.SetActive(false);
        ContentPofQuestProgUI.SetActive(false);
        ActionsUI.SetActive(true);
    }
}