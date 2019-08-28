using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTransition : MonoBehaviour
{
    public RoomController nextRoom;
    public Collider2D triggerCollider;
    public Collider2D wallCollider;

    void Start()
    {
        triggerCollider.enabled = true;
        wallCollider.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            triggerCollider.enabled = false;
            wallCollider.enabled = true;
            if (nextRoom != null)
            {
                RoomController.SetActiveRoom(nextRoom);
            }
        }
    }
}
