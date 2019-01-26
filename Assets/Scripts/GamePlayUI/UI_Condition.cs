using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Condition : MonoBehaviour
{
    public static UI_Condition Instance;
    //public Player[] players;
    //public Room[] rooms;
    List<string> conditions;
    public GameObject conditonPointGO;
    List<GameObject> _conditonGameObject = new List<GameObject>();
    public GameObject emptyGamObject;
    public Image _image;
     
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;

        //TurnStringToUI(new List<string>{ "p1|>|r10", "p1|x|p2", "p2|>|r7", "p3|x|p4", "p3|>r9",
                          //"r9|x|r7", "r6|x|r5", "r6|c|d2" }); //stage 5
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
