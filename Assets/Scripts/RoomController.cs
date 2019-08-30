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
    public Transform respawnPosition;
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

        if (GameRespawn.respawnRoomName != null)
        {
            if (GameRespawn.respawnRoomName == name)
            {
                RoomController.SetActiveRoom(this);
            }
        }
        else
        {
            if (isDefaultRoom)
            {
                RoomController.SetActiveRoom(this);
            }
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
        foreach (var enemy in roomEnemies)
        {
            if (enemy != null)
            {
                var healthController = enemy.GetComponent<HealthController>();
                if (healthController != null)
                {
                    if (healthController.health > 0)
                    {
                        count++;
                    }
                }
                else
                {
                    count++;
                }
            }
        }

        unlockDoorOnClear.SetLocked(count > 0);
        unlockDoorOnClear.SetOpened(count == 0);
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

        if (state)
        {
            if (enterancePosition != null)
            {
                GameObject.Find("Player").transform.position = enterancePosition.position;
            }
            else if(respawnPosition != null)
            {
                GameObject.Find("Player").transform.position = respawnPosition.position;
            }

            if (respawnPosition != null || enterancePosition != null)
            {
                GameRespawn.respawnRoomName = name;
            }
        }
    }
}
