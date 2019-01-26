using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    [Header("Stage Library")]
    public GameObject[] stagePrefabs;
    Dictionary<int, Room> currentRooms = new Dictionary<int, Room>();
    Dictionary<int, Player> currentPersons = new Dictionary<int, Player>();

    public static StageManager instance;
    public bool isWon = false;

    public GameObject[] personGO;
    public GameObject[] roomGO;

    private bool inited = false;

    // Start is called before the first frame update
    void Awake()
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
        if (inited) return;
        inited = true;

        currentRooms = new Dictionary<int, Room>();
        currentPersons = new Dictionary<int, Player>();

        //Get Stage Info
        List<string> roomsAndPerson = StageInfo.stageRooms[stage];

        foreach (string info in roomsAndPerson)
        {
            int tmpRoomID = int.Parse(info.Split('|')[0]);
            int tmpPersonID = int.Parse(info.Split('|')[1]);

            GameObject tempRoom = Instantiate(roomGO[tmpRoomID]);
            tempRoom.transform.position = new Vector3(Random.Range(-5f, 5f), 0f, Random.Range(-3f, 3f));
            tempRoom.transform.eulerAngles = new Vector3(0f, Random.Range(0, 4) * 90f, 0f);
            Room _tempRoom = tempRoom.GetComponent<Room>();
            currentRooms.Add(tmpRoomID, _tempRoom);

            if (tmpPersonID > 0)
            {
                GameObject tempPlayerManager = Instantiate(personGO[0]);
                Player _tempPlayer = tempPlayerManager.GetComponentInChildren<Player>();
                currentPersons.Add(tmpPersonID, _tempPlayer);
                _tempPlayer.InitPlayer( _tempRoom.startPos, _tempRoom.endPos );
                _tempPlayer.SetupPlayerIDAndRoomID(tmpPersonID, tmpRoomID);
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
                    Player _player = currentPersons[int.Parse(_conStr[0].Substring(1))];
                    _player.SetUpPlayerCondition(_conStr[1], _conStr[2]);
                    _player.InitPlayer(currentRooms[_player.roomSourceID].startPos, currentRooms[_player.roomTargetID].startPos);
                    break;
            }
        }

        //foreach (Player _player in currentPersons.Values)
        //{
        //    _player.InitPlayer(currentRooms[_player.roomSourceID].startPos, currentRooms[_player.roomTargetID].startPos);
        //}
    }

    public bool CheckWin()
    {
        // Check room connect conditions
        foreach (Room r in currentRooms.Values)
        {
            if (!r.CheckRoomValid()) return false;
        }

        //TODO: shut down inputs and wait for the animation ends
        isWon = true;
        inited = false;
        Debug.LogWarning("[GAME] YOU WIN!");
        return true;
    }

    public void CompleteStage()
    {
        Debug.LogWarning("[GAME] CONGRATULATIONS!");

        if (currentRooms != null)
        {
            foreach (Room r in currentRooms.Values)
            {
                Destroy(r);
            }
        }

        if (currentPersons != null)
        {
            foreach (Player p in currentPersons.Values)
            {
                Destroy(p);
            }
        }
    }

    //public List<Room> GetCurrentRoom()
    //{
    //    return currentRooms;
    //}
}
