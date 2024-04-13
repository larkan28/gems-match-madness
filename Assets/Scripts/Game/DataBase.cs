using UnityEngine;

public static class DataBase
{
    public static void LoadAll ()
    {
        bool isMuted = Util.IntToBool(PlayerPrefs.GetInt("Muted", 0));
        GameSound.Instance.SetMute(isMuted);
    }

    public static void SaveAll ()
    {
        Save("Muted", GameSound.Instance.IsMuted());
        PlayerPrefs.Save();
    }

    public static int Load (string _key)
    {
        return PlayerPrefs.GetInt(_key, 0);
    }

    public static void Save (string _key, int _value)
    {
        PlayerPrefs.SetInt(_key, _value);
    }

    public static void Save (string _key, bool _value)
    {
        PlayerPrefs.SetInt(_key, Util.BoolToInt(_value));
    }
}
