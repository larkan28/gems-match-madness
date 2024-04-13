using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuGameOver : MonoBehaviour
{
    public MenuLevel menuLevel;

    public GameObject root;
    public GameObject[] stars;

    public Button buttonPlay;
    public Button buttonLevel;
    public Button buttonReplay;

    public string textWin;
    public string textFail;

    public TextMeshProUGUI textTitle;
    public TextMeshProUGUI textScore;
    public TextMeshProUGUI textMoves;

    GameSound m_gameSound;
    GameSystem m_gameSystem;

    void Awake ()
    {
        m_gameSound = GameSound.Instance;
        m_gameSystem = GameSystem.Instance;

        buttonPlay.onClick.AddListener(ButtonPlay);
        buttonLevel.onClick.AddListener(ButtonLevel);
        buttonReplay.onClick.AddListener(ButtonReplay);
    }

    void OnEnable ()
    {
        m_gameSystem.gameEvent.OnStateUpdate += OnStateUpdate;
    }

    void OnDisable ()
    {
        m_gameSystem.gameEvent.OnStateUpdate -= OnStateUpdate;        
    }

    void LevelCompleted ()
    {
        textTitle.text = textWin;

        buttonPlay.interactable = !m_gameSystem.IsOnLevelMax();
        buttonLevel.interactable = true;
        buttonReplay.interactable = true;

        m_gameSound.PlaySound(m_gameSound.Data.soundGameWin);
    }

    void LevelFailed ()
    {
        textTitle.text = textFail;

        buttonPlay.interactable = false;
        buttonLevel.interactable = true;
        buttonReplay.interactable = true;

        m_gameSound.PlaySound(m_gameSound.Data.soundGameLose);
    }

    void OnStateUpdate (GameSystem.State _currState)
    {
        root.SetActive(m_gameSystem.IsGameOver());
        
        if (root.activeSelf)
        {
            switch (_currState)
            {
                case GameSystem.State.Won:
                    LevelCompleted();
                    break;
                case GameSystem.State.Failed:
                    LevelFailed();
                    break;
            }

            Player player = m_gameSystem.GetPlayer();

            if (player != null)
            {
                int starsObtained = player.playerController.Stars;

                textScore.text = player.playerScore.Score.ToString();
                textMoves.text = player.playerController.LeftMovesText;

                for (int i = 0; i < stars.Length; i++)
                    stars[i].SetActive((i + 1) <= starsObtained);
            }
        }
    }

    void ButtonLevel ()
    {
        menuLevel.ShowMenu(true);
        ShowMenu(false);
    }

    void ButtonReplay ()
    {
        m_gameSystem.Restart();
    }

    void ButtonPlay ()
    {
        m_gameSystem.ShowAd();
    }

    public void ShowMenu (bool _value)
    {
        root.SetActive(_value);
    }
}
