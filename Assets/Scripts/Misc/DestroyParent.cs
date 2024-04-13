using UnityEngine;

public class DestroyParent : MonoBehaviour
{
    void Remove ()
    {
        Item m_itemParent = GetComponentInParent<Item>();
        
        if (m_itemParent != null)
            m_itemParent.DestroyImmediatly();
    }

    void SetLayerOrder (int _index)
    {
        if (TryGetComponent(out SpriteRenderer spriteRenderer))
            spriteRenderer.sortingOrder = _index;
    }
}
