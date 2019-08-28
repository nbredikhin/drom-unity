using UnityEngine;
using Cinemachine;

public class RoomController : MonoBehaviour
{
    private static RoomController activeRoom;

    public bool isDefaultRoom = false;
    public CinemachineVirtualCamera roomCamera;
    // GameObjects to be activated when room entered
    public GameObject[] roomEnemies;
    public Transform enterancePosition;
    // Door to be unlocked when all enemies are dead;
    public DoorController unlockDoorOnClear;
    // Current room state
    private bool isRoomActive = false;
    private float clearCheckTime = 0.0f;

    public static void SetActiveRoom(RoomController room)
    {
        if (activeRoom == room) return;
        if (activeRoom != null)
        {
            activeRoom.SetRoomActive(false);
        }
        room.SetRoomActive(true);
        activeRoom = room;
    }

    private void Start()
    {
        clearCheckTime = Time.time;
        SetRoomActive(false);

        if (isDefaultRoom)
        {
            ActivateRoom();
        }
    }

    private void Update()
    {
        if (Time.time - clearCheckTime > 1.0f)
        {
            CheckIfRoomClear();
        }
    }

    public void CheckIfRoomClear()
    {
        if (unlockDoorOnClear == null) return;
        int count = 0;
        foreach (var gameObject in roomEnemies)
        {
            if (gameObject != null)
                count++;
        }

        unlockDoorOnClear.SetLocked(count > 0);
    }

    public void ActivateRoom()
    {
        RoomController.SetActiveRoom(this);
    }

    public void SetRoomActive(bool state)
    {
        isRoomActive = state;

        foreach (var gameObject in roomEnemies)
        {
            if (gameObject != null)
                gameObject.SetActive(state);
        }

        roomCamera.gameObject.SetActive(state);

        if (state && enterancePosition != null)
        {
            GameObject.Find("Player").transform.position = enterancePosition.position;
        }
    }
}
