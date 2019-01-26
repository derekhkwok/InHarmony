using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public partial class StageManager : MonoBehaviour
{
    [Header("Stage Library")]
    public GameObject[] stagePrefabs;
    Dictionary<int, Room> currentRooms = new Dictionary<int, Room>();
    Dictionary<int, Player> currentPersons = new Dictionary<int, Player>();

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
        currentLv = stage;
        InputManager.Instance.SetCanDrag(false);
        inited = true;
        isEnding = false;

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
            _tempRoom.SetID( tmpRoomID );
            currentRooms.Add(tmpRoomID, _tempRoom);

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
                    _player.InitPlayer(currentRooms[_player.roomSourceID].startPos, currentRooms[_player.roomTargetID].startPos);
                    break;
            }
        }

        InputManager.Instance.SetCanDrag(true);
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

            if (myPath.Last() != p.roomTargetID) // not reaching target
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
        Debug.LogWarning("[GAME] YOU WIN!");
        StartCoroutine(CompleteStage());
        return true;
    }

    IEnumerator CompleteStage()
    {
        Debug.LogWarning("[GAME] STAGE CLEAR! CONGRATULATIONS!");
        yield return new WaitForSeconds(1f);
        //TODO: Animation
    }

    void OnClickEndStage() 
    {
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

    Transform tempParent = null;

    public void RoomSniping(Room draggedRoom) {
        foreach(Door d in draggedRoom.doors) {
            if (d.connectedRoom != null) {
                List<Room> roomNeedToMove = d.connectedRoom.SearchConnectedRooms(draggedRoom);
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
}
