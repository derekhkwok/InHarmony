using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager instance;

    private AudioSource source;

    public enum SFX
    {
        WELCOME,
        BTNPRESS,
        TRANSITION,
    }

    void Awake()
    {
        source = GetComponent<AudioSource>();
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
    }

    public void PlaySFX( SFX sfx )
    {
        AudioClip clip = Resources.Load(System.Enum.GetName(sfx.GetType(), sfx)) as AudioClip;
        source.PlayOneShot(clip);
    }
}
