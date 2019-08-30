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

    void Start()
    {
        deathScreen.SetActive(false);
        interactionText.SetActive(false);
        hud.SetActive(true);
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
