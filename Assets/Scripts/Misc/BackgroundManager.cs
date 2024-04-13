using UnityEngine;
using UnityEngine.UI;

public class BackgroundManager : MonoBehaviour
{
    public Sprite[] backgrounds;

    Image m_image;
    GameEvent m_gameEvent;

    void Awake ()
    {
        m_image = GetComponent<Image>();
        m_gameEvent = GameSystem.Instance.gameEvent;

        ChangeBackground();
    }

    void OnEnable ()
    {
        m_gameEvent.OnGameRestart += OnGameRestart;
    }

    void OnDisable ()
    {
        m_gameEvent.OnGameRestart -= OnGameRestart;
    }

    void ChangeBackground ()
    {
        m_image.sprite = backgrounds[Random.Range(0, backgrounds.Length)];
    }

    void OnGameRestart ()
    {
        ChangeBackground();
    }
}
