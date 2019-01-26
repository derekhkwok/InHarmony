    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Player: MonoBehaviour {
    // Start is called before the first frame update
    //Data to be get
    public Transform start;
    public Transform goal;
    public List<int> excludedRoomID;
    public List<int> excludedPersonID;
    public bool isRoomPathValid = true;
    public List<int> roomList = new List<int>();
    public NavMeshAgent agent;
    public bool IsRoomUpdated = false;


    //Data to be set
    public int roomSourceID = 1;
    public int playerID = 1;
    public int roomTargetID = -1;
    public int currentRoomIn = 1;


    private bool alreadyExcludedRoomList = false;
    private NavMeshPath navMeshPath;
    private NavMeshHit hit;

    void Start(){
        //navMeshPath = new NavMeshPath();
        //gameObject.transform.position = start.position;
        //RoomUpdated();
        //navMeshPath = agent.path;
        //agent.destination = goal.position;

        //if(((NavMesh.SamplePosition(gameObject.transform.position, out hit, 10f, 1)) ||
        //    NavMesh.CalculatePath(gameObject.transform.position, goal.position, 1, navMeshPath)) &&
        //    (navMeshPath.status == NavMeshPathStatus.PathComplete)) {
        //    Debug.Log(navMeshPath.status);

        //    Debug.Log("POSSIBLE");
        //} else {
        //    Debug.Log("NOPE POSSIBLE");
        //    agent.isStopped = true;
        //}
    }

    // Update is called once per frame
    void Update(){
        if(IsRoomUpdated) {
            RoomUpdated();
            IsRoomUpdated = false;
        }

        if (goal != null)
        {
            //Debug.Log(Vector3.Distance(goal.position, gameObject.transform.position));
            if (Vector3.Distance(goal.position, gameObject.transform.position) < 2f)
            {
                Transform temp = start;
                start = goal;
                goal = temp;
                RoomUpdated();
            }
        }

        //if middle rooms to destinations are removed; walk back to starting room
        //if starting room is disconnected; stand idlely
    }

    public void RestPosition(Vector3 newPos) {
        agent.Warp(newPos);
    }

    public void RoomUpdated() {
        if(!agent.enabled) agent.enabled = true;
        Room currMovingRoom = InputManager.Instance.currentRoom;
        if(currMovingRoom != null) {
            if(currMovingRoom.id == roomSourceID || currMovingRoom.id == currentRoomIn) {
                gameObject.transform.position = start.position;
            }
        }
        navMeshPath = new NavMeshPath();
        agent.destination = goal == null ? start.position : goal.position;
        navMeshPath = agent.path;
        //Debug.Log((NavMesh.SamplePosition(gameObject.transform.position, out hit, 10f, 1)));
        //Debug.Log(NavMesh.CalculatePath(gameObject.transform.position, goal.position, 1, navMeshPath));
        //Debug.Log(navMeshPath.status);

        if(((NavMesh.SamplePosition(gameObject.transform.position, out hit, 10f, 1)) ||
            NavMesh.CalculatePath(gameObject.transform.position, goal.position, 1, navMeshPath)) &&
            (navMeshPath.status == NavMeshPathStatus.PathComplete)) {
            //Debug.Log(navMeshPath.status);
            agent.isStopped = false;
            for(int i = 0; i < navMeshPath.corners.Length - 1; i++) {
                Ray ray = new Ray(navMeshPath.corners[i], (navMeshPath.corners[i + 1] - navMeshPath.corners[i]).normalized);
                RaycastHit hit;
                //Debug.Log(Vector3.Distance(navMeshPath.corners[i + 1], navMeshPath.corners[i]));
                //Debug.DrawRay(ray.origin, Vector3.Distance(navMeshPath.corners[i + 1], navMeshPath.corners[i]) * ray.direction * 50f, Color.red, 5f);
                if(Physics.Raycast(ray, out hit, Vector3.Distance(navMeshPath.corners[i + 1], navMeshPath.corners[i]) * 50f)){
                    //Debug.Log(hit.collider.name);
                    Room hitRoom = hit.collider.transform.GetComponent<Room>();
                    if(hitRoom != null) {
                        if(!roomList.Contains(hitRoom.id)){
                            roomList.Add(hitRoom.id);
                        }
                    }
                }
            }

            //Debug.Log("POSSIBLE");
        } else {
            //Debug.Log("NOPE POSSIBLE");
            agent.isStopped = true;
        }
        //if(NavMesh.CalculatePath(gameObject.transform.position, goal.position, 1, navMeshPath)) {
        //    Debug.Log(navMeshPath.status);
        //    foreach(Vector3 v in navMeshPath.corners)
        //        Debug.DrawRay(v, Vector3.forward, Color.red, 5f);
        //}

    }
    //Called on OnRoomEnter trigger
    public void IsExcludedRoom(int roomID) {
        if(!alreadyExcludedRoomList && excludedRoomID.Contains(roomID)) {
            isRoomPathValid = false;
            alreadyExcludedRoomList = false;
        }
    }

    public void InitPlayer( Transform _start, Transform _goal )
    {
        start = _start;
        goal = _goal;
        navMeshPath = new NavMeshPath();
        gameObject.transform.position = start.position;
        Invoke("RoomUpdated", 3f);
    }

    public void SetupPlayerIDAndRoomID( int _playerID, int _belongRoomID)
    {
        playerID = _playerID;
        roomSourceID = _belongRoomID;
    }

    public void SetUpPlayerCondition( string _case, string _target )
    {
        if (excludedRoomID == null) excludedRoomID = new List<int>();
        if (excludedPersonID == null) excludedPersonID = new List<int>();

        switch(_case)
        {
            case ">":
                roomTargetID = int.Parse(_target.Replace("r", ""));
                break;

            case "x":
                if (_target[0].ToString() == "r")
                    excludedRoomID.Add(int.Parse(_target.Replace("r", "")));
                else if (_target[0].ToString() == "p")
                    excludedPersonID.Add(int.Parse(_target.Replace("p", "")));
                break;
        }
    }
    //Called 
    public List<Room> GetPath() {
        return roomList;
    }
}
