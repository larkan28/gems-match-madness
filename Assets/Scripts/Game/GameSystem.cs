using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GameSystem : Singleton<GameSystem>
{
    public enum State
    {
        Paused = 0,
        Playing,
        Won,
        Failed
    };

    public GameData gameData;
    public GameEvent gameEvent;
    public LevelManager[] levels;

    State m_currState;
    LevelManager m_currLevel;
    BoardManager m_boardManager;
    InterstitialAd m_interstitialAd;
    List<Item> m_collectingItems = new List<Item>();
    int m_levelsUnlocked;

    public int LevelsUnlocked
    {
        get => m_levelsUnlocked;
    }

    public LevelManager CurrLevel
    {
        get => m_currLevel;
    }

    void Awake ()
    {
        DataBase.LoadAll();

        m_levelsUnlocked = DataBase.Load("LevelsUnlocked");
        m_interstitialAd = GetComponent<InterstitialAd>();

        Task task = m_interstitialAd.InitServices();
    }

    void Start ()
    {
        Application.targetFrameRate = 60;
        PlayLevel(levels[Mathf.Clamp(m_levelsUnlocked, 0, levels.Length - 1)]);
    }

    void Update ()
    {
        if (m_collectingItems.Count < 1)
            return;

        int count = 0;

        foreach (var item in m_collectingItems)
        {
            if (item != null)
                count++;
        }

        if (count <= 0)
            SetState(State.Failed);
    }

    void OnApplicationQuit ()
    {
        DataBase.SaveAll();
    }

    public void PlayLevel (LevelManager _level)
    {
        if (_level == null)
            _level = levels[0];

        m_currLevel = _level;
        gameEvent.GameRestart();

        m_currLevel.Init();
        m_collectingItems.Clear();

        SetState(State.Playing, true);
    }

    public void Restart ()
    {
        PlayLevel(m_currLevel);
    }

    public void ShowAd ()
    {
        m_interstitialAd.ShowAd();
    }

    public void NextLevel ()
    {
        int index = LevelIndex(m_currLevel) + 1;

        if (index < levels.Length)
            PlayLevel(levels[index]);
    }

    public void SwitchPause ()
    {
        if (m_currState == State.Paused)
            SetState(State.Playing);
        else
            SetState(State.Paused);
    }

    public void SetState (State _newState, bool _forceEnter = false)
    {
        if (!IsAvaiableToChangeState(m_currState, _newState) && !_forceEnter)
            return;

        m_currState = _newState;

        if (m_currState == State.Paused)
            Time.timeScale = 0f;
        else
            Time.timeScale = 1f;

        gameEvent.UpdateState(m_currState);
    }

    public void CheckLastMove (Item[] _existingItems)
    {
        m_collectingItems.Clear();

        foreach (var item in _existingItems)
        {
            if (item.Tile == null)
                m_collectingItems.Add(item);
        }

        if (m_collectingItems.Count < 1)
            SetState(State.Failed);
    }

    public void SendItemToGoals (ItemData _itemCollected)
    {
        m_currLevel.SendUpdateToGoals(_itemCollected);

        if (m_currLevel.IsGoalsCompleted())
        {
            int levelUnlocked = LevelIndex(m_currLevel) + 1;

            if (levelUnlocked > m_levelsUnlocked)
            {
                m_levelsUnlocked = levelUnlocked;
                DataBase.Save("LevelsUnlocked", m_levelsUnlocked);
            }

            SetState(State.Won);
        }
    }

    public bool IsOnLevelMax ()
    {
        return LevelIndex(m_currLevel) >= (levels.Length - 1);
    }

    int LevelIndex (LevelManager _level)
    {
        for (int i = 0; i < levels.Length; i++)
        {
            if (levels[i] == _level)
                return i;
        }

        return 0;
    }

    bool IsAvaiableToChangeState (State _currState, State _newState)
    {
        if (m_currState == _newState)
            return false;

        switch (_newState)
        {
            case State.Paused: return (_currState == State.Playing);
            case State.Playing: return (_currState == State.Paused);
            case State.Won: return (_currState != State.Failed);
            case State.Failed: return (_currState != State.Won);
        }

        return false;
    }

    public bool IsPaused ()
    {
        return m_currState != State.Playing;
    }

    public bool IsGameOver ()
    {
        return m_currState == State.Won || m_currState == State.Failed;
    }

    public Player GetPlayer ()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
            return player.GetComponent<Player>();

        return null;
    }

    public BoardManager BoardManager
    {
        get
        {
            if (m_boardManager == null)
            {
                GameObject board = GameObject.FindGameObjectWithTag("Board");

                if (board != null)
                    m_boardManager = board.GetComponent<BoardManager>();
            }

            return m_boardManager;
        }
    }
}
