using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Gems Match Madness/ItemData", order = 0)]
public class ItemData : ScriptableObject
{
    public Item itemPrefab;

    public enum Type
    {
        None = 0,
        Red,
        Green,
        Blue,
        Magent,
        Yellow
    };

    public Type type;

    public bool hasEffect;
    public bool affectedByMatch;
    public bool isSelectable;
    public bool isJoker;

    public ParticleSystem effect;
    public Gradient gradient;
    public string itemName;
    public Sprite icon;
    
    public int maxLifes;

    public bool MatchsTo (ItemData _data)
    {
        return _data != null && type == _data.type;
    }

    public bool EqualsTo (ItemData _data)
    {
        return _data != null && itemName.Equals(_data.itemName) && type == _data.type;
    }

    public override string ToString ()
    {
        return itemName;
    }
}