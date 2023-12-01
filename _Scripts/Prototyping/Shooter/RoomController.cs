using UnityEngine;
using UnityEngine.Events;

public class RoomController : MonoBehaviour
{
    private static RoomController ACTIVE_ROOM = null;

    [Header("Room Settings")]
    public int enemyCountIncreasePerRoom = 5;
    public bool lockDoorsOnAwake = false;

    [Header("References")]
    public SInt enemiesToClearPerRoom;
    public SBool roomClearInProgress;
    public RunningSet enemySet;
    public RunningSet roomSet;
    public DoorController[] doors;
    public ScaleFoundryController scaleFoundry;

    [Header("Events")]
    public SEvent enemyKilled;
    public SIntegerEvent spawnTriggerEvent;
    public SEvent cancelRoomActivation;
    public SEvent resetEvent;
    [Space] public UnityEvent onActivated = new UnityEvent();
    [Space] public UnityEvent onCleared = new UnityEvent();

    private Collider2D roomCollider;
    private int roomIndex;
    private bool isCleared = false;
    private bool inProgress = false;
    private bool doorsLocked = false;
    private int enemiesCleared = 0;

    public int RoomIndex { get { return roomIndex; } }
    public Bounds RoomBounds { get { return roomCollider.bounds; } }
    public int EnemiesRemaining { get { return enemiesToClearPerRoom.Value - enemiesCleared; } }
    public bool IsCleared { get { return isCleared; } }
    public bool InProgress { get { return inProgress; } }
    public bool DoorsLocked { get { return doorsLocked; } }
    public static RoomController ActiveRoom { get { return ACTIVE_ROOM; } }


    private void Start()
    {
        roomCollider = GetComponent<Collider2D>();
        roomIndex = roomSet.IndexOf(gameObject);
        if (lockDoorsOnAwake)
        {
            LockDoors();
        }
    }

    private void OnEnable()
    {
        enemyKilled.sharedEvent += IncrementKillCount;
        cancelRoomActivation.sharedEvent += SetCleared;
        resetEvent.sharedEvent += Reset;
    }

    private void OnDisable()
    {
        enemyKilled.sharedEvent -= IncrementKillCount;
        cancelRoomActivation.sharedEvent -= SetCleared;
        resetEvent.sharedEvent -= Reset;
    }

    private void Reset()
    {
        enemiesToClearPerRoom.Value = 15;
    }

    private void IncrementKillCount()
    {
        if (inProgress)
        {
            enemiesCleared++;
            if (enemiesCleared >= enemiesToClearPerRoom.Value)
            {
                SetCleared();
            }
        }
    }

    public void NotifyDoorOpened(DoorController door)
    {
        if (!isCleared)
        {
            // lock other 3 doors
            foreach (DoorController otherDoor in doors)
            {
                if (otherDoor != door)
                {
                    otherDoor.LockDoor(false);
                }
            }
        }
    }

    public void LockDoors()
    {
        doorsLocked = true;
        foreach (DoorController door in doors)
        {
            door.LockDoor(true);
        }
    }

    public void UnlockDoors()
    {
        doorsLocked = false;
        foreach (DoorController door in doors)
        {
            door.UnlockDoor();
        }
    }

    [ContextMenu("Set Cleared")]
    public void SetCleared()
    {
        ACTIVE_ROOM = null;
        isCleared = true;
        if (isCleared && inProgress)
        {
            UnlockDoors();
            inProgress = false;
        }
        if (scaleFoundry)
        {
            scaleFoundry.Set(isCleared);
            scaleFoundry.SetInteractable(isCleared);
            scaleFoundry.UpdateInteractableTooltip();
        }
        if (roomClearInProgress != null)
            roomClearInProgress.Value = false;
        if (onCleared != null)
            onCleared.Invoke();
        enemiesToClearPerRoom.Value += enemyCountIncreasePerRoom;
    }

    [ContextMenu("Activate Room")]
    public void ActivateRoom()
    {
        if (inProgress || isCleared)
        {
            Debug.Log("Room already in progress or cleared");
            return;
        }
        if (ACTIVE_ROOM != null && ActiveRoom != this)
        {
            Debug.LogError("Cannot activate room while another room is active: " + roomIndex);
        }
        else
        {
            ACTIVE_ROOM = this;
            inProgress = true;
            LockDoors();
            scaleFoundry.Set(false);
            spawnTriggerEvent.Fire(enemiesToClearPerRoom.Value);
            if (roomClearInProgress != null)
                roomClearInProgress.Value = true;
            if (onActivated != null)
                onActivated.Invoke();
        }
    }
}
