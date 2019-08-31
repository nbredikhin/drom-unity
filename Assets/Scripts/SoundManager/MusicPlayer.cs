using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicPlayer : MonoBehaviour
{
    public static MusicPlayer instance;
    private AudioSource music;
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
        if (SceneManager.GetActiveScene().name == "Final Level")
        {
            music.volume = 0.0f;
        }
        music.volume = PersistentGameState.isGameMuted ? 0.0f : 1.0f;
    }
}
