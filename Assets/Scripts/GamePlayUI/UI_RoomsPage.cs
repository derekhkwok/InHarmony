using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_RoomsPage : MonoBehaviour
{
    public GameObject[] defaultRooms;
    List<GameObject> _rooms = new List<GameObject>();

    private int[] _toWaitRoom;
    private int[] _inShowRoom;

    private void Start()
    {
        SetUpRooms(new int[] { 2, 3, 7 });
    }

    public void SetUpRooms( int[] thisStageRooms )
    {
        for ( int i = 0; i < thisStageRooms.Length; i++)
        {
            GameObject _tempRoom = Instantiate(defaultRooms[thisStageRooms[i]]) as GameObject;
            _tempRoom.GetComponent<RectTransform>().SetParent( this.GetComponent< RectTransform >() );
            _rooms.Add(_tempRoom);
            _tempRoom.transform.localPosition = new Vector3(75f, _rooms.Count * 150f -50f, 0f);
        }

    }
}
