using System;

public class EnemiesEvent
{
    public event Action onEnemyDefeated;
    public void EnemyDefeated()
    {
        if (onEnemyDefeated != null)
        {
            onEnemyDefeated();
        }
    }
}
