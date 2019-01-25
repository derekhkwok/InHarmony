using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{

    [Header("Stage Library")]
    public GameObject[] stagePrefabs;

    public GameObject currentStage;
    List<Room> currentRooms = new List<Room>();
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
        currentRooms.AddRange(GetComponentsInChildren<Room>());
=======
<<<<<<< HEAD
        //foreach (Room r in GetComponentInChildren<Room>())
        //{

        //}
=======
        foreach (Room r in GetComponentsInChildren<Room>())
        {
            currentRooms.Add(r.id, r);
        }
>>>>>>> 9b35121b4f8691f485623a2ede4b78f5c3eec0e3
>>>>>>> ef1e1b35734ad14daa5fc179d800495f40464a03

        //foreach (Person r in GetComponentsInChildren<Room>())
        //{
        //    currentRooms.Add(r.roomID, r);
        //}

    }

    public bool CheckWin()
    {
        if (currentStage == null) return false;

<<<<<<< HEAD
        //foreach
=======
        // Check room connect conditions
        foreach (Room r in currentRooms)
        {

        }
        return true;
>>>>>>> 9b35121b4f8691f485623a2ede4b78f5c3eec0e3
    }
}
