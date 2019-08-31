using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEditor;

public class DoorController : MonoBehaviour
{
    public string nextLevel;
    public RoomController nextRoom;
    public Animator doorAnimator;
    public bool isFrontDoor = true;

    [SerializeField] private bool _isOpened = false;
    [SerializeField] private bool _isLocked = false;

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

    public void SetOpened(bool state)
    {
        _isOpened = state;
        doorAnimator.SetBool("IsOpened", _isOpened && !_isLocked);
        doorAnimator.speed = 1;
    }

    public void SetLocked(bool state)
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
        if (nextLevel != null && nextLevel.Length > 1)
        {
            PersistentGameState.respawnRoomName = null;
            SceneManager.LoadScene(nextLevel);
        }
        else if (nextRoom != null)
        {
            RoomController.SetActiveRoom(nextRoom);
        }
    }
}
