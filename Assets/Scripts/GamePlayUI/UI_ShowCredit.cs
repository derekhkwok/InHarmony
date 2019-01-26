using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_ShowCredit : MonoBehaviour
{
    bool showCredit = false;
    public GameObject creditPage;

    public void OnClick()
    {
        if (showCredit) {
            iTween.ScaleTo(creditPage, iTween.Hash(
                "x", 0f,
                "y", 0f,
                "z", 0f,
                "time", 0.5f,
                "islocal", true,
                "easetype", iTween.EaseType.easeInOutQuad));
        } else {
            iTween.ScaleTo(creditPage, iTween.Hash(
                "x", 1f,
                "y", 1f,
                "z", 1f,
                "time", 0.5f,
                "islocal", true,
                "easetype", iTween.EaseType.easeInOutQuad));
        }

        showCredit = !showCredit;
    }
}
