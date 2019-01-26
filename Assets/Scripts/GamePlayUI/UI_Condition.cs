using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Condition : MonoBehaviour
{
    public static UI_Condition Instance;

    public Player[] players;
    public Room[] rooms;
    List<string> conditions;
    public GameObject conditonPointGO;

    List<GameObject> _conditonGameObject = new List<GameObject>();
    public GameObject emptyGamObject;
    public Image _image;
     
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;

        //List<string> testUI = new List<string>()
        //{
        //    "p1|>|r1",
        //    "p1|x|p2",
        //    "r1|x|r2",
        //    "r1|c|d2",
        //    "r1|c|r4",

        //};

        //TurnStringToUI(testUI);
    }

    public void Init()
    {
        //clean old Object 
        foreach ( GameObject go in _conditonGameObject)
        {
            Destroy(go);
        }
        _conditonGameObject = new List<GameObject>();

        //find out all condition and store as string from
        foreach ( Player p in players)
        {
            if ( p.roomTargetID != -1)
            {
                conditions.Add("p" + p.playerID + "|>|r" + p.roomTargetID);
            }

            if ( p.excludedRoomID != null && p.excludedRoomID.Count > 0)
            {
                foreach( int excludeRoomID in p.excludedRoomID)
                {
                    conditions.Add("p" + p.playerID + "|x|r" + excludeRoomID);
                }
            }

            if (p.excludedPersonID != null && p.excludedPersonID.Count > 0)
            {
                foreach (int excludePersonID in p.excludedPersonID)
                {
                    conditions.Add("p" + p.playerID + "|x|r" + excludePersonID);
                }
            }
        }

        foreach ( Room r in rooms)
        {
            if ( r.excludedRooms != null && r.excludedRooms.Count > 0)
            {
                foreach (int excludeRoom in r.excludedRooms)
                {
                    conditions.Add("r" + r.id + "|x|r" + excludeRoom);
                }
            }

            if (r.mustConnectedRooms != null && r.mustConnectedRooms.Count > 0)
            {
                int zeroCount = 0;

                foreach (int mustConnectedRoom in r.mustConnectedRooms)
                {
                    if (mustConnectedRoom == 0) zeroCount++;
                    else if (mustConnectedRoom > 0)
                    {
                        conditions.Add("r" + r.id + "|c|r" + mustConnectedRoom);
                    }
                }
            }
        }

        TurnStringToUI(conditions);
    }

    public void TurnStringToUI( List<string> _conditions)
    {
        //print string from as UI conditon
        foreach (string con in _conditions)
        {
            string[] _conStr = con.Split('|');

            GameObject _tempEmpty = Instantiate(emptyGamObject) as GameObject;
            _conditonGameObject.Add(_tempEmpty);
            _tempEmpty.transform.SetParent(conditonPointGO.transform);
            _tempEmpty.transform.localPosition = new Vector3(0f, -30f * (_conditonGameObject.Count-1), 0f);

            for (int i = 0; i < _conStr.Length; i++)
            {
                Image tempImage = Instantiate(_image) as Image;
                tempImage.gameObject.SetActive(true);
                tempImage.sprite = UI_TextureHelper.Instance.GetSprite(_conStr[i]);
                tempImage.transform.SetParent(_tempEmpty.transform);
                tempImage.transform.localPosition = new Vector3(i * 27f, 0f, 0f);
            }
        }
    }
}
