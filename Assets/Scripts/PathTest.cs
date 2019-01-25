using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PathTest : MonoBehaviour
{

    NavMeshAgent agent;

    [SerializeField]
    Transform target;
    NavMeshPath p;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        agent.SetDestination(target.position);
        p = agent.path;
    }
/*
    private void OnDrawGizmos() {
        p = agent.path;
        Debug.Log(p.status);
        foreach (Vector3 v in p.corners) {
            Gizmos.DrawSphere(v, 1f);
        }
    }*/
}
