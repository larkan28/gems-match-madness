using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemLightning : Item
{
    public float collectDelay;
    public float destroyDelay;

    ParticleSystem m_effect;
    List<Item> m_listItems = new List<Item>();

    public override void OnEffect (ItemData _matching)
    {
        if (_matching == null)
            return;

        m_boardManager.isWaitingForResponse = true;
        PlayAnimation("Effect");

        if (Data.effect != null)
            m_effect = Instantiate(Data.effect, transform.position, Quaternion.identity);

        Tile[,] tiles = m_boardManager.Board.Tiles;
        m_listItems.Clear();

        foreach (var tile in tiles)
        {
            if (tile == null || tile.IsFrozen)
                continue;

            Item itemOnTile = tile.item;

            if (itemOnTile == null || itemOnTile == this)
                continue;

            if (!_matching.MatchsTo(itemOnTile.Data))
                continue;

            m_listItems.Add(itemOnTile);
        }

        StopCoroutine(ProcessItems());
        StartCoroutine(ProcessItems());
    }

    IEnumerator ProcessItems ()
    {
        yield return new WaitForSeconds(destroyDelay);

        foreach (var item in m_listItems)
        {
            yield return new WaitForSeconds(collectDelay);
            item.Collect();
        }

        yield return new WaitForSeconds(destroyDelay);
        Collect();

        if (m_effect != null)
            Destroy(m_effect);

        m_boardManager.isWaitingForResponse = false;
    }
}
