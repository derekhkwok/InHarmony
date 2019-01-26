using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;
    public static int latestClearedLv = 0;

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    public void StartStage(int stage)
    {
        //TODO: Stage script to start
        StageManager.instance.InitStage(stage);
    }

    public static void EndStage(int clearedStage)
    {
        latestClearedLv = clearedStage;
    }

}
