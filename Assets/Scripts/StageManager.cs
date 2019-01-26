using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{

    [Header("Stage Library")]
    public GameObject[] stagePrefabs;

    public GameObject currentStage;
    List<Room> currentRooms = new List<Room>();
    //Dictionary<int, Person> currentPersons = new Dictionary<int, Person>();

    public static StageManager instance;
    public bool isWon = false;

    // Start is called before the first frame update
    void Start()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        isWon = false;
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (isWon)
        {
            // TODO: check for animation ends
        }
    }

    public void InitStage(int stage)
    {
        if (currentStage != null)
        {
            Destroy(currentStage);
            currentStage = null;
        }

        //Get Stage Info
        List<string> roomsAndPerson = StageInfo.stageRooms[stage];

        //Get Stage Condition - show On UI
        List<string> stageCondition = StageInfo.stageCondition[stage];
        UI_Condition.Instance.TurnStringToUI(stageCondition);


        currentStage = Instantiate(stagePrefabs[stage]) as GameObject;
        currentStage.transform.parent = transform;
        isWon = false;

        currentRooms.AddRange(GetComponentsInChildren<Room>());
        foreach (Room r in GetComponentsInChildren<Room>())
        {
            currentRooms.Add(r);
        }

    }

    public bool CheckWin()
    {
        if (currentStage == null) return false;

        // Check room connect conditions
        foreach (Room r in currentRooms)
        {
            if (!r.CheckRoomValid()) return false;
        }

        //TODO: shut down inputs and wait for the animation ends
        isWon = true;
        Debug.LogWarning("[GAME] YOU WIN!");
        return true;
    }

    public void CompleteStage()
    {
        Debug.LogWarning("[GAME] CONGRATULATIONS!");
        Destroy(currentStage);
    }

    public List<Room> GetCurrentRoom()
    {
        return currentRooms;
    }
}
