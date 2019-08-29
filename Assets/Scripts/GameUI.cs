using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    public GameObject deathScreen;

    void Start()
    {
        deathScreen.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnPlayerDead()
    {
        deathScreen.SetActive(true);
    }

    public void OnRespawnClick()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
