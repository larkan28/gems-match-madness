using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public float moveSpeed;

    protected Tile m_tile;
    protected Vector3 m_point;
    protected Vector3 m_scale;
    protected Animator m_animator;
    protected ItemData m_itemData;
    protected GameData m_gameData;
    protected GameSound m_gameSound;
    protected GameSystem m_gameSystem;
    protected BoardManager m_boardManager;
    protected bool m_isReordered;
    protected bool m_isCollected;
    protected int m_itemLife;

    public bool HasReordered
    {
        get => m_isReordered;
        set => m_isReordered = value;
    }

    public Vector3 Point
    {
        get => m_point;
        set => m_point = value;
    }

    public Tile Tile
    {
        get => m_tile;
        set
        {
            if (m_tile != null && value == null)
                m_tile.item = null;

            m_tile = value;

            if (m_tile != null)
            {
                m_point = m_tile.Point;
                m_tile.item = this;
            }

            SetScale();
        }
    }

    public ItemData Data
    {
        get => m_itemData;
        set => m_itemData = value;
    }

    void Update ()
    {
        Vector3 pos = transform.position;

        if (pos != m_point)
            transform.position = Vector3.MoveTowards(pos, m_point, moveSpeed * Time.deltaTime);
        else
        {
            if (m_isCollected)
                PlayDestroy();

            if (m_isReordered)
                PlayReorder();
        }
    }

    public void Init (ItemData _itemData, Vector2 _scale)
    {
        m_gameData = GameSystem.Instance.gameData;
        m_gameSound = GameSound.Instance;
        m_gameSystem = GameSystem.Instance;
        m_boardManager = m_gameSystem.BoardManager;

        m_itemLife = _itemData.maxLifes;
        m_itemData = _itemData;
        m_animator = GetComponentInChildren<Animator>();

        SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (spriteRenderer != null)
            spriteRenderer.sprite = m_itemData.icon;

        m_scale = _scale;
        SetScale();

        OnInit();
    }

    public void PlayAnimation (string _name)
    {
        m_animator.SetTrigger(_name);
    }

    public void PlayDestroy ()
    {
        if (m_isCollected && m_tile != null)
        {
            Collect();
            return;
        }

        m_isCollected = false;
        m_gameSystem.SendItemToGoals(m_itemData);
        
        m_animator.SetTrigger("Destroy");
    }

    void PlayReorder ()
    {
        m_isReordered = false;
        m_animator.SetTrigger("Jump");
    }

    public void MoveImmediatly ()
    {
        transform.position = m_point;
    }

    public void MoveTo (Vector2 _point)
    {
        m_isCollected = true;
        m_point = _point;
    }

    public void Collect ()
    {
        if (m_tile == null)
            return;

        if (m_itemLife > 0 && (--m_itemLife) > 0)
        {
            OnDamaged();
            return;
        }

        Tile = null;

        if (TryGetComponent(out BoxCollider2D boxCollider2D))
            boxCollider2D.enabled = false;
        
        m_gameSound.PlaySound(m_gameData.soundItemCollect);
        m_gameSystem.gameEvent.ItemCollect(this);
    }

    public void DestroyImmediatly ()
    {
        Tile = null;
        Destroy(gameObject);
    }

    public void SetScale ()
    {
        if (m_tile != null && m_tile.IsFrozen)
            transform.localScale = m_scale * 0.75f;
        else
            transform.localScale = m_scale;
    }

    public bool IsMoving ()
    {
        return transform.position != m_point;
    }

    public bool IsAvailableToSelect ()
    {
        return m_itemData.isSelectable && !m_tile.IsFrozen;
    }

    public override string ToString ()
    {
        return m_itemData.ToString() + "(x:" + m_tile.x + ",y:" + m_tile.y + ")";
    }

    public virtual void OnInit () { }
    public virtual void OnEffect (ItemData _matching) { }
    public virtual void OnDamaged () { }
}