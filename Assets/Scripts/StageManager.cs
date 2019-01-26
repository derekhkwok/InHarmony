using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{

    [Header("Stage Library")]
    public GameObject[] stagePrefabs;

    public GameObject currentStage;
    Dictionary<int, Room> currentRooms = new Dictionary<int, Room>();
    Dictionary<int, PlayerManager> currentPersons = new Dictionary<int, PlayerManager>();

    public static StageManager instance;
    public bool isWon = false;

    public GameObject[] personGO;
    public GameObject[] roomGO;

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

        currentRooms = new Dictionary<int, Room>();
        currentPersons = new Dictionary<int, PlayerManager>();

        //Get Stage Info
        List<string> roomsAndPerson = StageInfo.stageRooms[stage];

        foreach (string info in roomsAndPerson)
        {
            int tmpRoomID = int.Parse(info.Split('|')[0]);
            int tmpPersonID = int.Parse(info.Split('|')[1]);

            GameObject tempRoom = Instantiate(roomGO[tmpRoomID]);
            Room _tempRoom = tempRoom.AddComponent<Room>();
            currentRooms.Add(tmpRoomID, _tempRoom);

            if (tmpPersonID > 0)
            {
                GameObject tempPlayerManager = Instantiate(personGO[tmpPersonID]);
                PlayerManager _tempPlayer = tempPlayerManager.GetComponent<PlayerManager>();
                currentPersons.Add(tmpPersonID, _tempPlayer);
                _tempPlayer.SetupPlayerIDAndBelongRoom(tmpPersonID, tmpRoomID);
            }
        }

        //Get Stage Condition - show On UI
        List<string> stageCondition = StageInfo.stageCondition[stage];
        UI_Condition.Instance.TurnStringToUI(stageCondition);

        foreach ( string _con in stageCondition) {
            string[] _conStr = _con.Split('|');

            switch (_con[0].ToString())
            {
                case "r":
                case "R":
                    currentRooms[int.Parse(_conStr[0].Substring(1))].SetUpRoomsCondition(_conStr[1], _conStr[2]);
                    break;

                case "p":
                case "P":
                    currentPersons[int.Parse(_conStr[0].Substring(1))].SetUpPlayerCondition(_conStr[1], _conStr[2]);
                    break;
            }
        }
    }

    public bool CheckWin()
    {
        if (currentStage == null) return false;

        // Check room connect conditions
        foreach (Room r in currentRooms.Values)
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

    //public List<Room> GetCurrentRoom()
    //{
    //    return currentRooms;
    //}
}
