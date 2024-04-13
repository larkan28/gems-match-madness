using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public RectTransform rectOrigin;

    Tile[,] m_tiles;

    int m_sizeX;
    int m_sizeY;

    float m_tileDiameterX;
    float m_tileDiameterY;

    Vector2 m_worldSize;
    Vector2 m_tileScale;

    GameData m_gameData;
    Transform m_tilesRoot;
    LevelManager m_levelManager;

    bool m_isInitialized;

    public Tile[,] Tiles
    {
        get => m_tiles;
    }

    public Vector2 TileScale
    {
        get => m_tileScale;
    }

    Transform TilesRoot
    {
        get
        {
            if (m_tilesRoot == null)
                m_tilesRoot = transform.GetChild(0);

            return (m_tilesRoot == null) ? transform : m_tilesRoot;
        }
    }

    void Awake ()
    {
        if (rectOrigin != null)
            transform.position = Util.ScreenToWorldPoint(rectOrigin);
    }

    public void Init (IBoardHandler _handler, LevelManager _level, GameData _data)
    {
        m_isInitialized = false;

        if (m_tiles != null)
        {
            foreach (var tile in m_tiles)
                DestroyTile(tile);
        }

        if (_level == null || _data == null)
            return;

        m_gameData = _data;
        m_levelManager = _level;

        m_worldSize = CalculateWorldSize();
        m_worldSize *= m_levelManager.worldScale;

        m_sizeX = m_levelManager.sizeX;
        m_sizeY = m_levelManager.sizeY;

        m_tileDiameterX = m_worldSize.x / m_sizeX;
        m_tileDiameterY = m_worldSize.y / m_sizeY;
        m_tileScale = new Vector2(1f * (m_tileDiameterX / 2f), 1f * (m_tileDiameterY / 2f));

        m_tiles = new Tile[m_sizeX, m_sizeY];

        StopAllCoroutines();
        StartCoroutine(CR_CreateBoard(_handler));
    }

    IEnumerator CR_CreateBoard (IBoardHandler _handler)
    {
        Vector3 worldBottomLeft = transform.position - Vector3.right * m_worldSize.x / 2f - Vector3.up * m_worldSize.y / 2f;

        float radiusX = m_tileDiameterX / 2f;
        float radiusY = m_tileDiameterY / 2f;
        
        float delayCreate = m_gameData.createTileDelay;

        for (int x = 0; x < m_sizeX; x++)
        {
            for (int y = 0; y < m_sizeY; y++)
            {
                Tile tile = Instantiate(m_gameData.tilePrefab, TilesRoot);

                if (tile != null)
                {
                    Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * m_tileDiameterX + radiusX) + Vector3.up * (y * m_tileDiameterY + radiusY);

                    tile.x = x;
                    tile.y = y;
                    tile.board = this;
                    tile.Init(m_gameData, worldPoint, m_tileScale);

                    m_tiles[x, y] = tile;
                }
            }

            if (delayCreate > 0f)
                yield return new WaitForSeconds(delayCreate);
        }

        foreach (var tileToRemove in m_levelManager.tilesToRemove)
        {
            int x = tileToRemove.x;
            int y = tileToRemove.y;

            if (!IsValidTile(x, y))
                continue;

            if (tileToRemove.isFrozen)
                m_tiles[x, y].IsFrozen = true;
            else
                DestroyTile(m_tiles[x, y]);
        }

        m_isInitialized = true;

        if (_handler != null)
            _handler.OnBoardCreated();
    }

    void DestroyTile (Tile _tile)
    {
        if (m_tiles == null || _tile == null)
            return;

        Destroy(_tile.gameObject);
        m_tiles[_tile.x, _tile.y] = null;
    }

    bool IsValidTile (int _x, int _y)
    {
        return _x >= 0 && _x < m_sizeX && _y >= 0 && _y < m_sizeY;
    }

    Vector2 CalculateWorldSize ()
    {
        Vector2 size = Util.CameraBoundsToVec(Camera.main);

        int w = Screen.width;
        int h = Screen.height;

        int min = Mathf.Min(w, h);
        int max = Mathf.Max(w, h);
        int diff = max - min;

        float percent = 1f - ((float) diff / max);

        if (w > h)
            size.x *= percent;
        else
            size.y *= percent;

        return size;
    }

    public bool IsInitialized ()
    {
        return m_isInitialized;
    }

    public bool AreNeighbours (Tile _tileA, Tile _tileB)
    {
        if (!m_isInitialized || _tileA == null || _tileB == null)
            return false;
            
        int distX = Mathf.Abs(_tileA.x - _tileB.x);
        int distY = Mathf.Abs(_tileA.y - _tileB.y);

        if (distX == 1 && distY == 1)
            return m_levelManager.allowDiagonals;

        return distX <= 1 && distY <= 1;
    }

    public Tile RandomTile (bool _isEmpty, Tile _ignoredTile = null)
    {
        if (!m_isInitialized)
            return null;

        List<Tile> availableTiles = new List<Tile>();

        foreach (var tile in m_tiles)
        {
            if (tile == null)
                continue;
                
            if (tile.IsEmpty() == _isEmpty)
                availableTiles.Add(tile);
        }

        if (_ignoredTile != null)
            availableTiles.Remove(_ignoredTile);

        return availableTiles[Random.Range(0, availableTiles.Count)];
    }

    public Tile RandomTile ()
    {
        if (!m_isInitialized)
            return null;

        int x = Random.Range(0, m_sizeX);
        int y = Random.Range(0, m_sizeY);
        
        return m_tiles[x, y];
    }

    public Tile FindTileIn (int _x, int _y)
    {
        if (!m_isInitialized || !IsValidTile(_x, _y))
            return null;

        return m_tiles[_x, _y];
    }

    public Tile[] GetNeighbours (Tile _tile, bool _includeDiagonals = false)
    {
        if (!m_isInitialized || _tile == null)
            return null;

        List<Tile> results = new List<Tile>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                if (!_includeDiagonals && (x == -1  || x == 1) && y != 0)
                    continue;

                int posX = _tile.x + x;
                int posY = _tile.y + y;

                if (IsValidTile(posX, posY))
                    results.Add(m_tiles[posX, posY]);
            }
        }

        return results.ToArray();
    }

    public Tile[] GetVerticalNeighbours (Tile _tile)
    {
        if (!m_isInitialized || _tile == null)
            return null;

        List<Tile> results = new List<Tile>();

        for (int i = 0; i < m_sizeY; i++)
            results.Add(m_tiles[_tile.x, i]);

        return results.ToArray();
    }

    public Tile[] GetHorizontalNeighbours (Tile _tile)
    {
        if (!m_isInitialized || _tile == null)
            return null;

        List<Tile> results = new List<Tile>();

        for (int i = 0; i < m_sizeX; i++)
            results.Add(m_tiles[i, _tile.y]);

        return results.ToArray();
    }
}
