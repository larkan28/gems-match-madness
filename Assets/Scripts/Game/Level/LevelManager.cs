using UnityEngine;

[CreateAssetMenu(fileName = "LevelManager", menuName = "Gems Match Madness/LevelManager", order = 0)]
public class LevelManager : ScriptableObject
{
    [Header("Configs")]
    public bool allowDiagonals;

    public int maxMoves;
    public int maxSeconds;

    public Goal[] goals;

    [Header("Size")]
    [Range(0, 1)] public float worldScale;

    public int sizeX;
    public int sizeY;

    [Header("Shape")]
    public TileDestroyer[] tilesToRemove;
    public TileCreator[] itemsToCreate;

    public void Init ()
    {
        for (int i = 0; i < goals.Length; i++)
            goals[i].Init();
    }

    public void SendUpdateToGoals (ItemData _itemData)
    {
        for (int i = 0; i < goals.Length; i++)
            goals[i].Update(_itemData);
    }

    public bool IsGoalsCompleted ()
    {
        for (int i = 0; i < goals.Length; i++)
        {
            if (!goals[i].IsCompleted())
                return false;
        }

        return true;
    }
}

[System.Serializable]
public class TileDestroyer
{
    public int x;
    public int y;

    public bool isFrozen;
}

[System.Serializable]
public class TileCreator
{
    public int x;
    public int y;

    public ItemData itemData;
}