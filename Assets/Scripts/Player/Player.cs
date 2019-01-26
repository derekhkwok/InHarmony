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
    public List<int> excludedPersonID; //??
    public bool isRoomPathValid = true;
    public bool isPersonPathValid = true;
    public NavMeshAgent agent;
    public bool IsRoomUpdated = false;


    //Data to be set
    public int roomSourceID = 1;
    public int playerID = 1;
    public int roomTargetID = -1;
    public int currentRoomIn = 1;


    private bool alreadyExcludedRoomList = false;
    private bool alreadyExcludedPersonList = false;
    private NavMeshPath navMeshPath;
    private NavMeshHit hit;

    void Start(){
        navMeshPath = new NavMeshPath();
        gameObject.transform.position = start.position;
        RoomUpdated();
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

        //Debug.Log(Vector3.Distance(goal.position, gameObject.transform.position));
        if(Vector3.Distance(goal.position, gameObject.transform.position) < 2f) {
            Transform temp = start;
            start = goal;
            goal = temp;
            RoomUpdated();
        }

        //if middle rooms to destinations are removed; walk back to starting room
        //if starting room is disconnected; stand idlely
    }

    public void RestPosition(Vector3 newPos) {
        agent.Warp(newPos);
    }

    public void RoomUpdated() {
        Room currMovingRoom = InputManager.GetInstance().currentRoom;
        if(currMovingRoom != null) {
            if(currMovingRoom.id == roomSourceID || currMovingRoom.id == currentRoomIn) {
                gameObject.transform.position = start.position;
            }
        }
        navMeshPath = new NavMeshPath();
        agent.destination = goal.position;
        navMeshPath = agent.path;
        //Debug.Log((NavMesh.SamplePosition(gameObject.transform.position, out hit, 10f, 1)));
        //Debug.Log(NavMesh.CalculatePath(gameObject.transform.position, goal.position, 1, navMeshPath));
        //Debug.Log(navMeshPath.status);

        if(((NavMesh.SamplePosition(gameObject.transform.position, out hit, 10f, 1)) ||
            NavMesh.CalculatePath(gameObject.transform.position, goal.position, 1, navMeshPath)) &&
            (navMeshPath.status == NavMeshPathStatus.PathComplete)) {
            //Debug.Log(navMeshPath.status);
            agent.isStopped = false;

            //Debug.Log("POSSIBLE");
        } else {
            //Debug.Log("NOPE POSSIBLE");
            agent.isStopped = true;
        }

    }
    //Called on OnRoomEnter trigger
    public void IsExcludedRoom(int roomID) {
        if(!alreadyExcludedRoomList && excludedRoomID.Contains(roomID)) {
            isRoomPathValid = false;
            alreadyExcludedRoomList = false;
        }
    }

    
    //Called 
}
