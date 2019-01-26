using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MainMenuView : MonoBehaviour
{
    public static bool isFirstEnter = true;

    public int currentLv = 1;
    public float tileStartY;
    public float lowestTilePosY;
    public float tilePadding;
    public Animation startAnim;

    public SpriteRenderer[] tileSprs;
    public TextMesh[] tileText;
    public GameObject groundPS;
    public GameObject skyPS;

    public SpriteRenderer rulerLow;
    public SpriteRenderer rulerMid;
    public Transform rulerMidPt;
    public SpriteRenderer rulerHigh;

    private bool canClick = false;
    private Vector3 tileOriScale;

    public GameObject parent;

    public GameObject car1;
    public GameObject car2;

    public GameObject title;

    //private BoxCollider navArea;

    public static MainMenuView instance { get; private set; }
    public static MainMenuView SummonMenu()
    {
        if( instance == null )
        {
            instance = Instantiate(Resources.Load("MainMenu_VP"), new Vector3( 0f, 7f, 0f ), Quaternion.identity, null ) as MainMenuView;
        }
        return instance;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        currentLv = GameManager.maxClearedLv + 1;

        //navArea = GameObject.Find("NavArea").GetComponent<BoxCollider>() ;
        //navArea.enabled = false ;

        //SFXManager.instance.PlaySFX(SFXManager.SFX.WELCOME);

        Invoke("MoveCar1", UnityEngine.Random.Range(2f, 6f));
        Invoke("MoveCar2", UnityEngine.Random.Range(2f, 6f));

        parent.transform.localPosition = new Vector3(0f, 0f, -10f);
        tileOriScale = tileSprs[0].transform.localScale;

        iTween.MoveTo(car1, iTween.Hash(
            "x", -11f,
            "time", UnityEngine.Random.Range( 5f, 8f ),
            "easetype", iTween.EaseType.linear,
            "islocal", true
            ));

        iTween.MoveTo(car2, iTween.Hash(
            "x", 11f,
            "time", UnityEngine.Random.Range(6f, 9f),
            "easetype", iTween.EaseType.linear,
            "islocal", true
            ));

        for (int i = 0; i < currentLv - 1; i++)
        {
            tileSprs[i].transform.localPosition = new Vector3(tileSprs[i].transform.localPosition.x,
                tileSprs[i].transform.localPosition.y, lowestTilePosY + tilePadding * i);
        }
        for (int i = currentLv - 1; i < tileSprs.Length; i++)
        {
            tileSprs[i].transform.localPosition = new Vector3(tileSprs[i].transform.localPosition.x,
                tileSprs[i].transform.localPosition.y, tileStartY);
            if(i >= currentLv - 1)
            {
                tileSprs[i].gameObject.SetActive(false);
            }
        }
        if ( isFirstEnter )
        {
            //GameManager.instance.camOriPos = 
            isFirstEnter = false;
            startAnim.Play();
            Invoke("DropTile", 2f);
            iTween.MoveTo(parent, iTween.Hash(
                "z", 0f,
                "time", 1f,
                "islocal", true,
                "easetype", iTween.EaseType.easeOutCubic
                ));
        }
        else
        {
            title.SetActive(false);
            iTween.MoveTo(parent, iTween.Hash(
                "z", 0f,
                "time", 1f,
                "islocal", true,
                "easetype", iTween.EaseType.easeOutCubic,
                "oncomplete", "DropTile",
                "oncompletetarget", gameObject
                ));
        }
    }

    public void MoveCar1()
    {
        car1.transform.localPosition = new Vector3(11f, car1.transform.localPosition.y, car1.transform.localPosition.z);
        float time = UnityEngine.Random.Range(5f, 8f);
        iTween.MoveTo(car1, iTween.Hash(
            "x", -11f,
            "time", time,
            "easetype", iTween.EaseType.linear,
            "islocal", true
        ));
        Invoke("MoveCar1", time + UnityEngine.Random.Range(3f, 6f));
    }

    public void MoveCar2()
    {
        car2.transform.localPosition = new Vector3(-11f, car2.transform.localPosition.y, car2.transform.localPosition.z);
        float time = UnityEngine.Random.Range(6f, 9f);
        iTween.MoveTo(car2, iTween.Hash(
            "x", 11f,
            "time", time,
            "easetype", iTween.EaseType.linear,
            "islocal", true
        ));
        Invoke("MoveCar2", time + UnityEngine.Random.Range(4f, 7f));
    }

    public void DropTile()
    {
        tileSprs[currentLv - 1].gameObject.SetActive(true);
        iTween.MoveTo(tileSprs[currentLv - 1].gameObject, iTween.Hash(
            "z", lowestTilePosY + tilePadding * ( currentLv - 1),
            "time", 0.15f * ( 7 - currentLv ) + 0.3f,
            "islocal", true,
            "easetype", iTween.EaseType.easeInCubic,
            "oncomplete", "Dust",
            "oncompletetarget", gameObject
            ));
        Invoke("collapseSfx", 0.15f * ( 7 - currentLv ) + 0.3f - 0.15f);
    }

    private void collapseSfx()
    {
        SFXManager.instance.PlaySFX(SFXManager.SFX.buildingCollapse2);
    }

    public void Dust()
    {
        if (currentLv == 1)
        {
            groundPS.gameObject.SetActive(false);
            groundPS.gameObject.SetActive(true);
        }
        else
        {
            skyPS.transform.localPosition = new Vector3(skyPS.transform.localPosition.x, skyPS.transform.localPosition.y, lowestTilePosY + tilePadding * (currentLv - 1) - tilePadding / 2f);
            skyPS.gameObject.SetActive(false);
            skyPS.gameObject.SetActive(true);
        }
        StartCoroutine(ShowFloorText());
    }

    public IEnumerator ShowFloorText()
    {
        /*iTween.ValueTo(this.gameObject, iTween.Hash(
            "from", 0f,
            "to", 1f,
            "time", 0.5f,
            "easetype", iTween.EaseType.linear
            //"onupdate", "UpdateColor"
            ));*/

        yield return new WaitForSeconds(0.5f);

        iTween.MoveTo(rulerLow.gameObject, iTween.Hash(
            "z", -1.8f,
            "time", 0.3f,
            "islocal", true,
            "easetype", iTween.EaseType.easeInOutQuad
            ));

        iTween.MoveTo(rulerHigh.gameObject, iTween.Hash(
            "y", 5.7f / 6f * currentLv,
            "time", 0.3f,
            "islocal", true,
            "easetype", iTween.EaseType.easeInOutQuad
            ));
        iTween.ScaleTo(rulerMidPt.gameObject, iTween.Hash(
            "x", 40f / 6f * currentLv,
            "time", 0.3f,
            "islocal", true,
            "easetype", iTween.EaseType.easeInOutQuad
            ));
        tileText[currentLv - 1].color = Color.red;
        tileText[currentLv - 1].fontSize = 115;
        for ( int i = 0; i < currentLv; i ++ )
        {
            iTween.MoveTo(tileText[i].gameObject, iTween.Hash(
                "x" , -5.77f,
                "time", 0.3f,
                "islocal", true,
                "delay", 0.1f * i,
                "easetype", iTween.EaseType.easeOutCubic
                ));
        }
        yield return new WaitForSeconds(currentLv * 0.1f + 0.3f);
        canClick = true;
    }

    /*public void UpdateColor( float newVal )
    {
        Debug.Log(newVal);
        rulerLow.color = new Color(rulerLow.color.r, rulerLow.color.g, rulerLow.color.b, (float)newVal);
        rulerMid.color = new Color(rulerMid.color.r, rulerMid.color.g, rulerMid.color.b, (float)newVal);
        rulerHigh.color = new Color(rulerHigh.color.r, rulerHigh.color.g, rulerHigh.color.b, (float)newVal);
    }*/

    // Update is called once per frame
    float time = 0f;
    void Update()
    {
        if (canClick)
        {
            time += Time.deltaTime * 3f;
            float y = Mathf.Sin(time - 45f) * 0.05f + 1.05f;
            tileSprs[currentLv - 1].transform.localScale = new Vector3(tileOriScale.x * y, tileOriScale.y * y, 1f);

            Vector3 pos = Vector3.zero;
            if (Input.GetMouseButtonUp(0))
            {
                pos = Input.mousePosition;
            }
            else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                pos = Input.GetTouch(0).position;
            }
            if (pos != Vector3.zero)
            {
                Ray ray = Camera.main.ScreenPointToRay(pos);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider != null && hit.collider.CompareTag("UIMenu"))
                    {
                        int stageID = int.Parse(hit.collider.name.Substring(4, 1));
                        StartCoroutine( EnterStage(stageID) );
                    }
                }
            }
        }
    }

    public IEnumerator EnterStage(int id)
    {
        SFXManager.instance.PlaySFX(SFXManager.SFX.powerUp2);
        iTween.MoveTo(parent, iTween.Hash(
            "z", -10f,
            "time", 0.5f,
            "islocal", true,
            "easetype", iTween.EaseType.easeInCubic
            ));
        //navArea.enabled = true;
        yield return new WaitForSeconds(0.5f);
        // Enterstage
        StageManager.instance.InitStage(id);
        Destroy(this.gameObject);
    }

}
