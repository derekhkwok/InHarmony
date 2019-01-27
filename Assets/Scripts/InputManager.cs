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
    private float camOriSize;
    private Vector3 camOriPos;
    public static InputManager Instance{
        get { return _instance;}
    }
    private static InputManager _instance;
    private bool holdRoom = false;
    private float holdRoomTime = 0f;

    public Room currentRoom { get; private set; }
    Vector3 offset;

    private float speed = 10f;
    private float x;
    private float z;

    Vector3 lastMousePos;

    [SerializeField]
    float zoomMin, zoomMax;
    float touchDisStart, zoomStart;

    [SerializeField]
    float camSpeed, fadeSpeed;
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
        camOriSize = targetCam.orthographicSize;
        camOriPos = targetCam.transform.position;
    }

    public float GetCamSize()
    {
        return targetCam.orthographicSize;
    }

    public Vector3 GetCamPos()
    {
        return targetCam.transform.position;
    }

    public void ResetCamSetting()
    {
        targetCam.orthographicSize = camOriSize;
        targetCam.transform.position = camOriPos;
    }

    public void ZoomCameraByStage(int stage)
    {
        targetCam.orthographicSize = Mathf.Clamp(zoomMin + stage, zoomMin, zoomMax);
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
                            StageManager.instance.PullRoomSortingToFront(currentRoom.id);
                            StageManager.instance.SetPlayerAgent(currentRoom.id, false);
                            currentRoom.SetNavTag(false);

                            holdRoomTime = 0f;
                            holdRoom = false;

                            offset = currentRoom.transform.position - targetCam.ScreenToWorldPoint(Input.GetTouch(0).position);
                        }
                    } else {
                        camMove = true;
                        touchStartPos = Input.GetTouch(0).position;
                    }
                    touchLastPos = Input.GetTouch(0).position;
                    break;
                case TouchPhase.Moved:
                    if (!camMove) {
                        holdRoom = false;
                        holdRoomTime = 0f;
                        
                        Vector3 camPos = targetCam.ScreenToWorldPoint(Input.GetTouch(0).position);
                        currentRoom.transform.position = new Vector3(camPos.x + offset.x, currentRoom.transform.position.y, camPos.z + offset.z);
                        if (Vector2.Distance(Input.GetTouch(0).position, touchStartPos) > 0.5f) {
                            UI_RotateButton.Instance.RemoveBtn();
                        } else {
                            holdRoomTime += Time.deltaTime;
                            if (holdRoomTime > 0.65f) {
                                if (!holdRoom) {
                                    UI_RotateButton.Instance.gameObject.SetActive(true);
                                    UI_RotateButton.Instance.SetRotateAction(() => {
                                        if (UI_RotateButton.Instance.targetRoom != null)
                                            UI_RotateButton.Instance.targetRoom.transform.eulerAngles =
                                                new Vector3(0f, 90f, 0f) + UI_RotateButton.Instance.targetRoom.transform.eulerAngles;
                                    }, Input.GetTouch(0).position, currentRoom);
                                }
                                holdRoom = true;
                            }
                        }
                        
                    } else {
                        if(Vector2.Distance(Input.GetTouch(0).position, touchStartPos) > 0.5f) {
                            camMoving = true;
                        }
                        if (camMoving) {
                            camMoveSpeed = Input.GetTouch(0).position - touchLastPos;
                        }
                    }
                    touchLastPos = Input.GetTouch(0).position;
                    break;
                case TouchPhase.Canceled:
                case TouchPhase.Ended:
                    if(currentRoom != null && holdRoom == false) {
                        currentRoom.SetMoveRoom(false);
                        Room temp = currentRoom;
                        currentRoom = null;
                        StageManager.instance.SetPlayerAgent(temp.id, true);
                        StageManager.instance.UpdateRoomConnection();
                        StageManager.instance.RoomSniping(temp);
                        temp.SetNavTag(true);
                        currentRoom = null;

                        Invoke("DelayChecking", 0.5f);
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
            if (!canDrag)
                return;

            switch (Input.GetTouch(1).phase) {
                case TouchPhase.Began:
                    touchDisStart = Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);
                    zoomStart = targetCam.orthographicSize;
                    
                    touchLastPos = (Input.GetTouch(0).position + Input.GetTouch(1).position) / 2f;
                    break;
                case TouchPhase.Moved:
                    float touchDis = Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);
                    targetCam.orthographicSize = Mathf.Clamp(zoomStart * touchDisStart / touchDis, zoomMin, zoomMax);

                    camMoveSpeed = (Input.GetTouch(0).position + Input.GetTouch(1).position) / 2f - touchLastPos;
                    camMoving = true;
                    break;
                case TouchPhase.Canceled:
                case TouchPhase.Ended:
                    touchLastPos = Input.GetTouch(0).position;
                    break;
            }
        }

        if (camMoving) {
            targetCam.transform.Translate(camMoveSpeed.x * camSpeed, camMoveSpeed.y * camSpeed, 0f, Space.Self);
            if (!camMove) {
                float v = camMoveSpeed.magnitude;
                v = Mathf.Clamp(v - fadeSpeed, 0f, 20f);
                camMoveSpeed = camMoveSpeed.normalized * v;
                if (v <= 0f)
                    camMoving = false;
            }
        }






#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0)) {
            if (!canDrag)
                return;

            Ray ray = targetCam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit)) {
                Room temp = hit.collider.GetComponent<Room>();
                if(temp) {
                    currentRoom = temp;
                    currentRoom.SetMoveRoom(true);
                    StageManager.instance.UpdateRoomConnection();
                    StageManager.instance.SetPlayerAgent(currentRoom.id, false);
                    holdRoomTime = 0f;
                    holdRoom = false;
                    lastMousePos = Input.mousePosition;
                    offset = currentRoom.transform.position - targetCam.ScreenToWorldPoint(Input.mousePosition);

                    StageManager.instance.PullRoomSortingToFront(currentRoom.id);
                    currentRoom.SetNavTag(false);
                }
            } else {
                camMove = true;
                touchStartPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                touchLastPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            }
        }

        if (camMove) {
            if (Vector2.Distance(Input.mousePosition, touchStartPos) > 0.5f) {
                camMoving = true;
            }
            if (camMoving) {
                camMoveSpeed = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - touchLastPos;
            }
            touchLastPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
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

                UI_RotateButton.Instance.RemoveBtn();
            }
            lastMousePos = Input.mousePosition;

            Vector3 camPos = targetCam.ScreenToWorldPoint(Input.mousePosition);
            currentRoom.transform.position = new Vector3(camPos.x + offset.x, currentRoom.transform.position.y, camPos.z + offset.z);
        }

        if(Input.GetMouseButtonUp(0)) {
            if (currentRoom != null) {
                currentRoom.SetMoveRoom(false);
                Room temp = currentRoom;
                currentRoom = null;
                temp.SetNavTag(true);
                StageManager.instance.SetPlayerAgent(temp.id, true);
                StageManager.instance.UpdateRoomConnection();
                StageManager.instance.RoomSniping(temp);
                Invoke("DelayChecking", 0.5f);
            }
            camMove = false;
        }
#endif
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

    void DelayChecking() {
        UI_Condition.Instance.CheckingPassConditionUI();

        StageManager.instance.CheckWin();
    }
}
