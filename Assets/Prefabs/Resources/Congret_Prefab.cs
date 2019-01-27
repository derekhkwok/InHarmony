using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Congret_Prefab : MonoBehaviour
{
    public static Congret_Prefab Instance;

    public GameObject enterBtn;
    public ParticleSystem victoryPS;
    public Action onEnter;
    public Vector3 oriPos;

    public void Create( Action _onEnter)
    {
        onEnter = _onEnter;
        enterBtn.SetActive(true);
        enterBtn.transform.localScale = new Vector3(0f, 0f, 0f);
        enterBtn.transform.position = oriPos;

        SFXManager.instance.PlaySFX(SFXManager.SFX.victory);
        Invoke("EnableEnterBtn", 0.5f);
    }

    private void Start()
    {
        Instance = this;
        oriPos = enterBtn.transform.position;
        enterBtn.SetActive(false);
        enterBtn.transform.localScale = new Vector3(0f, 0f, 0f);
    }

    private void EnableEnterBtn()
    {
        victoryPS.Play();
        iTween.ScaleTo(enterBtn.gameObject, iTween.Hash(
            "scale", Vector3.one,
            "time", 0.5f,
            "islocal", true,
            "easetype", iTween.EaseType.easeOutBack
            ));
    }

    // Update is called once per frame
    public void Dismiss()
    {
        //iTween.MoveTo(this.gameObject, iTween.Hash(
            //"z", -15f,
            //"time", 0.5f,
            //"islocal", true,
            //"easetype", iTween.EaseType.easeInCubic,
            //"oncomplete", "TerminateMyself"
            //));

        iTween.MoveTo(enterBtn.gameObject, iTween.Hash(
            "y", -1000f,
            "time", 0.5f,
            "islocal", true,
            "easetype", iTween.EaseType.easeOutBack
            //"oncomplete", "TerminateMyself"
        ));

        Invoke("TerminateMyself", 1f);
    }

    public void TerminateMyself()
    {
        victoryPS.Stop();
        StageManager.instance.OnClickEndStage();
        enterBtn.SetActive(false);
        enterBtn.transform.localScale = new Vector3(0f, 0f, 0f);
        enterBtn.transform.position = oriPos;

    }
}
