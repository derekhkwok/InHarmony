using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UI_TextureHelper : MonoBehaviour
{
    public static UI_TextureHelper Instance;
    public Sprite[] roomTexture; //[0] is null
    public Sprite[] peopleTexture; //[0] is null
    public Sprite[] compareUI;
    private string[] compareStr = new string[] { ">", "x", "c", "d2" };

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
    }

    public Sprite GetSprite( string _textureIndex)
    {
        if (System.Array.IndexOf(compareStr, _textureIndex) != -1)
        {
            return GetConditionUI(_textureIndex);
        }
        else
        {
            switch (_textureIndex[0].ToString())
            {
                case "r":
                case "R":
                    return roomTexture[int.Parse(_textureIndex.Substring(1))];

                case "p":
                case "P":
                    return peopleTexture[int.Parse(_textureIndex.Substring(1))];

                default:
                    return null;
            }
        }
    }

    private Sprite GetConditionUI(string _compareStr)
    {
        return compareUI[System.Array.IndexOf(compareStr, _compareStr)];
    }
}
