using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "Gems Match Madness/GameData", order = 0)]
public class GameData : ScriptableObject
{
    [Header("Configs")]
    public float orderItemsDelay;
    public float createItemDelay;
    public float createTileDelay;
    public float startBoardDelay;
    public float removeStepDelay;
    public float removeItemDelay;

    [Range(0, 1)] public float starsPercent3;
    [Range(0, 1)] public float starsPercent2;
    [Range(0, 1)] public float starsPercent1;

    [Header("References")]
    public Tile tilePrefab;
    public ItemData tileFrozen;
    public ItemData[] allNormalItems;
    public ItemData[] allSpecialItems;
    public ComboData[] itemsAtCombo;

    public Color tileColorA;
    public Color tileColorB;
    
    [Header("Audios")]
    public AudioClip soundMenuSelect;
    public AudioClip soundGameWin;
    public AudioClip soundGameLose;
    public AudioClip soundItemCreate;
    public AudioClip soundItemCollect;
    public AudioClip soundReorder;
    public AudioClip soundTeleport;

    public AudioClip[] soundItemSelects;
    public AudioClip[] soundMusics;

    [Range(0f, 1f)] public float musicVolumeInGame;
    [Range(0f, 1f)] public float musicVolumeGameOver;

    public AudioClip SelectSoundAt (int _index)
    {
        return soundItemSelects[Mathf.Clamp(_index, 0, soundItemSelects.Length - 1)];
    }

    public AudioClip RandomMusicSound ()
    {
        return soundMusics[Random.Range(0, soundMusics.Length)];
    }

    public ItemData ItemAtCombo (int _index, ItemData _data)
    {
        ItemData lastReward = null;

        for (int i = 0; i < itemsAtCombo.Length; i++)
        {
            ComboData combo = itemsAtCombo[i];

            if (combo == null || _index < i)
                continue;

            if (combo.isJoker)
            {
                lastReward = ItemByName(combo.itemName);
                continue;
            }

            if (_data == null)
                continue;

            lastReward = ItemByName(combo.itemName, _data.type);
        }

        return lastReward;
    }

    public ItemData ItemByName (string _name, ItemData.Type _color = ItemData.Type.None)
    {
        foreach (var item in allSpecialItems)
        {
            if (item.itemName.Equals(_name) && item.type == _color)
                return item;
        }

        return null;
    }

    public ItemData RandomNormalItem ()
    {
        return allNormalItems[Random.Range(0, allNormalItems.Length)];
    }
}

[System.Serializable]
public class ComboData
{
    public string itemName;
    public bool isJoker;
}