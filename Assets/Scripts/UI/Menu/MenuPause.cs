using UnityEngine;
using UnityEngine.UI;

public class MenuPause : MonoBehaviour
{
    public GameObject root;

    public Button buttonExit;
    public Button buttonLevels;
    public Button buttonRestart;
    public Button buttonContinue;

    public Button buttonMute;
    public Button buttonMuteDisabled;

    public MenuLevel menuLevel;

    GameSound m_gameSound;
    GameSystem m_gameSystem;

    void Awake ()
    {
        m_gameSound = GameSound.Instance;
        m_gameSystem = GameSystem.Instance;

        buttonExit.onClick.AddListener(ButtonExit);
        buttonLevels.onClick.AddListener(ButtonLevels);
        buttonRestart.onClick.AddListener(ButtonRestart);
        buttonContinue.onClick.AddListener(ButtonContinue);

        buttonMute.onClick.AddListener(ButtonMute);
        buttonMuteDisabled.onClick.AddListener(ButtonMute);
    }

    void Start ()
    {
        SwitchMuteButtons();
    }

    void OnEnable ()
    {
        m_gameSystem.gameEvent.OnStateUpdate += OnStateUpdate;
    }

    void OnDisable ()
    {
        m_gameSystem.gameEvent.OnStateUpdate -= OnStateUpdate;
    }

    void ButtonExit ()
    {
        Application.Quit();
    }

    void SwitchMuteButtons ()
    {
        bool isMuted = m_gameSound.IsMuted();

        buttonMute.gameObject.SetActive(!isMuted);
        buttonMuteDisabled.gameObject.SetActive(isMuted);
    }

    void ButtonMute ()
    {
        m_gameSound.SwitchMute();
        SwitchMuteButtons();
    }

    void ButtonLevels ()
    {
        menuLevel.ShowMenu(true);
        ShowMenu(false);
    }

    void ButtonContinue ()
    {
        m_gameSystem.SetState(GameSystem.State.Playing);
    }

    void ButtonRestart ()
    {
        m_gameSystem.Restart();
    }
    
    void OnStateUpdate (GameSystem.State _currState)
    {
        ShowMenu(_currState == GameSystem.State.Paused);
    }

    public void ShowMenu (bool _value)
    {
        root.SetActive(_value);
    }
}
