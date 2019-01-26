using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    // Start is called before the first frame update
    //Data to be
    public Transform start;
    public Transform goal;
    public List<int> excludedRoommID;
    public List<int> excludedPersonID;
    public int belongsRoomID = -1;
    public int goalRoomID = -1;
    public int playerID = 0;
    public bool isPathValid = false;
    public UnityEngine.AI.NavMeshAgent navMeshAgent;

    private UnityEngine.AI.NavMeshAgent agent;

    void Start(){
        gameObject.transform.localPosition = start.localPosition;
        agent = navMeshAgent;
        agent.destination = goal.position;
    }

    // Update is called once per frame
    void Update(){
        if(transform.localPosition.x == goal.localPosition.x && transform.localPosition.z == goal.localPosition.z) {
            Transform temp = start;
            start = goal;
            goal = temp;
            agent.destination = goal.position;
        }
    }

    public void SetupPlayerIDAndBelongRoom( int _playerID, int _belongRoom)
    {
        playerID = _playerID;
        belongsRoomID = _belongRoom;
    }

    public void SetUpPlayerCondition(string _case, string _target)
    {
        if (excludedRoommID == null) excludedRoommID = new List<int>();
        if (excludedPersonID == null) excludedPersonID = new List<int>();

        switch (_case)
        {
            case ">":
                goalRoomID = int.Parse(_target.Replace("r", ""));
                break;

            case "x":
                if (_target[0].ToString() == "r")
                    excludedRoommID.Add(int.Parse(_target.Replace("r", "")));
                else if (_target[0].ToString() == "p")
                    excludedPersonID.Add(int.Parse(_target.Replace("p", "")));
                break;
        }
    }
}
