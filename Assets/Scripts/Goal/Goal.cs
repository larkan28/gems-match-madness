using UnityEngine;

[System.Serializable]
public class Goal
{
    public int collectAmount;
    public ItemData itemToCollect;

    int m_currCollected;
    GameEvent m_gameEvent;

    public void Init ()
    {
        m_currCollected = 0;

        m_gameEvent = GameSystem.Instance.gameEvent;
        m_gameEvent.CreateGoals(this);
    }

    public void Update (ItemData _itemData)
    {
        if (IsCompleted() || !itemToCollect.EqualsTo(_itemData))
            return;

        m_currCollected++;
        m_gameEvent.UpdateGoals(this);
    }

    public bool IsCompleted ()
    {
        return m_currCollected >= collectAmount;
    }

    public int GetRemaining ()
    {
        return collectAmount - Mathf.Clamp(m_currCollected, 0, collectAmount);
    }
}
