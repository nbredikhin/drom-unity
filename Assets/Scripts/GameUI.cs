using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public GameObject deathScreen;
    public GameObject interactionText;
    public GameObject hud;
    public Image healthBar;
    public GameObject pauseScreen;
    public Text muteButtonText;
    public bool isFakePause = false;
    private bool isGamePaused = false;

    public AudioClip FancyPauseSound;

    void Start()
    {
        deathScreen.SetActive(false);
        interactionText.SetActive(false);
        hud.SetActive(true);
        pauseScreen.SetActive(false);

        muteButtonText.text = PersistentGameState.isGameMuted ? "UNMUTE GAME" : "MUTE GAME";
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (deathScreen.activeInHierarchy)
        {
            return;
        }
        isGamePaused = !isGamePaused;

        if (isFakePause)
        {
            GameObject.FindObjectOfType<PlayerController>().GetComponent<Collider2D>().enabled = !isGamePaused;
            if (isGamePaused)
            {
                GameObject.FindObjectOfType<PlayerController>().GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            }
            else
            {
                GameObject.FindObjectOfType<PlayerController>().GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
            }
            var AS = GetComponent<AudioSource>();
            if (!AS.isPlaying)
            {
                // DigitalRuby.SoundManagerNamespace.SoundManager.PlayOneShotSound(AS, FancyPauseSound);
            }
        }
        else
        {
            Time.timeScale = isGamePaused ? 0 : 1;
        }

        float gameVolume = PersistentGameState.isGameMuted ? 0.0f : 1.0f;
        DigitalRuby.SoundManagerNamespace.SoundManager.SoundVolume = isGamePaused ? 0.0f : gameVolume;
        pauseScreen.SetActive(isGamePaused);
    }

    void OnPlayerDead()
    {
        HideInteractionSuggestion();
        deathScreen.SetActive(true);
        hud.SetActive(false);
    }

    public void OnRespawnClick()
    {
        hud.SetActive(true);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ToggleGameMute()
    {
        PersistentGameState.isGameMuted = !PersistentGameState.isGameMuted;
        muteButtonText.text = PersistentGameState.isGameMuted ? "UNMUTE GAME" : "MUTE GAME";
    }

    public void ShowInteractionSuggestion(InteractableObject targetObject)
    {
        interactionText.SetActive(true);
        var textComponent = interactionText.GetComponentInChildren<Text>();
        var name = targetObject.readableName != null ? targetObject.readableName : "TODO Add name";
        textComponent.text = "Press E to use " + name;
    }

    public void HideInteractionSuggestion()
    {
        interactionText.SetActive(false);
    }

    public void UpdateHealth(float percentage)
    {
        healthBar.fillAmount = percentage;
    }
}
