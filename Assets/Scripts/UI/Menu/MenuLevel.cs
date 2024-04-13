using UnityEngine;

public class MenuLevel : MonoBehaviour
{
    public MenuPause menuPause;
    public MenuGameOver menuGameOver;

    public GameObject root;
    public Transform rootLevels;
    public UI_Level levelPrefab;

    GameSystem m_gameSystem;
    UI_Level[] m_levels;

    void Awake ()
    {
        m_gameSystem = GameSystem.Instance;

        CreateLevels();
        UpdateLevels();
    }

    void OnEnable ()
    {
        m_gameSystem.gameEvent.OnStateUpdate += OnStateUpdate;
    }

    void OnDisable ()
    {
        m_gameSystem.gameEvent.OnStateUpdate -= OnStateUpdate;
    }

    void CreateLevels ()
    {
        LevelManager[] levels = m_gameSystem.levels;
        m_levels = new UI_Level[levels.Length];

        for (int i = 0; i < m_levels.Length; i++)
        {
            m_levels[i] = Instantiate(levelPrefab, rootLevels);
            m_levels[i].Init(levels[i], this, i);
        }
    }

    void UpdateLevels ()
    {
        int levelsUnlocked = m_gameSystem.LevelsUnlocked;
        
        for (int i = 0; i < m_levels.Length; i++)
            m_levels[i].Show(levelsUnlocked >= i);
    }

    void OnStateUpdate (GameSystem.State _currState)
    {
        ShowMenu(false);
    }

    public void ButtonExit ()
    {
        if (m_gameSystem.IsGameOver())
            menuGameOver.ShowMenu(true);
        else
            menuPause.ShowMenu(true);

        ShowMenu(false);
    }

    public void ShowMenu (bool _value)
    {
        root.SetActive(_value);

        if (root.activeSelf)
            UpdateLevels();
    }

    public void OnSelectLevel (LevelManager _level)
    {
        m_gameSystem.PlayLevel(_level);
    }
}
