using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Room : MonoBehaviour
{
    public int id { get; private set; }
    public void SetID(int input) {
        id = input;
    }

    public bool isMoving { get; private set; }
    
    public List<Door> doors { get; private set; }
    [SerializeField]
    public List<int> excludedRooms, mustConnectedRooms;
    [SerializeField]
    int minNumConnectedRoom;

    public Transform startPos;
    public Transform endPos;

    NavMeshSourceTag navTag;
    NavMeshObstacle[] walls;

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
        if (rm.Count < minNumConnectedRoom || rm.Count <= 0)
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
    
    public List<Room> SearchConnectedRooms(Room from, List<Room> excludedRooms = null) {
        List<Room> returnList = new List<Room>();
        returnList.Add(this);
        if (excludedRooms == null)
            excludedRooms = new List<Room>();
        excludedRooms.Add(this);

        foreach (Door d in doors) {
            if (d.connectedRoom != null) {
                Debug.Log("hihi search");
                if ((from != null && from != d.connectedRoom)) {
                    if (excludedRooms != null) {
                        if (!excludedRooms.Contains(d.connectedRoom))
                            returnList.AddRange(d.connectedRoom.SearchConnectedRooms(this, excludedRooms));
                    } else
                        returnList.AddRange(d.connectedRoom.SearchConnectedRooms(this, excludedRooms));
                }
            }
        }

        return returnList;
    }

    public void UpdateConnectedRoom() {
        foreach (Door d in doors)
            d.Search();
    }

    public void SetMoveRoom(bool input) {
        isMoving = input;


        //do reset person
    }

    public void SetNavTag(bool input) {
//        navTag.enabled = input;
        foreach (NavMeshObstacle n in walls)
            n.enabled = input;
    }




    private void Start() {
        doors = new List<Door>(GetComponentsInChildren<Door>());
        foreach (Door d in doors)
            d.SetRoom(this);

        navTag = GetComponentInChildren<NavMeshSourceTag>();
        walls = GetComponentsInChildren<NavMeshObstacle>();
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

    private void OnTriggerStay(Collider col) {
        Player p = col.GetComponent<Player>();
        if (p != null && this != InputManager.Instance.currentRoom && p.agent.enabled) {
            p.SetCurrentRoomIn(id);
        }
    }

}
