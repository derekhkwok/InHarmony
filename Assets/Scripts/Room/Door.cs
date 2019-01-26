using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{

    [SerializeField]
    float radius = 0.1f;

    public Room selfRoom { get; private set; }
    public void SetRoom(Room input) {
        selfRoom = input;
    }

    public Room connectedRoom { get; private set; }
    public Door connectedDoor { get; private set; }

    public void Search() {
        connectedRoom = null;
        Collider[] cols = Physics.OverlapSphere(transform.position, radius, 1<<9);
        if(cols.Length > 0) {
            Collider tar = null;
            float min = float.MaxValue;
            foreach(Collider c in cols) {
                Door d = c.GetComponent<Door>();
                if (d && !d.selfRoom.isMoving && Vector3.Angle(transform.forward, d.transform.forward) > 175f && !selfRoom.isMoving) {
                    if (Vector3.Distance(transform.position, c.transform.position) < min) {
                        tar = c;
                        min = Vector3.Distance(transform.position, c.transform.position);
                    }
                }
            }

            if (tar != null) {
                connectedRoom = tar.GetComponent<Door>().selfRoom;
                connectedDoor = tar.GetComponent<Door>();
            }
        }
    }


    private void OnDrawGizmos() {
        Gizmos.DrawSphere(transform.position, radius);
    }
}
