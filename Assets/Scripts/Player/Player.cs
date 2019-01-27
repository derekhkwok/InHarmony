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

    public SpriteRenderer playerFace;

    public ParticleSystem angrayParticle;
    public ParticleSystem sadParticle;
    public ParticleSystem successParticle;


    private bool alreadyExcludedRoomList = false;
    private NavMeshPath navMeshPath;
    private NavMeshHit hit;
    private float time;

    Coroutine peparePath;

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

        if (excludedRoomID.Contains(currentRoomIn))
        {
            //sad
            angrayParticle.gameObject.SetActive(false);
            sadParticle.gameObject.SetActive(true);
            successParticle.gameObject.SetActive(false);
            sadParticle.Play();
            playerFace.sprite = UI_TextureHelper.Instance.GetPeopleFace(playerID + 12);
        } else {
            bool angryMan = false;
            for ( int i = 0; i < excludedPersonID.Count; i++)
            {
               if ( StageManager.instance.currentPersons[excludedPersonID[i]].currentRoomIn == currentRoomIn)
                {
                    //angray
                    angryMan = true;
                    angrayParticle.gameObject.SetActive(true);
                    sadParticle.gameObject.SetActive(false);
                    successParticle.gameObject.SetActive(false);
                    angrayParticle.Play();
                    playerFace.GetComponent<SpriteRenderer>().sprite = UI_TextureHelper.Instance.GetPeopleFace(playerID + 6);
                    break;
                }
            }

            if ( !angryMan)
            {
                playerFace.sprite = UI_TextureHelper.Instance.GetPeopleFace(playerID);
                angrayParticle.gameObject.SetActive(false);
                sadParticle.gameObject.SetActive(false);
                successParticle.gameObject.SetActive(false);
            }
        }

        if (goal != null)
        {
            //Debug.Log(Vector3.Distance(goal.position, gameObject.transform.position));
/*            if (Vector3.Distance(goal.position, gameObject.transform.position) < 2f)
            {
                //time += Time.deltaTime;
                Transform temp = start;
                start = goal;
                goal = temp;
                //if(time > 3f) {
                    RoomUpdated();
                    //time = 0f;
                //}
            }*/
        }

        //if middle rooms to destinations are removed; walk back to starting room
        //if starting room is disconnected; stand idlely
    }

    public void SetCurrentRoomIn(int input) {
        currentRoomIn = input;
    }

    public void Win()
    {
        //win
        angrayParticle.gameObject.SetActive(false);
        sadParticle.gameObject.SetActive(false);
        successParticle.gameObject.SetActive(true);
        successParticle.Play();
    }

    public void RestPosition(Vector3 newPos) {
        agent.Warp(newPos);
    }

    public void RoomUpdated() {
        roomList = new List<int>();
//        if(!agent.enabled) agent.enabled = true;
        Room currMovingRoom = InputManager.Instance.currentRoom;
        if(currMovingRoom != null) {
 /*           if(currMovingRoom.id == currentRoomIn) {
                gameObject.transform.position = start.position;
            } else if(currMovingRoom.id == roomSourceID) {
                gameObject.transform.position = start.position;
            }*/
            
        }
        gameObject.transform.position = start.position;
        if (!agent.enabled)
            return;

        navMeshPath = new NavMeshPath();
        agent.destination = goal == null ? start.position : goal.position;
        navMeshPath = agent.path;
        
        Debug.Log((NavMesh.SamplePosition(gameObject.transform.position, out hit, 10f, 1)));
        Debug.Log(NavMesh.CalculatePath(gameObject.transform.position, goal.position, 1, navMeshPath));
        Debug.Log(navMeshPath.status);

        StopAllCoroutines();
        StartCoroutine(PeparePath());

    }

    IEnumerator PeparePath() {
        while (agent.pathPending)
            yield return 0;

        if (((NavMesh.SamplePosition(gameObject.transform.position, out hit, 10f, 1)) ||
            NavMesh.CalculatePath(gameObject.transform.position, goal.position, 1, navMeshPath)) &&
            (navMeshPath.status == NavMeshPathStatus.PathComplete)) {
            //Debug.Log(navMeshPath.status);
            agent.isStopped = false;
            roomList.Add(roomSourceID);
            for (int i = 0; i < navMeshPath.corners.Length - 1; i++) {
                Ray ray = new Ray(navMeshPath.corners[i], (navMeshPath.corners[i + 1] - navMeshPath.corners[i]).normalized);
                RaycastHit hit;
                //Debug.Log(Vector3.Distance(navMeshPath.corners[i + 1], navMeshPath.corners[i]));
                Debug.DrawRay(ray.origin, Vector3.Distance(navMeshPath.corners[i + 1], navMeshPath.corners[i]) * ray.direction, Color.red, 5f);
                if (Physics.Raycast(ray, out hit, Vector3.Distance(navMeshPath.corners[i + 1], navMeshPath.corners[i]))) {
                    Debug.Log(hit.collider.name);
                    Room hitRoom = hit.collider.transform.GetComponent<Room>();
                    if (hitRoom != null) {
                        if (!roomList.Contains(hitRoom.id)) {
                            roomList.Add(hitRoom.id);
                        }
                    }
                }
            }
            Debug.Log(roomList.Count);
            SFXManager.instance.PlaySFX(SFXManager.SFX.Footstep01);
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
        currentRoomIn = roomSourceID;
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
        if(StageManager.instance.currentRooms.ContainsKey(roomTargetID))
            goal = StageManager.instance.currentRooms[roomTargetID].endPos;
    }
    //Called 
    public List<int> GetPath() {
        return roomList;
    }

    Transform playerRoot = null;
    public void SetAgent(int id, bool input) {
        if (playerRoot == null)
            playerRoot = transform.parent;
        if (currentRoomIn == id) {
            agent.enabled = input;

            playerRoot.parent = input ? null : StageManager.instance.currentRooms[id].transform;
        }
    }
}
