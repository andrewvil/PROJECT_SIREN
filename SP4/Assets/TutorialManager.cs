using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager instance;
    [SerializeField] private TMP_Text tutorialText;

    [SerializeField] private GameObject tutindicator;
    [SerializeField] private Transform targetCheckpoint;

    [SerializeField] private PlayerData playerData;

    public enum TUTORIAL
    {
        WASDMOVE,
        MOUSELOOK,
        SCROLLINVENTORY,
        CAMERAPIC,
        DONE
    }

    public TUTORIAL tut;

    void Awake()
    {
        if(instance==null)
            instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        if(playerData.hasLoad)
        {
            tut = TUTORIAL.DONE;
        }
        else
        {
            tut = TUTORIAL.WASDMOVE;
            scrollCoroutine = StartCoroutine(ScrollText("W,A,S,D - Move"));
        }
        tutindicator.SetActive(false);
    }

    List<string> wasd = new List<string>();

    bool scrolledUp = false;
    bool scrolledDown = false;

    Vector3 origMousePos;
    Coroutine scrollCoroutine;

    public bool picDone = false;
    void Update()
    {
        switch(tut)
        {
            case TUTORIAL.WASDMOVE:
            if(Input.GetKeyDown(KeyCode.W))
            {
                if(!wasd.Contains("w")) {
                    wasd.Add("w");
                }
            }
            if(Input.GetKeyDown(KeyCode.A))
            {
                if(!wasd.Contains("a")) {
                    wasd.Add("a");
                }
            }
            if(Input.GetKeyDown(KeyCode.S))
            {
                if(!wasd.Contains("s")) {
                    wasd.Add("s");
                }
            }
            if(Input.GetKeyDown(KeyCode.D))
            {
                if(!wasd.Contains("d")) {
                    wasd.Add("d");
                }
            }
            if(wasd.Count >= 4 && scrollCoroutine == null)
            {
                scrollCoroutine = StartCoroutine(ScrollText("Move mouse - Look"));
                origMousePos = Input.mousePosition;
                tut = TUTORIAL.MOUSELOOK;
            }
            break;
            case TUTORIAL.MOUSELOOK:
            if(origMousePos != Input.mousePosition && scrollCoroutine == null)
            {
                StartCoroutine(ScrollText("Mouse scroll - Swap items"));
                tut = TUTORIAL.SCROLLINVENTORY;
            }
            break;
            case TUTORIAL.SCROLLINVENTORY:
            if(Input.GetAxis("Mouse ScrollWheel") > 0f)
            {
                scrolledUp = true;
            }
            else if(Input.GetAxis("Mouse ScrollWheel") < 0f)
            {
                scrolledDown = true;
            }
            if(scrolledUp && scrolledDown && scrollCoroutine==null)
            {
                scrollCoroutine = StartCoroutine(ScrollText("LMB - Use Item (Use the camera to take a picture)"));
                tutindicator.SetActive(true);
                tut = TUTORIAL.CAMERAPIC;
            }
            break;
            case TUTORIAL.CAMERAPIC:
            {
                Vector3 screenPos = Camera.main.WorldToScreenPoint(targetCheckpoint.position);
                tutindicator.transform.position = Vector3.Lerp(tutindicator.transform.position,screenPos,0.1f);
            }
            if(picDone && scrollCoroutine==null)
            {
                tutindicator.SetActive(false);
                StartCoroutine(ScrollText("Good Luck."));
                tut = TUTORIAL.DONE;
            }
            break;
            case TUTORIAL.DONE:
            if(scrollCoroutine == null)
                tutorialText.text = "";
            break;
        }
    }

    private IEnumerator ScrollText(string text)
    {
        tutorialText.text = "";
        foreach(char character in text)
        {
            tutorialText.text = tutorialText.text + character;
            yield return new WaitForSeconds(0.02f);
        }
        yield return new WaitForSeconds(1f);
        scrollCoroutine = null;
    }
}
