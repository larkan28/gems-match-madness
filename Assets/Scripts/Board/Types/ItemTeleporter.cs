using System.Collections;
using UnityEngine;
using TMPro;

public class ItemTeleporter : Item
{
    public TextMeshPro textLifes;
    public float teleportDelay;

    public override void OnInit ()
    {
        textLifes.text = m_itemLife.ToString();
    }

    public override void OnDamaged ()
    {
        m_boardManager.isWaitingForResponse = true;
        textLifes.text = m_itemLife.ToString();

        StopCoroutine(Teleport());
        StartCoroutine(Teleport());
    }

    IEnumerator Teleport ()
    {
        yield return new WaitForSeconds(teleportDelay);
        Tile randomTile = m_boardManager.Board.RandomTile(false, m_tile);

        if (randomTile != null)
        {
            Item randomItem = randomTile.item;

            randomItem.Tile = m_tile;
            randomItem.PlayAnimation("Teleport");

            Tile = randomTile;
            PlayAnimation("Teleport");
        }

        m_gameSound.PlaySound(m_gameData.soundTeleport);
        m_boardManager.isWaitingForResponse = false;
    }
}