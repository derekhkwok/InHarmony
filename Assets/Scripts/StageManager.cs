using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public partial class StageManager : MonoBehaviour
{
    [Header("Stage Library")]
    public GameObject[] stagePrefabs;
    public Dictionary<int, Room> currentRooms = new Dictionary<int, Room>();
    public Dictionary<int, Player> currentPersons = new Dictionary<int, Player>();

    public static StageManager instance { get; private set; }
    public bool isWon = false;

    public GameObject[] personGO;
    public GameObject[] roomGO;

    private bool inited = false;
    bool isEnding = false;

    int currentLv = 0;

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
        isEnding = false;
    }

    // Update is called once per frame
    //void Update()
    //{
    //    if (isWon && !isEnding)
    //    {
    //        // TODO: check for animation ends
    //        //Player[] playerReached = currentPersons.Select((KeyValuePair<int, Player> arg) => arg.Value.AniHaveReachedDest()).ToArray();
    //        //if (playerReached.Length == currentPersons.Count)
    //        //{
    //        //    isEnding = true;
    //        //    StartCoroutine(CompleteStage());
    //        //}

    //    }
    //}

    public void InitStage(int stage)
    {
        if (inited) return;
        inited = true;



        currentLv = stage;
        InputManager.Instance.ZoomCameraByStage(stage);

        InputManager.Instance.SetCanDrag(false);
        isEnding = false;

        currentRooms = new Dictionary<int, Room>();
        currentPersons = new Dictionary<int, Player>();

        //Get Stage Info
        List<string> roomsAndPerson = StageInfo.stageRooms[stage];

        int count = 0;
        foreach (string info in roomsAndPerson)
        {
            int tmpRoomID = int.Parse(info.Split('|')[0]);
            int tmpPersonID = int.Parse(info.Split('|')[1]);

            GameObject tempRoom = Instantiate(roomGO[tmpRoomID]);
            tempRoom.transform.position = new Vector3(7.5f * (count%3), 0f, 3f * count/3);
            tempRoom.transform.eulerAngles = new Vector3(0f, Random.Range(0, 4) * 90f, 0f);
            Room _tempRoom = tempRoom.GetComponent<Room>();
            _tempRoom.SetID( tmpRoomID );
            currentRooms.Add(tmpRoomID, _tempRoom);
            count++;

            if (tmpPersonID > 0)
            {
                GameObject tempPlayerManager = Instantiate(personGO[0]);
                Player _tempPlayer = tempPlayerManager.GetComponentInChildren<Player>();
                currentPersons.Add(tmpPersonID, _tempPlayer);
                _tempPlayer.SetupPlayerIDAndRoomID(tmpPersonID, tmpRoomID);
                _tempPlayer.InitPlayer( _tempRoom.startPos, _tempRoom.endPos);
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
                    //Debug.LogError("Player :" + _player.playerID + ":" + _player.roomTargetID);
                    //_player.InitPlayer(currentRooms[_player.roomSourceID].startPos, currentRooms[_player.roomTargetID].startPos);
                    break;
            }
        }

        InputManager.Instance.SetCanDrag(true);

        InitRoomSortingOrder();
        //foreach (Player _player in currentPersons.Values)
        //{
        //    _player.InitPlayer(currentRooms[_player.roomSourceID].startPos, currentRooms[_player.roomTargetID].startPos);
        //}
    }

    public bool CheckWin()
    {
        InputManager.Instance.SetCanDrag(false);
        // Check room connect conditions
        foreach (Room r in currentRooms.Values)
        {
            if (!r.CheckRoomValid())
            {
                InputManager.Instance.SetCanDrag(true);

                return false;
            }
        }

        // TODO: Player connection
        // Waiting for p.GetPath()
        foreach (Player p in currentPersons.Values)
        {
            bool isValid = true;
            List<int> myPath = p.GetPath();

            if ((myPath.Count > 0 && myPath.Last() != p.roomTargetID) || (myPath.Count == 0 && p.roomTargetID != -1)) // not reaching target
            {
                isValid = false;
            }
            else // check if whether it overlap with its enemies
            {
                foreach (int otherP in p.excludedPersonID)
                {
                    List<int> otherPath = currentPersons[otherP].GetPath();
                    if (myPath.Take(myPath.Count - 1).Intersect(otherPath.Take(otherPath.Count - 1)).Any())
                    {
                        isValid = false;
                        break;
                    }
                }
            }

            if (!isValid)
            {
                InputManager.Instance.SetCanDrag(true);
                return false;
            }
        }

        isWon = true;

        UI_Condition.Instance.WinDestoryObject();
        foreach ( Player player in currentPersons.Values) {
            player.Win();
        }
        Debug.LogWarning("[GAME] YOU WIN!");
        Congret_Prefab congret = Congret_Prefab.Create(() => { OnClickEndStage(); });
        return true;
    }

    void OnClickEndStage() 
    {
        if (currentRooms != null)
        {
            foreach (Room r in currentRooms.Values)
            {
                Destroy(r.gameObject);
            }
        }

        if (currentPersons != null)
        {
            foreach (Player p in currentPersons.Values)
            {
                Destroy(p.gameObject);
            }
        }
        inited = false;
        currentRooms = null;
        currentPersons = null;

        GameManager.RefreshMaxClearedStage(currentLv);

        MainMenuView.SummonMenu();
    }

    public void UpdateRoomConnection() {
        foreach(KeyValuePair<int, Room> r in currentRooms) {
            r.Value.UpdateConnectedRoom();
        }

        foreach (KeyValuePair<int, Player> p in currentPersons)
            p.Value.RoomUpdated();
    }

    public void SetPlayerAgent(int id, bool input) {
        foreach (KeyValuePair<int, Player> p in currentPersons)
            p.Value.SetAgent(id, input);
    }


    Transform tempParent = null;

    public void RoomSniping(Room draggedRoom) {
        foreach(Door d in draggedRoom.doors) {
            if (d.connectedRoom != null) {
                List<Room> roomNeedToMove = d.connectedRoom.SearchConnectedRooms(draggedRoom, new List<Room>() { draggedRoom });
                roomNeedToMove.Add(d.connectedRoom);
                if (tempParent == null)
                    tempParent = new GameObject().GetComponent<Transform>();
                tempParent.position = d.connectedDoor.transform.position;
                foreach (Room r in roomNeedToMove) {
                    r.transform.parent = tempParent;
                }
                tempParent.position = d.transform.position;
                foreach (Room r in roomNeedToMove) {
                    r.transform.parent = null;
                }
            }
        }
    }

    public void InitRoomSortingOrder() {
        for(int i = 0; i < currentRooms.Count; i++) {
            currentRooms.Values.ToArray()[i].GetComponentInChildren<SpriteRenderer>().sortingOrder = i;
        }
    }

    public void PullRoomSortingToFront(int id) {
        if (currentRooms.ContainsKey(id)) {
            int start = currentRooms[id].GetComponentInChildren<SpriteRenderer>().sortingOrder;
            currentRooms[id].GetComponentInChildren<SpriteRenderer>().sortingOrder = currentRooms.Count - 1;
            for(int i = start + 1; i < currentRooms.Count; i++) {
                foreach(Room r in currentRooms.Values.ToArray()) {
                    if(r.GetComponentInChildren<SpriteRenderer>().sortingOrder == i && r.id != id) {
                        r.GetComponentInChildren<SpriteRenderer>().sortingOrder = i - 1;
                        break;
                    }
                }
            }
        }
    }
}
