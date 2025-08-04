using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FeatureTabs : MonoBehaviour
{
    
    public GameObject FeaturesTab;
    
    
    public void HidingTabsFeatures()
    {
        FeaturesTab.SetActive(false);
    }

    public void ShowCraftTab()
    {
        HidingTabsFeatures();
        FeaturesTab.SetActive(true);
    }
    public void ShowSalvageTab()
    {
        HidingTabsFeatures();

    }
    public void ShowDailyMisTab()
    {
        HidingTabsFeatures();

    }
    public void ShowQuestTab()
    {
        HidingTabsFeatures();

    }
}
