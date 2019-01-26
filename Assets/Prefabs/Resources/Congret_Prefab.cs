using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Congret_Prefab : MonoBehaviour
{
    public GameObject enterBtn;
    public Action onEnter;

    public static Congret_Prefab Create( Action onEnter)
    {
        Congret_Prefab congret = ((GameObject)Instantiate(Resources.Load("Congret_Prefab"), Vector3.zero, Quaternion.identity)).GetComponent<Congret_Prefab>() ;
        congret.onEnter = onEnter;
        return congret;
    }

    private void Start()
    {
        Invoke("EnableEnterBtn", 2f);
    }

    private void Update()
    {
        Vector3 pos = Vector3.zero;
        if (Input.GetMouseButtonUp(0))
        {
            pos = Input.mousePosition;
        }
        else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            pos = Input.GetTouch(0).position;
        }
        if (pos != Vector3.zero)
        {
            Ray ray = Camera.main.ScreenPointToRay(pos);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider != null && hit.collider.name == "Btn_Enter" )
                {
                    Dismiss();
                }
            }
        }
    }

    private void EnableEnterBtn()
    {
        enterBtn.gameObject.SetActive(true);
    }

    // Update is called once per frame
    public void Dismiss()
    {
        iTween.MoveTo(this.gameObject, iTween.Hash(
            "z", -15f,
            "time", 0.5f,
            "islocal", true,
            "easetype", iTween.EaseType.easeInCubic,
            "oncomplete", "TerminateMyself"
            ));
    }

    public void TerminateMyself()
    {
        onEnter();
        Destroy(this.gameObject);
    }
}
