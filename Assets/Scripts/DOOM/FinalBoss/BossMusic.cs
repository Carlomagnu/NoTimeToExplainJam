using UnityEngine;

public class BossMusic : MonoBehaviour
{
    public static BossMusic Instance;

    [SerializeField] AudioSource doomMusic;
    [SerializeField] AudioClip DoomMusic;

    void Awake()
    {
        Instance = this;
    }

    public void PlayDoomMusic()
    {
        if (!doomMusic.isPlaying)
            doomMusic.PlayOneShot(DoomMusic, 0.5f);
    }
}

