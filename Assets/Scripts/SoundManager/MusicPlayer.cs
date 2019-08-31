using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicPlayer : MonoBehaviour
{
    public static MusicPlayer instance;
    private AudioSource music;
    public static bool isMusicDisabled = false;
    void Awake()
    {
        if (instance != null && instance != this) {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }
        music = GetComponent<AudioSource>();
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (isMusicDisabled)
        {
            music.volume = 0.0f;
        }
        else
        {
            music.volume = PersistentGameState.isGameMuted ? 0.0f : 1.0f;
        }
    }
}
