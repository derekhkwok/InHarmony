using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{

    public bool canDrag;
    public void SetCanDrag(bool input) {
        canDrag = input;
    }

    [SerializeField]
    Camera targetCam;
    public static InputManager Instance{
        get { return _instance;}
    }
    private static InputManager _instance;
    private bool holdRoom = false;
    private float holdRoomTime = 0f;

    public Room currentRoom { get; private set; }

    private float speed = 10f;
    private float x;
    private float z;

    Vector3 lastMousePos;

    [SerializeField]
    float zoomMin, zoomMax;
    float touchDisStart, zoomStart;

    [SerializeField]
    float camSpeed;
    bool camMove, camMoving;
    Vector2 camMoveSpeed;
    Vector2 touchStartPos, touchLastPos;
    Vector3 camLastPos;
   
    void Awake(){
        if(_instance != null && _instance != this) {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }


    // Update is called once per frame
    void Update()
    {
        if(Input.touchCount == 1) {
            switch (Input.GetTouch(0).phase) {
                case TouchPhase.Began:
                    if (!canDrag)
                        return;

                    Ray ray = targetCam.ScreenPointToRay(Input.GetTouch(0).position);
                    RaycastHit hit;
                    if(Physics.Raycast(ray, out hit)) {
                        Room temp = hit.collider.GetComponent<Room>();
                        if (temp) {
                            currentRoom = temp;
                            currentRoom.SetMoveRoom(true);

                            StageManager.instance.UpdateRoomConnection();

                            holdRoomTime = 0f;
                            holdRoom = false;
                        }
                    } else {
                        camMove = true;
                        touchStartPos = Input.GetTouch(0).position;
                        touchLastPos = Input.GetTouch(0).position;
                    }
                    break;
                case TouchPhase.Moved:
                    if (!camMove) {
                        holdRoom = false;
                        holdRoomTime = 0f;
                        currentRoom.MoveRoom(targetCam.ScreenToWorldPoint(Input.GetTouch(0).position));
                    } else {
                        if(Vector2.Distance(Input.GetTouch(0).position, touchStartPos) > 0.5f) {
                            camMoving = true;
                        }
                        if (camMoving) {
                            camMoveSpeed = Input.GetTouch(0).position - touchLastPos;
                        }
                    }
                    break;
                case TouchPhase.Canceled:
                case TouchPhase.Ended:
                    if(currentRoom != null && holdRoom == false) {
                        currentRoom.SetMoveRoom(false);
                        currentRoom = null;

                        StageManager.instance.UpdateRoomConnection();
                    }
                    camMove = false;
                    break;

                case TouchPhase.Stationary:
                    if (!canDrag)
                        return;

                    holdRoomTime += Time.deltaTime;
                    if (holdRoomTime > 0.65f) {
                        if ( !holdRoom)
                        {
                            UI_RotateButton.Instance.gameObject.SetActive(true);
                            UI_RotateButton.Instance.SetRotateAction( ()=> {
                                if (UI_RotateButton.Instance.targetRoom != null)
                                    UI_RotateButton.Instance.targetRoom.transform.eulerAngles =
                                        new Vector3(0f, 90f, 0f) + UI_RotateButton.Instance.targetRoom.transform.eulerAngles;
                            }, Input.GetTouch(0).position, currentRoom);
                        }
                        holdRoom = true;
                    }
                    break;
            }
        }else if(Input.touchCount > 1 && currentRoom == null) {
            switch (Input.GetTouch(1).phase) {
                case TouchPhase.Began:
                    touchDisStart = Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);
                    zoomStart = targetCam.orthographicSize;
                    break;
                case TouchPhase.Moved:
                    float touchDis = Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);
                    targetCam.orthographicSize = Mathf.Clamp(zoomStart * touchDisStart / touchDis, zoomMin, zoomMax);
                    break;
                case TouchPhase.Canceled:
                case TouchPhase.Ended:
                    
                    break;
            }
        }

        if (camMoving) {
            targetCam.transform.Translate(camMoveSpeed.x * camSpeed, 0f, camMoveSpeed.y * camSpeed);
            if (!camMove) {
                float v = camMoveSpeed.magnitude;
                v = Mathf.Clamp(v - 0.1f, 0f, 5f);
                camMoveSpeed = camMoveSpeed.normalized * v;
                if (v <= 0f)
                    camMoving = false;
            }
        }







        if (Input.GetMouseButtonDown(0)) {
            Ray ray = targetCam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit)) {
                Room temp = hit.collider.GetComponent<Room>();
                if(temp) {
                    currentRoom = temp;
                    currentRoom.SetMoveRoom(true);
                    StageManager.instance.UpdateRoomConnection();
                    holdRoomTime = 0f;
                    holdRoom = false;
                    lastMousePos = Input.mousePosition;
                }
            }
        }
        if(currentRoom != null) {
            if(Vector3.Distance(Input.mousePosition, lastMousePos) < 1f) {
                holdRoomTime += Time.deltaTime;
                if (holdRoomTime > 0.65f) {
                    if (!holdRoom) {
                        UI_RotateButton.Instance.gameObject.SetActive(true);
                        UI_RotateButton.Instance.SetRotateAction(() => {
                            if (UI_RotateButton.Instance.targetRoom != null)
                                UI_RotateButton.Instance.targetRoom.transform.eulerAngles =
                                    new Vector3(0f, 90f, 0f) + UI_RotateButton.Instance.targetRoom.transform.eulerAngles;
                        }, currentRoom.transform.position, currentRoom);
                    }
                    holdRoom = true;
                }
            } else {
                holdRoomTime = 0f;
                holdRoom = false;
            }
            lastMousePos = Input.mousePosition;

            Vector3 camPos = targetCam.ScreenToWorldPoint(Input.mousePosition);
            currentRoom.transform.position = new Vector3(camPos.x, currentRoom.transform.position.y, camPos.z);
        }

        if(Input.GetMouseButtonUp(0)) {
            if (currentRoom != null) {
                currentRoom.SetMoveRoom(false);
                currentRoom = null;
                StageManager.instance.UpdateRoomConnection();
            }
        }
        //var mousex : float = Input.GetAxis("Mouse X");
        //var mousey : float = Input.GetAxis("Mouse Y");

        //if(mousex > mousexThreshold) {
        //    // Code for mouse moving right
        //} else if(mousex < -mousexThreshold) {
        //    // Code for mouse moving left
        //} else {
        //    // Code for mouse standing still
        //}

        //if(mousey > mouseyThreshold) {
        //    // Code for mouse moving forward
        //} else if(mousey < -mouseyThreshold) {
        //    // Code for mouse moving backward
        //} else {
        //    // Code for mouse standing still
        //}
        //}
    }
}
