using UnityEngine;

public class GameSound : Singleton<GameSound>
{
    GameSystem m_gameSystem;

    AudioSource m_audioSound;
    AudioSource m_audioMusic;
    AudioSource m_audioMenus;

    AudioSource Menus
    {
        get
        {
            if (m_audioMenus == null)
                m_audioMenus = gameObject.AddComponent<AudioSource>();

            return m_audioMenus;
        }
    }

    public AudioSource Sound
    {
        get
        {
            if (m_audioSound == null)
                m_audioSound = gameObject.AddComponent<AudioSource>();

            return m_audioSound;
        }
    }

    public AudioSource Music
    {
        get
        {
            if (m_audioMusic == null)
                m_audioMusic = gameObject.AddComponent<AudioSource>();

            return m_audioMusic;
        }
    }

    public GameData Data
    {
        get
        {
            if (m_gameSystem == null)
                m_gameSystem = GameSystem.Instance;

            return m_gameSystem.gameData;
        }
    }

    void Awake ()
    {
        m_gameSystem = GameSystem.Instance;
    }

    void Start ()
    {
        PlayMusic(Data.RandomMusicSound(), true, Data.musicVolumeInGame);
    }

    void OnEnable ()
    {
        m_gameSystem.gameEvent.OnGameRestart += OnGameRestart;
        m_gameSystem.gameEvent.OnStateUpdate += OnStateUpdate;
    }

    void OnDisable ()
    {
        m_gameSystem.gameEvent.OnGameRestart -= OnGameRestart;
        m_gameSystem.gameEvent.OnStateUpdate -= OnStateUpdate;
    }

    public void SetMute (bool _value)
    {
        Sound.mute = _value;
        Music.mute = _value;
    }

    public void SwitchMute ()
    {
        Sound.mute = !Sound.mute;
        Music.mute = !Music.mute;
    }

    public bool IsMuted ()
    {
        return Sound.mute && Music.mute;
    }

    public void PlaySound (AudioClip _clip, bool _isUnmuted = false)
    {
        if (_clip == null)
            return;
        
        if (_isUnmuted)
            Menus.PlayOneShot(_clip);
        else
            Sound.PlayOneShot(_clip);
    }
    
    public void PlayMusic (AudioClip _clip, bool _loop = true, float _volume = 1f)
    {
        if (_clip != null)
        {
            Music.volume = _volume;
            Music.loop = _loop;
            Music.clip = _clip;
            Music.Play();
        }
    }

    void OnGameRestart ()
    {
        Music.volume = Data.musicVolumeInGame;
    }
    
    void OnStateUpdate (GameSystem.State _currState)
    {
        if (m_gameSystem.IsGameOver())
            Music.volume = Data.musicVolumeGameOver;
    }
}
