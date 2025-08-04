using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutomataChecker : MonoBehaviour
{
    public AutomataChangingSprite spriteSwitcher;

    private void Update()
    {
        // Check if the sprite is upgrading
        if (spriteSwitcher.IsUpgrading())
        {
            Debug.Log("Upgrading to the next level...");
        }
        else
        {
            Debug.Log("Current level: " + spriteSwitcher.GetCurrentLevel());
        }
    }
}
