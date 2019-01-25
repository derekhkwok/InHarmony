using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{

    [Header("Stage Library")]
    public GameObject[] stagePrefabs;

    public GameObject currentStage;
    Dictionary<int, Room> currentRooms = new Dictionary<int, Room>();
    //Dictionary<int, Person> currentPersons = new Dictionary<int, Person>();

    public static StageManager instance;

    // Start is called before the first frame update
    void Start()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitStage(int stage)
    {
        if (currentStage != null)
        {
            Destroy(currentStage);
            currentStage = null;
        }
        currentStage = GameObject.Instantiate(stagePrefabs[stage]) as GameObject;
        currentStage.transform.parent = transform;

<<<<<<< HEAD
/*        foreach (Room r in GetComponentInChildren<Room>())
        {

        }*/
=======
        foreach (Room r in GetComponentsInChildren<Room>())
        {
            //currentRooms.Add(r.roomID, r);
        }
>>>>>>> 69581751b524cf0a8ced0432db73e38bc0c40072

        //foreach (Person r in GetComponentsInChildren<Room>())
        //{
        //    currentRooms.Add(r.roomID, r);
        //}

    }

    public Dictionary<int, List<int>> StartSearchTree(int roomID, List<int> excludeRooms)
    {
        Dictionary<int, List<int>> result = new Dictionary<int, List<int>>();
        SearchTree(roomID, excludeRooms, result, 0);
        return result;

    }

    void SearchTree(int roomID, List<int> excludeRooms, Dictionary<int, List<int>> result, int step)
    {
        step++;
        if(!result.ContainsKey(step))
        {
            result.Add(step, new List<int>());
        }

<<<<<<< HEAD
//        foreach
=======
        //foreach
>>>>>>> 69581751b524cf0a8ced0432db73e38bc0c40072
    }
}
