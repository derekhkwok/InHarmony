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

    [SerializeField]
    List<Door> doors;

    public List<Room> GetConnectedRooms() {
        List<Room> returnList = new List<Room>();
        foreach (Door d in doors)
            if (d.connectedRoom != null)
                returnList.Add(d.connectedRoom);

        return returnList;
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



    private void Start() {
        foreach (Door d in doors)
            d.SetRoom(this);
    }

}
