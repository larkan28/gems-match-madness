using UnityEngine;

public class PlayMultipleSound : MonoBehaviour
{
    public AudioClip[] sounds;
    public bool isUnmuteable;

    public void PlaySound (int _index)
    {
        if (sounds.Length < 1 || _index < 0 || _index > sounds.Length)
            return;

        GameSound.Instance.PlaySound(sounds[_index], isUnmuteable);
    }
}
