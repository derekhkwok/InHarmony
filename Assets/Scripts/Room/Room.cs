using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public int id { get; private set; }
    public void SetID(int input) {
        id = input;
    }

    public bool isMoving { get; private set; }
    
    List<Door> doors;
    [SerializeField]
    public List<int> excludedRooms, mustConnectedRooms;
    [SerializeField]
    int minNumConnectedRoom;

    public Transform startPos;
    public Transform endPos;

    public List<Room> GetConnectedRooms() {
        List<Room> returnList = new List<Room>();
        foreach (Door d in doors)
            if (d.connectedRoom != null)
                returnList.Add(d.connectedRoom);

        return returnList;
    }

    public bool CheckRoomValid() {
        bool returnBool = true;
        List<Room> rm = GetConnectedRooms();
        if (rm.Count >= minNumConnectedRoom)
            returnBool = false;

        List<int> mustConnect = new List<int>(mustConnectedRooms);
        foreach (Room r in rm) {
            if (excludedRooms.Contains(r.id))
                returnBool = false;

            if (mustConnect.Contains(r.id))
                mustConnect.Remove(r.id);
        }
        if (mustConnect.Count > 0)
            returnBool = false;

        return returnBool;

    }

    /* rubbish, no use
    public List<Room> SearchConnectedRooms(Room from, List<Room> excludedRooms = null) {
        List<Room> returnList = new List<Room>();
        foreach(Door d in doors) {
            if((from != null && from != d.connectedRoom)) {
                if(excludedRooms != null) {
                    if (!excludedRooms.Contains(d.connectedRoom))
                        returnList.AddRange(SearchConnectedRooms(this, excludedRooms));
                }else
                    returnList.AddRange(SearchConnectedRooms(this, excludedRooms));

            }
        }

        return returnList;
    }
    */

    public void SetMoveRoom(bool input) {
        isMoving = input;

        foreach (Door d in doors)
            d.Search();
    }

    public void MoveRoom(Vector2 input) {
        transform.Translate(new Vector3(input.x, 0f, input.y));
    }




    private void Start() {
        doors = new List<Door>(GetComponentsInChildren<Door>());
        foreach (Door d in doors)
            d.SetRoom(this);
    }

    public void SetUpRoomsCondition( string _case, string _target )
    {
        if (excludedRooms == null) excludedRooms = new List<int>();
        if (mustConnectedRooms == null) mustConnectedRooms = new List<int>();

        switch (_case) {
            case "x":
                excludedRooms.Add(int.Parse(_target.Replace("r", "")));
                break;

            case "c":
                if (_target != "d2")
                    mustConnectedRooms.Add(int.Parse(_target.Replace("r", "")));
                else
                    minNumConnectedRoom = 2;
                break;
        }
    }

}
