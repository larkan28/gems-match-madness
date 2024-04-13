using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerController playerController;
    public PlayerScore playerScore;

    GameSystem m_gameSystem;

    public GameEvent Events
    {
        get
        {
            return m_gameSystem.gameEvent;
        }
    }

    void Awake ()
    {
        m_gameSystem = GameSystem.Instance;

        playerController.Init(this);
        playerScore.Init(this);
    }

    void OnEnable ()
    {
        Events.OnGameRestart += OnGameRestart;
    }

    void OnDisable ()
    {
        Events.OnGameRestart -= OnGameRestart;
    }

    void Update ()
    {
        if (m_gameSystem.IsPaused())
            return;

        playerController.Think();
    }

    void OnGameRestart ()
    {
        playerController.Restart();
        playerScore.Restart();
    }
}
