using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public LayerMask layerItems;

    int m_movesLeft;

    public string LeftMovesText
    {
        get
        {
            if (m_levelManager == null || m_levelManager.maxMoves <= 0)
                return "-/-";

            return m_movesLeft + "/" + m_levelManager.maxMoves;
        }
    }

    public int LeftMoves
    {
        get => m_movesLeft;
    }

    float m_timeLeft;

    public int Stars
    {
        get
        {
            if (m_levelManager == null || (m_levelManager.maxSeconds > 0 && m_timeLeft <= 0))
                return 0;

            int maxMoves = m_levelManager.maxMoves;

            if (maxMoves > 0)
            {
                float percent = (float) m_movesLeft / maxMoves;

                if (percent >= m_gameData.starsPercent3)
                    return 3;
                else if (percent >= m_gameData.starsPercent2)
                    return 2;
                else if (percent >= m_gameData.starsPercent1)
                    return 1;
            }

            return 0;
        }
    }

    Item m_currItem;
    Player m_player;
    Camera m_mainCamera;
    ItemData m_currData;
    GameData m_gameData;
    GameInput m_gameInput;
    GameSound m_gameSound;
    GameEvent m_gameEvent;
    GameSystem m_gameSystem;
    List<Item> m_listItems;
    LevelManager m_levelManager;
    LineRenderer m_lineRenderer;
    BoardManager m_boardManager;

    public void Init (Player _player)
    {
        m_player = _player;
        m_listItems = new List<Item>();
        m_gameEvent = m_player.Events;
        m_gameInput = GameInput.Instance;
        m_gameSound = GameSound.Instance;
        m_gameSystem = GameSystem.Instance;
        m_mainCamera = Camera.main;
        m_lineRenderer = GetComponent<LineRenderer>();
        m_boardManager = m_gameSystem.BoardManager;
    }

    public void Think ()
    {
        if (m_levelManager == null || m_movesLeft < 1)
            return;

        if (m_gameInput.SelectDown())
            SelectStart();

        if (m_gameInput.SelectHold())
            Selecting();

        if (m_gameInput.SelectUp())
            SelectEnd();

        if (m_timeLeft >= 0)
            SubstractTime();
    }

    public void Restart ()
    {
        m_gameData = m_gameSystem.gameData;
        m_levelManager = m_gameSystem.CurrLevel;

        if (m_levelManager != null)
        {
            m_movesLeft = m_levelManager.maxMoves;
            m_gameEvent.UpdateMoves(m_movesLeft);

            m_timeLeft = m_levelManager.maxSeconds;
            m_gameEvent.UpdateTimer(Mathf.FloorToInt(m_timeLeft));
        }

        m_listItems.Clear();

        m_currItem = null;
        m_currData = null;

        ResetConnections();
    }

    void SelectStart ()
    {
        if (m_boardManager.IsDisabledSelection)
            return;

        if (m_listItems.Count > 0)
        {
            foreach (var item in m_listItems)
                item.PlayAnimation("Deselect");
        }

        ResetConnections();
        m_listItems.Clear();

        m_currItem = GetItemOverMouse();
        m_currData = null;

        SetMatching();
    }

    void Selecting ()
    {
        if (m_boardManager.IsDisabledSelection)
            return;

        Item item = GetItemOverMouse();

        if (!m_boardManager.IsValidMatch(m_currData, m_currItem, item))
            return;

        if (m_listItems.Contains(item))
        {
            if (m_listItems.Count > 1)
            {
                Item last = m_listItems[m_listItems.Count - 1];
                Item prev = m_listItems[m_listItems.Count - 2];

                if (item == prev)
                {
                    last.PlayAnimation("Deselect");

                    m_listItems.Remove(last);
                    m_currItem = item;

                    DrawConnections();
                    PlaySelectSound();

                    SetMatching();
                }
            }

            return;
        }

        item.PlayAnimation("Select");

        m_listItems.Add(item);
        m_currItem = item;

        DrawConnections();
        PlaySelectSound();

        SetMatching();
    }

    void SelectEnd ()
    {
        int count = m_listItems.Count;

        if (count >= 3)
        {
            SubstractMove();

            m_boardManager.CollectItems(m_listItems.ToArray());
            m_player.playerScore.AddScore(count);
        }
        else
        {
            foreach (var item in m_listItems)
                item.PlayAnimation("Deselect");
        }

        ResetConnections();

        m_currItem = null;
        m_currData = null;

        m_listItems.Clear();
    }

    void SetMatching ()
    {
        foreach (var item in m_listItems)
        {
            if (!item.Data.isJoker)
            {
                m_currData = item.Data;
                return;
            }
        }

        m_currData = null;
    }

    void PlaySelectSound ()
    {
        int count = m_listItems.Count;
        m_gameSound.PlaySound(m_gameData.SelectSoundAt(count));
    }

    void DrawConnections ()
    {
        int maxConnections = m_listItems.Count;

        if (maxConnections > 0)
        {
            m_lineRenderer.positionCount = maxConnections;

            if (m_currData != null)
                m_lineRenderer.colorGradient = m_currData.gradient;

            for (int i = 0; i < maxConnections; i++)
                m_lineRenderer.SetPosition(i, m_listItems[i].Tile.Point);

            m_gameEvent.ShowConnections(maxConnections, m_currData, m_listItems[maxConnections - 1].Tile.Point);
        }
    }

    void ResetConnections ()
    {
        m_lineRenderer.positionCount = 0;
        m_gameEvent.ShowConnections(0, null, Vector2.zero);
    }

    void SubstractMove ()
    {
        m_movesLeft--;
        m_gameEvent.UpdateMoves(m_movesLeft);
    }

    void SubstractTime ()
    {
        m_timeLeft -= Time.deltaTime;
        m_gameEvent.UpdateTimer(Mathf.FloorToInt(m_timeLeft));

        if (m_timeLeft <= 0)
            m_gameSystem.SetState(GameSystem.State.Failed);
    }

    Item GetItemOverMouse ()
    {
        Vector3 origin = m_mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = Vector2.zero;

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, 1f, layerItems);

        if (hit)
            return hit.collider.GetComponent<Item>();

        return null;
    }
}
