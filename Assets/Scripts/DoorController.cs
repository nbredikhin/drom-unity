using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEditor;

public class DoorController : MonoBehaviour
{
    public SceneAsset nextLevel;
    public Animator doorAnimator;
    public GameObject exitGameObject;
    public GameObject nextCamera;
    public bool isFrontDoor = true;

    public bool _isOpened = false;
    public bool _isLocked = false;

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

        SetOpened(_isOpened);
        SetLocked(_isLocked);
        // Skip animation on start
        doorAnimator.speed = 100;
    }

    void SetOpened(bool state)
    {
        _isOpened = state;
        doorAnimator.SetBool("IsOpened", _isOpened && !_isLocked);
        doorAnimator.speed = 1;
    }

    void SetLocked(bool state)
    {
        _isLocked = state;
        doorAnimator.SetBool("IsOpened", _isOpened && !_isLocked);

        transform.Find("Locked").GetComponent<SpriteRenderer>().enabled = _isLocked && isFrontDoor;
        transform.Find("Locked Outer").GetComponent<SpriteRenderer>().enabled = _isLocked && !isFrontDoor;
        transform.Find("Light Sprite").gameObject.SetActive(!_isLocked);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag != "Player")
        {
            return;
        }
        if (nextLevel != null)
        {
            SceneManager.LoadScene(nextLevel.name);
        }
        else
        {
            if (exitGameObject)
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

    }

    private void OnDrawGizmos()
    {
        if (exitGameObject != null)
            Debug.DrawLine(this.transform.position, exitGameObject.transform.position, Color.blue);
    }
}
