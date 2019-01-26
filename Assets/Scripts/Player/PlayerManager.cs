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
}
