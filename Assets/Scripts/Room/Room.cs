using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    List<Door> doors;

    
    public List<Room> SearchConnectedRooms(Room from, List<Room> excludedRooms = null) {
        List<Room> returnArray = new List<Room>();
        foreach(Door d in doors) {
            if((from != null && from != d.connectedRoom)) {
                if(excludedRooms != null) {
                    if (!excludedRooms.Contains(d.connectedRoom))
                        returnArray.AddRange(SearchConnectedRooms(this, excludedRooms));
                }else
                    returnArray.AddRange(SearchConnectedRooms(this, excludedRooms));

            }
        }

        return returnArray;
    }

}
