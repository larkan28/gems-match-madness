using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class UI_Connection : MonoBehaviour
{
    public Vector3 offset;
    public GameObject root;
    public TextMeshProUGUI textAmount;
    public Image imageIcon;

    RectTransform m_rectTransform;
    GameObject m_iconRoot;
    GameObject m_textRoot;
    GameSystem m_gameSystem;
    GameEvent m_gameEvent;
    GameData m_gameData;

    void Awake ()
    {
        m_rectTransform = GetComponent<RectTransform>();
        m_gameSystem = GameSystem.Instance;
        m_gameEvent = m_gameSystem.gameEvent;
        m_gameData = m_gameSystem.gameData;
        m_iconRoot = imageIcon.gameObject;
        m_textRoot = textAmount.gameObject;
    }

    void OnEnable ()
    {
        m_gameEvent.OnShowConnections += OnShowConnections;
        m_gameEvent.OnStateUpdate += OnStateUpdate;
    }

    void OnDisable ()
    {
        m_gameEvent.OnShowConnections -= OnShowConnections;
        m_gameEvent.OnStateUpdate -= OnStateUpdate;
    }

    void OnShowConnections (int _amount, ItemData _data, Vector2 _center)
    {
        if (_amount <= 0)
            root.SetActive(false);
        else
        {
            root.SetActive(true);

            ItemData itemData = m_gameData.ItemAtCombo(_amount - 1, _data);

            if (itemData != null)
            {
                m_iconRoot.SetActive(true);
                m_textRoot.SetActive(false);

                imageIcon.sprite = itemData.icon;
            }
            else
            {
                m_iconRoot.SetActive(false);
                m_textRoot.SetActive(true);

                textAmount.text = _amount.ToString();
            }

            m_rectTransform.position = Camera.main.WorldToScreenPoint(_center) + offset;
        }
    }

    void OnStateUpdate (GameSystem.State _currState)
    {
        if (root.activeSelf && m_gameSystem.IsGameOver())
            root.SetActive(false);
    }
}
