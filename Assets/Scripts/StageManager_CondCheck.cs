using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class StageManager : MonoBehaviour
{

    // c = connect
    // x = not connect
    // > = go to
    public bool CheckCondition_Room(string condString)
    {
        if (string.IsNullOrEmpty(condString)) return false;

        string[] cond = condString.Split('|');
        string mode = cond[0].Substring(0, 1);
        int ID = int.Parse(cond[0].Substring(1));
        Tuple<string, int> second = new Tuple<string, int>(cond[2].Substring(0, 1), int.Parse(cond[2].Substring(1)));

        switch (mode)
        {
            case "r":
                switch (cond[1])
                {
                    case "c":
                        switch (second.Item1)
                        {
                            case "d":
                                return currentRooms[ID].GetConnectedRooms().Count == second.Item2;
                            case "r":
                                return currentRooms[ID].GetConnectedRooms().Contains(currentRooms[second.Item2]);
                            default:
                                return false;
                        }
                    case "x":
                        return !currentRooms[ID].GetConnectedRooms().Contains(currentRooms[second.Item2]);
                    default:
                        return false;
                }
            case "p":
                switch (cond[1])
                {
                    case "x":
                        List<int> myPath = currentPersons[ID].GetPath();
                        List<int> otherPath = currentPersons[second.Item2].GetPath();
                        return !myPath.Take(myPath.Count - 1).Intersect(otherPath.Take(otherPath.Count - 1)).Any();
                    case ">":
                        return currentPersons[ID].GetPath().Last() == second.Item2;
                    default:
                        return false;
                }
            default:
                return false;
        }

    }
}
