using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Board))]
public class BoardManager : MonoBehaviour, IBoardHandler
{
    [HideInInspector] public bool isWaitingForResponse;
    
    Board m_board;
    Player m_player;
    GameData m_gameData;
    Transform m_itemsRoot;
    GameSound m_gameSound;
    GameSystem m_gameSystem;
    LevelManager m_levelManager;

    List<Tile> m_listTileFrozens;
    List<Item> m_listItemEffects;
    List<Item> m_listItemMatches;

    WaitForSeconds m_orderItemsDelay;
    WaitForSeconds m_createItemDelay;
    WaitForSeconds m_startBoardDelay;
    WaitForSeconds m_removeStepDelay;
    WaitForSeconds m_removeItemDelay;

    Transform ItemsRoot
    {
        get
        {
            if (m_itemsRoot == null)
                m_itemsRoot = transform.GetChild(1);

            return (m_itemsRoot == null) ? transform : m_itemsRoot;
        }
    }

    public Board Board
    {
        get => m_board;
    }

    bool m_isDisabledSelection;

    public bool IsDisabledSelection
    {
        get => m_isDisabledSelection;
    }

    void Awake ()
    {
        m_gameSound = GameSound.Instance;
        m_gameSystem = GameSystem.Instance;
        m_listTileFrozens = new List<Tile>();
        m_listItemEffects = new List<Item>();
        m_listItemMatches = new List<Item>();

        m_board = GetComponent<Board>();
    }

    void OnEnable ()
    {
        m_gameSystem.gameEvent.OnGameRestart += OnGameRestart;
    }

    void OnDisable ()
    {
        m_gameSystem.gameEvent.OnGameRestart -= OnGameRestart;
    }

    void OnGameRestart ()
    {
        Item[] items = ItemsRoot.GetComponentsInChildren<Item>();

        if (items != null)
        {
            foreach (var item in items)
                item.DestroyImmediatly();
        }

        m_player = m_gameSystem.GetPlayer();
        m_gameData = m_gameSystem.gameData;
        m_levelManager = m_gameSystem.CurrLevel;

        m_listTileFrozens.Clear();
        m_listItemEffects.Clear();
        m_listItemMatches.Clear();

        m_orderItemsDelay = new WaitForSeconds(m_gameData.createItemDelay);
        m_createItemDelay = new WaitForSeconds(m_gameData.createItemDelay);
        m_startBoardDelay = new WaitForSeconds(m_gameData.startBoardDelay);
        m_removeStepDelay = new WaitForSeconds(m_gameData.removeStepDelay);
        m_removeItemDelay = new WaitForSeconds(m_gameData.removeItemDelay);

        StopAllCoroutines();
        m_board.Init(this, m_levelManager, m_gameData);
    }

    void CreateItem (Tile _tileParent, ItemData _data = null, bool _playSound = true)
    {
        if (_data == null)
            _data = m_gameData.RandomNormalItem();

        Item item = Instantiate(_data.itemPrefab, ItemsRoot);

        if (item != null)
        {
            item.Tile = _tileParent;
            item.Init(_data, m_board.TileScale);

            if (_playSound)
                m_gameSound.PlaySound(m_gameData.soundItemCreate);
        }
    }

    void ItemCollect (Item _item)
    {
        Tile[] neighbours = m_board.GetNeighbours(_item.Tile);

        if (neighbours != null)
        {
            foreach (var neighbour in neighbours)
            {
                if (neighbour == null)
                    continue;

                if (neighbour.IsFrozen && !m_listTileFrozens.Contains(neighbour))
                {
                    m_listTileFrozens.Add(neighbour);
                    continue;
                }
                
                Item item = neighbour.item;

                if (item == null || !item.Data.affectedByMatch)
                    continue;

                if (m_listItemMatches.Contains(item))
                    continue;

                m_listItemMatches.Add(item);
            }
        }

        _item.Collect();
    }

    IEnumerator CR_FillBoard ()
    {
        m_isDisabledSelection = true;

        if (!m_board.IsInitialized())
            yield return null;

        yield return m_startBoardDelay;

        foreach (var tile in m_board.Tiles)
        {
            if (tile != null && tile.IsEmpty())
            {
                CreateItem(tile);
                yield return m_createItemDelay;
            }
        }

        m_isDisabledSelection = false;
    }

    IEnumerator CR_CollectItems (Item[] _items)
    {
        m_isDisabledSelection = true;
        Tile lastTile = _items[_items.Length - 1].Tile;

        m_listTileFrozens.Clear();
        m_listItemEffects.Clear();
        m_listItemMatches.Clear();

        foreach (var item in _items)
        {
            if (item.Data.hasEffect)
                m_listItemEffects.Add(item);
            else
            {
                ItemCollect(item);
                yield return m_removeItemDelay;
            }
        }

        ItemData nonJokerItem = Util.NotNullColorIn(_items);

        if (m_listItemEffects.Count > 0)
        {
            yield return m_removeStepDelay;

            foreach (var item in m_listItemEffects)
            {
                if (item == null)
                    continue;

                item.OnEffect(nonJokerItem);

                while (isWaitingForResponse)
                    yield return new WaitForEndOfFrame();

                yield return m_removeItemDelay;
            }
        }

        if (m_listItemMatches.Count > 0)
        {
            yield return m_removeStepDelay;

            foreach (var item in m_listItemMatches)
            {
                if (item == null)
                    continue;

                item.Collect();

                while (isWaitingForResponse)
                    yield return new WaitForEndOfFrame();

                yield return m_removeItemDelay;
            }
        }

        if (m_listTileFrozens.Count > 0)
        {
            yield return m_removeStepDelay;

            foreach (var tile in m_listTileFrozens)
            {
                m_gameSystem.SendItemToGoals(m_gameData.tileFrozen);

                tile.IsFrozen = false;
                yield return m_removeItemDelay;
            }
        }

        if (lastTile != null)
        {
            yield return m_removeStepDelay;

            ItemData currData = _items[0].Data;
            ItemData itemData = m_gameData.ItemAtCombo(_items.Length - 1, currData);

            if (itemData != null)
                CreateItem(lastTile, itemData);
        }

        yield return m_removeStepDelay;

        if (m_player != null && m_player.playerController.LeftMoves <= 0)
        {
            Item[] existingItems = ItemsRoot.GetComponentsInChildren<Item>();
            m_gameSystem.CheckLastMove(existingItems);
        }

        Tile[,] tiles = m_board.Tiles;

        int sizeX = tiles.GetLength(0);
        int sizeY = tiles.GetLength(1);

        for (int x = 0; x < sizeX; x++)
        {
            bool hasReordered = false;

            for (int y = 0; y < sizeY; y++)
            {
                Tile currTile = tiles[x, y];

                if (currTile == null || currTile.IsEmpty())
                    continue;

                if (!currTile.item.IsAvailableToSelect())
                    continue;

                lastTile = null;

                for (int i = (y - 1); i >= 0; i--)
                {
                    Tile deepTile = tiles[x, i];

                    if (deepTile != null && deepTile.IsEmpty())
                        lastTile = deepTile;
                }

                if (lastTile != null)
                {
                    currTile.item.HasReordered = true;
                    currTile.item.Tile = lastTile;
                    currTile.item = null;

                    hasReordered = true;
                }
            }

            if (hasReordered)
            {
                m_gameSound.PlaySound(m_gameData.soundReorder);
                yield return m_orderItemsDelay;
            }
        }

        yield return m_removeStepDelay;

        m_isDisabledSelection = false;
        RefillBoard();
    }

    public void OnBoardCreated ()
    {
        foreach (var itemToCreate in m_levelManager.itemsToCreate)
        {
            Tile tile = m_board.FindTileIn(itemToCreate.x, itemToCreate.y);

            if (tile != null)
                CreateItem(tile, itemToCreate.itemData, false);
        }

        StopAllCoroutines();
        StartCoroutine(CR_FillBoard());
    }

    public void RefillBoard ()
    {
        StopAllCoroutines();
        StartCoroutine(CR_FillBoard());
    }

    public void CollectItems (Item[] _items)
    {
        StopAllCoroutines();
        StartCoroutine(CR_CollectItems(_items));
    }

    public bool IsValidMatch (ItemData _match, Item _curr, Item _selected)
    {
        if (_curr == null || _selected == null || !_selected.IsAvailableToSelect())
            return false;

        if (_match != null && !_selected.Data.isJoker && !_match.MatchsTo(_selected.Data))
            return false;

        return m_board.AreNeighbours(_curr.Tile, _selected.Tile);
    }
}
