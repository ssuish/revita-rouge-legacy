using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowFeaturesCode : MonoBehaviour
{
    public GameObject FeatureCanvas;
    public GameObject CloseTabFeat;
    public GameObject craftBut;
    public GameObject SalvageBut;
    public GameObject DailyMissionBut;
    public GameObject QuestsBut;
    
    public void ShowAllTabsForCraft()
    {
        // Hindi papa kita yung mga buttons
        FeatureCanvas.SetActive(!FeatureCanvas.activeSelf);
        CloseTabFeat.SetActive(true);
        craftBut.SetActive(true);
        SalvageBut.SetActive(true);
        DailyMissionBut.SetActive(true);
        QuestsBut.SetActive(true);
    }
}
