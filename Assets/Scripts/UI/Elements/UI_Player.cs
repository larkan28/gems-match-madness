using UnityEngine;
using TMPro;
using System.Text;

public class UI_Player : MonoBehaviour
{
    public GameObject root;
    public GameObject rootTimer;

    public TextMeshProUGUI textMoves;
    public TextMeshProUGUI textTimer;
    public TextMeshProUGUI textCombo;
    public TextMeshProUGUI textScore;

    Animator m_animatorMoves;
    Animator m_animatorTimer;
    Animator m_animatorScore;

    GameEvent m_gameEvent;
    GameSystem m_gameSystem;
    StringBuilder m_strBuilder;

    void Awake ()
    {
        m_gameSystem = GameSystem.Instance;
        m_strBuilder = new StringBuilder();

        m_animatorMoves = GetComponentInChildren<Animator>();
        m_animatorTimer = GetComponentInChildren<Animator>();
        m_animatorScore = GetComponentInChildren<Animator>();
    }

    void OnEnable ()
    {
        if (m_gameEvent == null)
            m_gameEvent = m_gameSystem.gameEvent;

        m_gameEvent.OnUpdateCombo += OnUpdateCombo;
        m_gameEvent.OnUpdateMoves += OnUpdateMoves;
        m_gameEvent.OnUpdateTimer += OnUpdateTimer;
        m_gameEvent.OnUpdateScore += OnUpdateScore;
        m_gameEvent.OnStateUpdate += OnStateUpdate;
        m_gameEvent.OnGameRestart += OnGameRestart;
    }

    void OnDisable ()
    {
        m_gameEvent.OnUpdateCombo -= OnUpdateCombo;
        m_gameEvent.OnUpdateMoves -= OnUpdateMoves;
        m_gameEvent.OnUpdateTimer -= OnUpdateTimer;
        m_gameEvent.OnUpdateScore -= OnUpdateScore;
        m_gameEvent.OnStateUpdate -= OnStateUpdate;
        m_gameEvent.OnGameRestart -= OnGameRestart;
    }

    void OnUpdateTimer (int _seconds)
    {
        if (_seconds < 0)
        {
            rootTimer.SetActive(false);
            return;
        }

        if (!rootTimer.activeSelf)
            rootTimer.SetActive(true);

        int mins = _seconds / 60;
        int secs = _seconds % 60;

        textTimer.text = string.Format("{0:00}:{1:00}", mins, secs);
    }

    void OnUpdateMoves (int _moves)
    {
        m_strBuilder.Clear();
        m_strBuilder.Append("Moves: ");
        m_strBuilder.Append(_moves);

        textMoves.text = m_strBuilder.ToString();
    }

    void OnUpdateCombo (int _combo)
    {

    }
    
    void OnUpdateScore (int _score)
    {
        m_strBuilder.Clear();
        m_strBuilder.Append("Score: ");
        m_strBuilder.Append(_score);

        textScore.text = m_strBuilder.ToString();
        m_animatorScore.SetTrigger("Add");
    }

    void OnStateUpdate (GameSystem.State _currState)
    {
        //root.SetActive(!m_gameSystem.IsGameOver());
    }

    void OnGameRestart ()
    {
        //root.SetActive(true);
    }
}
