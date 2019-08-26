using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

public class DoorController : MonoBehaviour
{
    public Animator doorAnimator;
    public GameObject exitGameObject;
    public GameObject nextCamera;
    public bool isFrontDoor = true;

    void Awake()
    {
        var sprites = GetComponentsInChildren<SpriteRenderer>();
        foreach (var sprite in sprites)
        {
            switch (sprite.gameObject.name)
            {
                case "Door Sprite":
                    sprite.enabled = isFrontDoor;
                    break;
                case "Outer Sprite":
                    sprite.enabled = !isFrontDoor;
                    break;
            }
        }

        doorAnimator.SetBool("IsOpened", false);
    }

    void SetOpened(bool state)
    {
        doorAnimator.SetBool("IsOpened", state);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (exitGameObject && collider.gameObject.tag == "Player")
        {
            collider.gameObject.transform.position = exitGameObject.transform.position;
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
        if (exitGameObject != null)
            Debug.DrawLine(this.transform.position, exitGameObject.transform.position, Color.blue);
    }
}
