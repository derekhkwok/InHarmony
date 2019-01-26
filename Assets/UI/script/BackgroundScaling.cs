using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScaling : MonoBehaviour
{
    private float slope = 0.07f ;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = new Vector3( InputManager.Instance.GetCamPos().x, -10f, InputManager.Instance.GetCamPos().z );
        float scale = slope * (InputManager.Instance.GetCamSize() - 5f) + 0.33f;
        this.transform.localScale = new Vector3(scale, scale, scale);
    }
}
