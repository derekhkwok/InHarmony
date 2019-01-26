using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShadow : MonoBehaviour
{
    Quaternion rotate_Ori;

    // Start is called before the first frame update
    void Awake()
    {
        rotate_Ori = gameObject.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.rotation = rotate_Ori;
    }
}
