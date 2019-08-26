using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomDoor : MonoBehaviour
{
    public GameObject nextDoor;
    public GameObject nextCamera;

    private void Start()
    {

    }

    private void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (nextDoor && collider.gameObject.tag == "Player")
        {
            collider.gameObject.transform.position = nextDoor.transform.position;
            foreach (var camera in GameObject.FindGameObjectsWithTag("Camera"))
            {
                if (camera != nextCamera)
                {
                    camera.SetActive(false);
                }
            }
            nextCamera.SetActive(true);
        }
    }

    private void OnDrawGizmos()
    {
        if (nextDoor != null)
            Debug.DrawLine(this.transform.position, nextDoor.transform.position, Color.blue);
    }
}
