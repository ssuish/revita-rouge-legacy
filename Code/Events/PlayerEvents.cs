using System;

public class PlayerEvents
{
    public event Action onDisablePlayerMovement;

    public void DisablePlayerMovement()
    {
        if (onDisablePlayerMovement != null)
        {
            onDisablePlayerMovement();
        }
    }

    public event Action onEnablePlayerMovement;

    public void EnablePlayerMovement()
    {
        if (onEnablePlayerMovement != null)
        {
            onEnablePlayerMovement();
        }
    }

    public event Action<int> onExperienceGained; // Add parameter to get the quest info rewards. 
    public void ExperienceGained(int experience)
    {
        if (onExperienceGained != null)
        {
            onExperienceGained(experience);
        }
    }

    public event Action<int> onPlayerLevelChange;
    public void PlayerLevelChange(int level)
    {
        if (onPlayerLevelChange != null)
        {
            onPlayerLevelChange(level);
        }
    }

    public event Action<float> onPlayerExperienceChange;

    public void PlayerExperienceChange(float experience)
    {
        if (onPlayerExperienceChange != null)
        {
            onPlayerExperienceChange(experience);
        }
    }
    
    public event Action<int> onPlayerDeathCount;
    public void PlayerDeath(int deathCount)
    {
        if (onPlayerDeathCount != null)
        {
            onPlayerDeathCount(deathCount);
        }
    }

    public event Action<string> onPlayerEatenFood;
    public void PlayerEatenFood(string FoodName)
    {
        if (onPlayerEatenFood != null)
        {
            onPlayerEatenFood(FoodName);
        }
    }
}