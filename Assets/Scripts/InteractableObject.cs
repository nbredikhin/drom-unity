using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public string readableName;
    private bool isPlayerNear = false;

    void Update()
    {
        if (isPlayerNear && Input.GetKeyDown("e"))
        {
            this.SendMessage("OnInteract");
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            isPlayerNear = true;
            GameObject.Find("UI").SendMessage("ShowInteractionSuggestion", this);
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            isPlayerNear = false;
            GameObject.Find("UI").SendMessage("HideInteractionSuggestion");
        }
    }
}
