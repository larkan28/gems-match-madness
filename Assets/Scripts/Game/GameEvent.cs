using System;
using UnityEngine;

[CreateAssetMenu(fileName = "GameEvent", menuName = "Gems Match Madness/GameEvent", order = 0)]
public class GameEvent : ScriptableObject
{
    public event Action<int> OnUpdateTimer;
    public void UpdateTimer (int _seconds) { OnUpdateTimer?.Invoke(_seconds); }

    public event Action<int> OnUpdateCombo;
    public void UpdateCombo (int _combo) { OnUpdateCombo?.Invoke(_combo); }

    public event Action<int> OnUpdateScore;
    public void UpdateScore (int _score) { OnUpdateScore?.Invoke(_score); }

    public event Action<Goal> OnUpdateGoals;
    public void UpdateGoals (Goal _goal) { OnUpdateGoals?.Invoke(_goal); }

    public event Action<Goal> OnCreateGoals;
    public void CreateGoals (Goal _goal) { OnCreateGoals?.Invoke(_goal); }

    public event Action<int> OnUpdateMoves;
    public void UpdateMoves (int _moves) { OnUpdateMoves?.Invoke(_moves); }

    public event Action<Item> OnItemCollect;
    public void ItemCollect (Item _item) { OnItemCollect?.Invoke(_item); }

    public event Action<int, ItemData, Vector2> OnShowConnections;
    public void ShowConnections (int _amount, ItemData _data, Vector2 _center) { OnShowConnections?.Invoke(_amount, _data, _center); }

    public event Action<GameSystem.State> OnStateUpdate;
    public void UpdateState (GameSystem.State _currState) { OnStateUpdate?.Invoke(_currState); }

    public event Action OnGameRestart;
    public void GameRestart () { OnGameRestart?.Invoke(); }
}
