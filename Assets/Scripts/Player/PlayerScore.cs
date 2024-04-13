using UnityEngine;

public class PlayerScore : MonoBehaviour
{
    public int scoreMultiplier;
    public int scoreComboMultiplier;

    int m_maxScore;
    int m_currScore;

    GameEvent m_gameEvent;

    public int Score
    {
        get => m_currScore;
        set
        {
            m_currScore = value;
            m_gameEvent.UpdateScore(m_currScore);
        }
    }

    public void Init (Player _player)
    {
        m_gameEvent = _player.Events;
    }

    public void Restart ()
    {
        Score = 0;
    }

    public void AddScore (int _combo)
    {
        int multiplier = scoreComboMultiplier * (_combo - 3);
        int amount = scoreMultiplier;

        if (multiplier > 0)
            amount *= multiplier;

        Score += _combo * amount;
    }
}
