using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGrenade : Item
{
    public AudioClip effect;

    public float collectDelay;
    public float destroyDelay;

    ParticleSystem m_effect;
    List<Item> m_listItems = new List<Item>();

    public override void OnEffect (ItemData _matching)
    {
        m_boardManager.isWaitingForResponse = true;
        
        PlayAnimation("Effect");
        m_gameSound.PlaySound(effect);

        if (Data.effect != null)
            m_effect = Instantiate(Data.effect, transform.position, Quaternion.identity);

        m_listItems.Clear();
        Tile[] neighbours = m_boardManager.Board.GetNeighbours(Tile, true);

        foreach (var neighbour in neighbours)
        {
            if (neighbour == null)
                continue;

            Item itemNeighbour = neighbour.item;

            if (itemNeighbour == null || !itemNeighbour.IsAvailableToSelect())
                continue;

            if (!Data.isJoker && Data.MatchsTo(itemNeighbour.Data))
                continue;

            m_listItems.Add(itemNeighbour);
        }

        StartCoroutine(ProcessItems());
    }

    IEnumerator ProcessItems ()
    {
        yield return new WaitForSeconds(destroyDelay);

        foreach (var item in m_listItems)
        {
            item.Point = m_point;
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
