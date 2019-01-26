using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_RotateButton : MonoBehaviour
{
    public static UI_RotateButton Instance;
    System.Action rotateAction;

    void Awake() {
        Instance = this;
        this.gameObject.SetActive(false);
    }

    public void SetRotateAction( System.Action _action, Vector3 mousePoition)
    {
        transform.position = mousePoition;
        rotateAction = _action;
    }

    public void onClick()
    {
        rotateAction();
    }
}
