using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_RotateButton : MonoBehaviour
{
    public static UI_RotateButton Instance;
    System.Action rotateAction;
    public Room targetRoom;

    void Awake() {
        Instance = this;
        this.gameObject.SetActive(false);
    }

    private void Update() {
        if (targetRoom) {
            transform.position = targetRoom.transform.position;
        }
    }

    public void SetRotateAction( System.Action _action, Vector3 mousePoition, Room inputRoom)
    {
        transform.position = mousePoition;
        rotateAction = _action;
        targetRoom = inputRoom;
    }

    public void RemoveBtn() {
        this.gameObject.SetActive(false);
    }

    public void onClick()
    {
        rotateAction();
    }
}
