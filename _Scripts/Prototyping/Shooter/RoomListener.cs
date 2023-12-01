using UnityEngine;

public class RoomListener : MonoBehaviour
{
    public IntReference roomIndex;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        RoomController room = collision.GetComponent<RoomController>();
        if (room)
        {
            Debug.Log("Room entered: " + room.RoomIndex);
            roomIndex.Value = room.RoomIndex + 1;
        }
    }
}
