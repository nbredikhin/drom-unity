using UnityEngine;

public class InteractableObject : MonoBehaviour
{
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
            Debug.Log("Press E to activate");
            isPlayerNear = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            isPlayerNear = false;
        }
    }
}
