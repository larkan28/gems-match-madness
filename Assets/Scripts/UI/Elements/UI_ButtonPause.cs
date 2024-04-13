using UnityEngine;
using UnityEngine.UI;

public class UI_ButtonPause : MonoBehaviour
{
    Button m_buttonPause;
    GameSound m_gameSound;
    GameSystem m_gameSystem;

    void Awake ()
    {
        m_gameSound = GameSound.Instance;
        m_gameSystem = GameSystem.Instance;

        m_buttonPause = GetComponent<Button>();
        m_buttonPause.onClick.AddListener(ButtonPause);
    }

    void OnEnable ()
    {
        m_gameSystem.gameEvent.OnStateUpdate += OnStateUpdate;
    }

    void OnDisable ()
    {
        m_gameSystem.gameEvent.OnStateUpdate -= OnStateUpdate;
    }

    void ButtonPause ()
    {
        m_gameSystem.SwitchPause();
    }

    void OnStateUpdate (GameSystem.State _currState)
    {
        m_buttonPause.interactable = (_currState == GameSystem.State.Playing);

        if (m_gameSystem.IsPaused())
            m_gameSound.Music.Pause();
        else
            m_gameSound.Music.UnPause();
    }
}
