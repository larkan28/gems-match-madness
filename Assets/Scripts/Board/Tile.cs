using UnityEngine;

public class Tile : MonoBehaviour
{
    public GameObject rootIcon;
    public GameObject rootFrozen;

    [HideInInspector] public int x;
    [HideInInspector] public int y;
    [HideInInspector] public Item item;
    [HideInInspector] public Board board;

    bool m_isFrozen = false;

    public bool IsFrozen
    {
        get => m_isFrozen;
        set
        {
            m_isFrozen = value;

            if (m_isFrozen)
                m_animatorFrozen.SetTrigger("Freeze");
            else
                m_animatorFrozen.SetTrigger("Unfreeze");

            if (item != null)
                item.SetScale();
        }
    }

    public Vector2 Point
    {
        get => transform.position;
    }

    Animator m_animatorFrozen;

    public void Init (GameData _gameData, Vector3 _point, Vector2 _scale)
    {
        m_animatorFrozen = rootFrozen.GetComponent<Animator>();

        if (rootIcon.TryGetComponent(out SpriteRenderer spriteRenderer))
            spriteRenderer.color = ((x + y) % 2 == 0) ? _gameData.tileColorA : _gameData.tileColorB;

        transform.position = _point;
        transform.localScale = _scale;
    }

    public bool IsEmpty ()
    {
        return item == null;
    }
}
