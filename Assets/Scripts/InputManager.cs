using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField]
    Camera raycastCam;

    Room currentRoom;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.touchCount > 0) {
            switch (Input.GetTouch(0).phase) {
                case TouchPhase.Began:
                    Ray ray = raycastCam.ScreenPointToRay(Input.GetTouch(0).position);
                    RaycastHit hit;
                    if(Physics.Raycast(ray, out hit)) {
                        Room temp = hit.collider.GetComponent<Room>();
                        if (temp) {
                            currentRoom = temp;
                            currentRoom.SetMoveRoom(true);
                        }
                    }
                    break;
                case TouchPhase.Moved:
                    currentRoom.MoveRoom(Input.GetTouch(0).deltaPosition);
                    break;
                case TouchPhase.Canceled:
                case TouchPhase.Ended:
                    if(currentRoom != null) {
                        currentRoom.SetMoveRoom(false);
                        currentRoom = null;
                    }
                    break;
            }
        }
    }
}
