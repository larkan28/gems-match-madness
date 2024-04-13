using System.Collections.Generic;
using UnityEngine;

public class UI_ManagerGoals : MonoBehaviour
{
    public UI_Goal goalUIPrefab;
    public Transform goalParent;

    GameEvent m_gameEvent;
    List<UI_Goal> m_listGoals;

    void Awake ()
    {
        m_gameEvent = GameSystem.Instance.gameEvent;
        m_listGoals = new List<UI_Goal>();
    }

    void OnEnable ()
    {
        m_gameEvent.OnUpdateGoals += OnUpdateGoals;
        m_gameEvent.OnCreateGoals += OnCreateGoals;
        m_gameEvent.OnItemCollect += OnItemCollect;
        m_gameEvent.OnGameRestart += OnGameRestart;
    }

    void OnDisable ()
    {
        m_gameEvent.OnUpdateGoals -= OnUpdateGoals;
        m_gameEvent.OnCreateGoals -= OnCreateGoals;
        m_gameEvent.OnItemCollect -= OnItemCollect;
        m_gameEvent.OnGameRestart -= OnGameRestart;
    }

    void OnUpdateGoals (Goal _goal)
    {
        foreach (var goal in m_listGoals)
        {
            if (goal.IsParentOf(_goal))
            {
                goal.SetAmount(_goal.GetRemaining());
                break;
            }
        }
    }

    void OnCreateGoals (Goal _goal)
    {
        UI_Goal goalUI = Instantiate(goalUIPrefab, goalParent);

        if (goalUI != null)
        {
            goalUI.Init(_goal);
            
            m_listGoals.Add(goalUI);
            m_gameEvent.UpdateGoals(_goal);
        }
    }

    void OnItemCollect (Item _itemCollected)
    {
        foreach (var goalUI in m_listGoals)
        {
            if (goalUI.IsContaining(_itemCollected))
            {
                Vector2 point = goalUI.GetPoistionUI();

                _itemCollected.MoveTo(point);
                return;
            }
        }

        _itemCollected.PlayDestroy();
    }

    void OnGameRestart ()
    {
        foreach (var goalUI in m_listGoals)
            Destroy(goalUI.gameObject);

        m_listGoals.Clear();
    }
}