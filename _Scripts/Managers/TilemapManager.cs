using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TilemapManager", menuName = "SO/Managers/TilemapManager")]
public class TilemapManager : SManager
{
    public PoolManager poolManager;
    // event for holding the action
    [Header("Room Extension Triggers")]
    [SerializeField] private SVector3Event extendRoomLeft;
    [SerializeField] private SVector3Event extendRoomRight;
    [SerializeField] private SVector3Event extendRoomUp;
    [SerializeField] private SVector3Event extendRoomDown;

    [Header("Room Prefabs")]
    [SerializeField] private List<GameObject> roomPrefabs;
    [SerializeField] private List<GameObject> hallwayPrefabs;

    [Header("Events")]
    [SerializeField] private SEvent roomGenerated;
    [SerializeField] private SEvent roomReset;

    private HashSet<Vector3> roomCoords = new HashSet<Vector3>() {
        new Vector3(0, 0)
    };
    private HashSet<Vector3> hallwayCoords = new HashSet<Vector3>();

    // execution command for extend on each direction
    private Action<Vector3> extendLeft;
    private Action<Vector3> extendRight;
    private Action<Vector3> extendUp;
    private Action<Vector3> extendDown;

    private System.Random random = new System.Random();

    public override void OnEnabled()
    {
        base.OnEnabled();

        extendLeft = (position) =>
        {
            Vector3 hallwayPosition = position + new Vector3(-2, -0.5f);
            Vector3 roomPosition = position + new Vector3(-12.5f, -0.5f);
            // Debug.Log("room extended left at: " + roomPosition);
            generateRoom(hallwayPosition, roomPosition, true);
        };
        extendRoomLeft.sharedEvent = extendLeft;

        extendRight = (position) =>
        {
            Vector3 hallwayPosition = position + new Vector3(2, -0.5f);
            Vector3 roomPosition = position + new Vector3(12.5f, -0.5f);
            // Debug.Log("room extended right at: " + roomPosition);
            generateRoom(hallwayPosition, roomPosition, true);
        };
        extendRoomRight.sharedEvent = extendRight;

        extendUp = (position) =>
        {
            Vector3 hallwayPosition = position + new Vector3(0, 2);
            Vector3 roomPosition = position + new Vector3(0, 12);
            // Debug.Log("room extended up at: " + roomPosition);
            generateRoom(hallwayPosition, roomPosition, false);
        };
        extendRoomUp.sharedEvent = extendUp;

        extendDown = (position) =>
        {
            Vector3 hallwayPosition = position + new Vector3(0, -2);
            Vector3 roomPosition = position + new Vector3(0, -13);
            // Debug.Log("room extended down at: " + roomPosition);
            generateRoom(hallwayPosition, roomPosition, false);
        };
        extendRoomDown.sharedEvent = extendDown;
        roomReset.sharedEvent += Reset;
    }

    public override void OnDisabled()
    {
        base.OnDisabled();

        extendRoomLeft.sharedEvent = null;
        extendRoomRight.sharedEvent = null;
        extendRoomUp.sharedEvent = null;
        extendRoomDown.sharedEvent = null;
        roomReset.sharedEvent -= Reset;
    }

    private void generateRoom(Vector3 hallwayPosition, Vector3 roomPosition, bool isHorizontal)
    {
        if (roomGenerated != null)
        {
            roomGenerated.Fire();
        }
        // if hallway in the target position exist, will not instantiate
        // (to prevent putting a door that block the way)
        if (hallwayCoords.Add(hallwayPosition))
        {
            if (isHorizontal)
            {
                Instantiate(hallwayPrefabs[0]).gameObject.transform.position = hallwayPosition;
            }
            else
            {
                Instantiate(hallwayPrefabs[1]).gameObject.transform.position = hallwayPosition;
            }
        }
        else
        {
            // Debug.Log("hallway already exist: " + hallwayPosition);
        }

        if (roomCoords.Add(roomPosition))
        {
            Instantiate(randomRoom()).gameObject.transform.position = roomPosition;
        }
        else
        {
            // Debug.Log("room already exist: " + roomPosition);
        }
    }

    private GameObject randomRoom()
    {
        return roomPrefabs[random.Next(roomPrefabs.Count)];
    }

    public void Reset()
    {
        roomCoords.Clear();
        hallwayCoords.Clear();
        roomCoords.Add(new Vector3(0, 0));
    }
}