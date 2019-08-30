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

    public bool isFakePause = false;
    private bool isGamePaused = false;

    void Start()
    {
        deathScreen.SetActive(false);
        interactionText.SetActive(false);
        hud.SetActive(true);
        pauseScreen.SetActive(false);
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
        }
        else
        {
            Time.timeScale = isGamePaused ? 0 : 1;
        }

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
