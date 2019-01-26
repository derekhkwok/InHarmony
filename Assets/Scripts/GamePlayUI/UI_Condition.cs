using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Condition : MonoBehaviour
{
    public static UI_Condition Instance;
    List<string> conditions;
    public GameObject conditonPointGO;
    List<GameObject> _conditonGameObject = new List<GameObject>();
    public GameObject emptyGamObject;
    public Image _image;
     
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }

    public void TurnStringToUI( List<string> _conditions)
    {
        conditions = new List<string>();
        _conditonGameObject = new List<GameObject>();

        //print string from as UI conditon
        for (int j = 0 ; j < _conditions.Count; j++)
        {
            string[] _conStr = _conditions[j].Split('|');
            conditions.Add(_conditions[j]);
            GameObject _tempEmpty = Instantiate(emptyGamObject) as GameObject;
            _conditonGameObject.Add(_tempEmpty);
            _tempEmpty.transform.SetParent(conditonPointGO.transform);
            _tempEmpty.transform.localPosition = new Vector3(0f, -45f * (_conditonGameObject.Count-1), 0f);
            _tempEmpty.transform.localScale = Vector3.one;
            _tempEmpty.transform.localRotation = new Quaternion(0f, 0f, 0f, 0f);

            for (int i = 0; i < _conStr.Length; i++)
            {
                Image tempImage = Instantiate(_image) as Image;
                tempImage.gameObject.SetActive(true);
                tempImage.sprite = UI_TextureHelper.Instance.GetSprite(_conStr[i]);
                tempImage.transform.SetParent(_tempEmpty.transform);
                tempImage.transform.localPosition = new Vector3(i * 55f + 20f, 0f, 0f);
                tempImage.transform.localScale = Vector3.one;
                tempImage.transform.localRotation = new Quaternion(0f, 0f, 0f, 0f);
            }
        }
    }

    public void CheckingPassConditionUI() {
        for ( int i = 0; i < conditions.Count; i++) {
            if ( StageManager.instance.CheckCondition( conditions[i]))
            {
                foreach(Image img in _conditonGameObject[i].GetComponentsInChildren<Image>())
                {
                    img.color = new Color(0.3f, 1f, 0.3f);
                }
            }
            else
            {
                foreach (Image img in _conditonGameObject[i].GetComponentsInChildren<Image>())
                {
                    img.color = new Color(1f, 1f, 1f);
                }
            }
        }
    }

    public void WinDestoryObject()
    {
        foreach (GameObject go in _conditonGameObject)
        {
            Destroy(go);
        }

        conditions = new List<string>();
        _conditonGameObject = new List<GameObject>();
    }


}
