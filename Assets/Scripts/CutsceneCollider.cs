using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneCollider : MonoBehaviour
{
    public string[] dialogueStrings;
    public Transform teleportPos;
    public GameObject cutsceneObject;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag != "Player")
        {
            return;
        }
        if (teleportPos != null)
        {
            collider.gameObject.transform.position = teleportPos.position;
        }
        GameObject.FindObjectOfType<GameUI>().LoadCutscene(dialogueStrings, gameObject);
        GameObject.FindObjectOfType<PlayerController>().GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
    }

    void OnCutsceneEnded()
    {
        if (cutsceneObject != null)
        {
            cutsceneObject.SetActive(false);
        }
        GameObject.FindObjectOfType<PlayerController>().GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        gameObject.SetActive(false);
    }
}
