using UnityEngine;
using System;

public class InputEvents
{
    public event Action<Vector2> onMovePressed;
    public void MovePressed(Vector2 moveDir)
    {
        if (onMovePressed != null)
        {
            onMovePressed(moveDir);
        }
    }

    public event Action onSubmitPressed;
    public void SubmitPressed()
    {
        if (onSubmitPressed != null)
        {
            onSubmitPressed();
        }
    }

    public event Action onQuestLogTogglePressed;
    public void QuestLogTogglePressed()
    {
        if (onQuestLogTogglePressed != null)
        {
            onQuestLogTogglePressed();
        }
    }
    

    public event Action onQuestProgressionTogglePressed;
    public void QuestProgressionTogglePressed()
    {
        if (onQuestProgressionTogglePressed != null)
        {
            onQuestProgressionTogglePressed();
        }
    }
    
    public event Action onEndingTogglePressed;
    public void EndingTogglePressed()
    {
        if (onEndingTogglePressed != null)
        {
            onEndingTogglePressed();
        }
    }
}