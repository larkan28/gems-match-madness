using UnityEngine;

public class PlaySingleSound : MonoBehaviour
{
    public AudioClip sound;
    public bool isUnmuteable;
    
    public void PlaySoundClip ()
    {
        if (sound != null)
            GameSound.Instance.PlaySound(sound, isUnmuteable);
    }
}
