using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Level : MonoBehaviour
{
    public TextMeshProUGUI textIndex;

    LevelManager m_levelManager;
    GameObject m_textObject;
    MenuLevel m_menuLevel;
    Button m_button;

    public void Init (LevelManager _level, MenuLevel _menuLevel, int _index)
    {
        m_levelManager = _level;
        m_menuLevel = _menuLevel;

        m_button = GetComponent<Button>();
        m_button.onClick.AddListener(ButtonClick);

        textIndex.text = (_index + 1).ToString();
        m_textObject = textIndex.gameObject;

        Show(false);
    }

    public void Show (bool _value)
    {
        m_textObject.SetActive(_value);
        m_button.interactable = _value;
    }

    void ButtonClick ()
    {
        if (m_menuLevel != null)
            m_menuLevel.OnSelectLevel(m_levelManager);
    }
}
